using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using Playstel;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Network
{
    public class NetworkUserData : MonoBehaviour
    {
        [Inject] private NetworkPayload _networkPayload;
        
        public enum UserDataType
        {
            AdBlindness, DietKhinkali, DoubleJump, SpikeProtection, TimeStone, ChestIsOpen
        }
        
        public bool GetUserData(UserDataType dataType)
        {
            _networkPayload.UserDataSafe
                .TryGetValue(dataType.ToString(), out var data);
            
            if (data == null) return false;
            
            return bool.Parse(data.ToString());
        }

        public async UniTask OpenChest()
        {
            await PlayFabHandler.ExecuteCloudScript
                (PlayFabHandler.Function.PurchaseChest);
            
            await _networkPayload.GetUserPayload();
        }

        public async UniTask UpdateChestItems(ChestItems chestItems)
        {
            string jsonValue = JsonConvert.SerializeObject(chestItems);
            
            Debug.Log("jsonValue: " + jsonValue);

            await PlayFabHandler.ExecuteCloudScript
                (PlayFabHandler.Function.UpdateChestValues, chestItems);
            
            await _networkPayload.GetUserPayload();
        }
        
        public class ChestItems
        {
            public bool AdBlindness;
            public bool DietKhinkali;
            public bool DoubleJump;
            public bool SpikeProtection;
            public bool TimeStone;
        }
    }
}