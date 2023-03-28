using Cysharp.Threading.Tasks;
using Project.Scripts.Zenject;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Project.Scripts.Addressable
{
    public class AddressableHandler : MonoBehaviour
    {
        public enum ObjectName
        {
            SpawnFX, ContractFX, EatFX, PortalFX, 
            DamageFX, SpeedFX, DefenceFX, DoubleJumpFX,
            
            Hinkali, GodsHinkali, ScoreHinkali, 
            
            Spikes,
            
            Bali, Satan, 
            
            DoubleJumpBottle, SpeedReverseBottle, SprikeProtectionBottle,
            
            Ground
        }

        [SerializeField] private ProjectInstaller projectInstaller;

        public enum DecorationType
        {
            Standart, Bali
        }
        
        public async UniTask<GameObject> CreateGameObject(ObjectName assetName, Transform transform = null)
        { 
            var asset = await Addressables
                .InstantiateAsync(assetName.ToString(), transform);
                
            projectInstaller.BindToSceneContext<GameObject>(asset);
            
            ReleaseFromMemoryOnDestroy(asset);

            return asset;
        }
        
        public async UniTask<GameObject> CreateGameObject(AssetReference assetReference, Transform transform = null)
        { 
            var asset = await Addressables
                .InstantiateAsync(assetReference, transform);
            
            projectInstaller.BindToSceneContext<GameObject>(asset);
            
            ReleaseFromMemoryOnDestroy(asset);
            
            return asset;
        }

        public async UniTask<GameObject> GetDecoration(DecorationType decorationType, int decorationIndex, Transform transform = null)
        {
            var decorationName = $"Ground_{decorationType}_{decorationIndex}";
            
            var asset = await Addressables
                .InstantiateAsync(decorationName, transform);
            
            projectInstaller.BindToSceneContext<GameObject>(asset);
            
            ReleaseFromMemoryOnDestroy(asset, true);

            return asset;
        }
        
        private static void ReleaseFromMemoryOnDestroy(GameObject assetInstance, bool releaseOnBecomeInvisible = false)
        {
            if (!assetInstance) return;

            if (assetInstance.GetComponent<NotifyOnDestroy>())
            {
                var notify = assetInstance.GetComponent<NotifyOnDestroy>();
                notify.instanceRef = assetInstance;
                notify.OnBecomeInvisible = releaseOnBecomeInvisible;
                notify.Destroyed += ReleaseFromMemory;
            }
            else
            {
                var notify = assetInstance.AddComponent<NotifyOnDestroy>();
                notify.instanceRef = assetInstance;
                notify.OnBecomeInvisible = releaseOnBecomeInvisible;
                notify.Destroyed += ReleaseFromMemory;
            }
        }
        
        private static void ReleaseFromMemory(GameObject assetReference, NotifyOnDestroy instance)
        {
            assetReference.SetActive(false);
            Addressables.ReleaseInstance(assetReference);
        }
    }
}