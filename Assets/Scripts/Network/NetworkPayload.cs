using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Playstel;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using static Project.Scripts.Network.NetworkUserStatistic;

namespace Project.Scripts.Network
{
    public class NetworkPayload : MonoBehaviour
    {
        [Inject] private CacheUserSettings CacheUserSettings;
        
        public GetPlayerCombinedInfoResultPayload Payload { get; private set; }
        
        public Dictionary<ObscuredString, ObscuredString> UserDataSafe = new();
        public ObscuredInt SavedScore;
        public void SetPayload(GetPlayerCombinedInfoResultPayload payload)
        {
            Payload = payload;
            SafeData(payload);
            SafeScore(payload);
        }

        private void SafeData(GetPlayerCombinedInfoResultPayload payload)
        {
            UserDataSafe = DataHandler.ConvertToSafeData(Payload.UserData);
        }

        private void SafeScore(GetPlayerCombinedInfoResultPayload payload)
        {
            var score = payload.PlayerStatistics
                .Find(value => value.StatisticName == StatisticType.Score.ToString());
            
            if(score == null) return;
            
            SavedScore = score.Value;
        }

        public string GetUsername()
        {
            if (Payload.AccountInfo == null || Payload.AccountInfo.Username == null)
            {
                return "";
            }

            return Payload.AccountInfo.Username.FirstCharacterToUpper();
        }

        private bool operationFinish;
        public async UniTask<GetPlayerCombinedInfoResult> GetUserPayload()
        {
            GetPlayerCombinedInfoResult payloadResult = null;
            operationFinish = false;

            Debug.Log("GetUserPayload"); 
            
            var request = new GetPlayerCombinedInfoRequest
            {
                //PlayFabId = CacheUserSettings.GetPlayFabTitleId(),
                InfoRequestParameters = PayloadAll()
            };

            PlayFabClientAPI.GetPlayerCombinedInfo(request, result =>
            {
                payloadResult = result;
                SetPayload(payloadResult.InfoResultPayload);
                operationFinish = true;
                Debug.Log("UpdateUserPayload is get");
            }, error =>
            {
                operationFinish = true;
            });

            await UniTask.WaitUntil(() => operationFinish);

            return payloadResult;
        }

        public GetPlayerCombinedInfoRequestParams PayloadAll()
        {
            var requestParams = new GetPlayerCombinedInfoRequestParams { };

            requestParams.GetUserAccountInfo = true;
            requestParams.GetPlayerProfile = true;
            requestParams.GetPlayerStatistics = true;
            requestParams.GetUserData = true;

            return requestParams;
        }
    }
}