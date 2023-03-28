using Cysharp.Threading.Tasks;
using Playstel;
using Project.Scripts.Addressable;
using Project.Scripts.Network;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Zenject
{
    public class ProjectInstaller : MonoInstaller
    {
        public EffectSound Sound;
        public EffectMusic Music;
        public EffectPostFX PostFX;
        public AddressableHandler AddressableHandler;
        public CacheUserSettings CacheUserSettings;
        public NetworkLogin NetworkLogin;
        public NetworkPayload NetworkPayload;
        public NetworkUserStatistic NetworkUserStatistic;
        public NetworkLeaderboard NetworkLeaderboard;
        public NetworkUserData NetworkUserData;
        public HandlerMessages HandlerMessages;
        public HandlerVibration HandlerVibration;
        public HandlerAd HandlerAd;
        
        public override void InstallBindings()
        {
            BindFrom(this);
            BindFrom(Sound);
            BindFrom(Music);
            BindFrom(PostFX);
            BindFrom(AddressableHandler);
            BindFrom(CacheUserSettings);
            BindFrom(NetworkLogin);
            BindFrom(NetworkPayload);
            BindFrom(NetworkUserStatistic);
            BindFrom(HandlerMessages);
            BindFrom(NetworkLeaderboard);
            BindFrom(HandlerVibration);
            BindFrom(NetworkUserData);
            BindFrom(HandlerAd);
        }
        
        private void BindFrom<T>(T instance)
        {
            Container.Bind<T>().FromInstance(instance).AsSingle().NonLazy();
        }
        
        public void BindToSceneContext<T>(GameObject instance)
        {
            if (instance.GetComponent<ZenAutoInjecter>() == null)
            {
                Container.InstantiateComponent<ZenAutoInjecter>(instance)
                    .ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;
            }
        }
        
        public async UniTask<TContract> InstantiateComponent<TContract>(GameObject instance) where TContract : Component
        {
            return Container.InstantiateComponent<TContract>(instance);
        }
    }
}