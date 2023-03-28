using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab.MultiplayerModels;
using Project.Scripts.Addressable;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiSlotFactory : MonoBehaviour
    {
        public UnityEngine.AddressableAssets.AssetReference asset;
        public Transform assetContainer;

        private List<GameObject> slotTable = new();

        [Inject] private AddressableHandler _addressableHandler;

        public async UniTask<GameObject> CreateSlot(UnityEngine.AddressableAssets.AssetReference assetReference = null)
        {
            if (assetReference == null)
            {
                assetReference = asset;
            }
            
            var slot = await _addressableHandler
                .CreateGameObject(assetReference, assetContainer);
            
            slotTable.Add(slot);

            return slot;
        }

        public void DestroyAllSlots()
        {
            foreach (var slot in slotTable)
            {
                Destroy(slot);
            }
        }
    }
}