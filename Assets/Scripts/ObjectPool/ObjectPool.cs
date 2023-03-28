using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab.MultiplayerModels;
using Project.Scripts.Addressable;
using Project.Scripts.Zenject;
using UnityEngine;
using Zenject;
using AssetReference = UnityEngine.AddressableAssets.AssetReference;

namespace Playstel
{
    public class ObjectPool : MonoBehaviour
    {
        #region Pool

        public List<Pool> pools = new List<Pool>();

        [System.Serializable]
        public class Pool
        {
            [Header("Info")] 
            public ObjectType objectType;
            public AssetReference reference;
            public int poolSize;
            
            public enum ObjectType
            {
                JumpDust, SpawnFX, ContractFX, EatFX, PortalFX, DamageFX, SpeedFX, DefenceFX, DoubleJumpFX,
                Khinkali, GodsKhinkali, ScoreKhinkali, 
                Spikes,
                Bali, Satan, 
                DoubleJumpBottle, SpeedReverseBottle, SprikeProtectionBottle
            }

            [Header("Created Objects")]
            public Queue<GameObject> queue = new Queue<GameObject>();
            public List<GameObject> objects = new List<GameObject>();

            public void RemovePool()
            {
                queue.Clear();

                for (int i = 0; i < objects.Count; i++)
                {
                    Destroy(objects[i]);
                }

                objects.Clear();
            }
        }

        #endregion

        #region Create Pools

        [Inject] private AddressableHandler _addressableHandler;
        
        public void Start()
        {
            foreach (var pool in pools)
            {
                CreatePoolObjects(pool);
            }
        }

        public async UniTask CreatePoolObjects(Pool pool)
        {
            for (int i = 0; i < pool.poolSize; i++)
            {
                await CreateObject(pool);
            }
        }

        private async UniTask CreateObject(Pool pool)
        {
            var obj = await _addressableHandler
                .CreateGameObject(pool.reference, transform);

            pool.queue.Enqueue(obj);
            obj.SetActive(false);

            AddEnqueueOnDisable(obj, pool);
        }
        
        public void AddEnqueueOnDisable(GameObject instance, Pool pool)
        {
            var notify = instance.AddComponent<NotifyOnDisable>();

            notify.instanceRef = instance;
            notify.queue = pool.queue;

            notify.Disabled += Enqueue;
        }

        public void Enqueue(GameObject obj, Queue<GameObject> queue)
        {
            queue.Enqueue(obj);
        }

        private void CheckDuplicate(Pool.ObjectType objectType)
        {
            if(pools == null || pools.Count == 0) return;
            
            var poolsList = pools.FindAll(pool => pool.objectType == objectType);

            if (poolsList.Count > 0)
            {
                foreach (var pool in poolsList)
                {
                    pool.RemovePool();
                    pools.Remove(pool);
                }
            }
        }

        #endregion

        #region Get Object

        public GameObject GetFromPool(Pool.ObjectType objectType, Transform targetParent = null)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                var pool = pools[i];

                if (objectType == pool.objectType)
                {
                    if (pool.queue.Count == 0)
                    {
                        Debug.LogError("Pool count is null | " + pool.objectType);
                        return null;
                    }

                    var obj = pool.queue.Dequeue();

                    obj.transform.SetParent(targetParent);

                    obj.SetActive(true);

                    return obj;
                }
            }

            return null;
        }

        #endregion

    }
}
