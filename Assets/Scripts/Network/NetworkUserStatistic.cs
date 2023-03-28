using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Playstel;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Network
{
    public class NetworkUserStatistic : MonoBehaviour
    {
        [Inject] private NetworkPayload _networkPayload;
        
        public enum StatisticType
        {
            Distance, Score
        }
        
        public int GetStatistic(StatisticType dataType)
        {
            var data = _networkPayload.Payload.PlayerStatistics
                .Find(value => value.StatisticName == dataType.ToString());
            
            if (data == null || data.Value == null) return 0;
            return data.Value;
        }

        private bool operationFinish;
        public async UniTask<UpdatePlayerStatisticsResult> SetStatistic
            (List<StatisticUpdate> statisticUpdates)
        {
            UpdatePlayerStatisticsResult operationResult = null;
            operationFinish = false;
        
            UpdatePlayerStatisticsRequest requestData = new UpdatePlayerStatisticsRequest() 
            {
                Statistics = statisticUpdates
            };
        
            PlayFabClientAPI.UpdatePlayerStatistics(requestData, result =>
            {
                operationResult = result;
                operationFinish = true;
            }, error =>
            {
                Debug.Log(error.ErrorMessage);
                operationFinish = true;
            });

            await UniTask.WaitUntil(() => operationFinish);
            
            if (operationResult != null)
            {
                await _networkPayload.GetUserPayload();
            }
            
            return operationResult;
        }
    }
}