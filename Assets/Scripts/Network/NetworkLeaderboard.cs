using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Playstel;
using UnityEngine;
using static Project.Scripts.Network.NetworkUserStatistic;

namespace Project.Scripts.Network
{
    public class NetworkLeaderboard : MonoBehaviour
    {
        private bool operationFinish;
        public async UniTask<GetLeaderboardResult> GetLeaderboardData
            (StatisticType statisticName, int maxResults = 95)
        {
            GetLeaderboardResult operationResult = null;
            operationFinish = false;
            var statName = statisticName.ToString();
            
            Debug.Log("GetLeaderboardData: " + statName);

            GetLeaderboardRequest requestData = new();

            var ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowLocations = true,
                ShowStatistics = true
            };

            requestData.StartPosition = 0;
            requestData.StatisticName = statisticName.ToString();
            requestData.MaxResultsCount = maxResults;
            requestData.ProfileConstraints = ProfileConstraints;

            PlayFabClientAPI.GetLeaderboard(requestData, result =>
            {
                operationResult = result;
                
                Debug.Log("GetLeaderboardData result: " + result.Leaderboard.Count);
                Debug.Log("GetLeaderboardData result 1: " + result.Leaderboard[0].Profile.TitleId);
                Debug.Log("GetLeaderboardData result 1: " + result.Leaderboard[0].Profile.PlayerId);
                Debug.Log("GetLeaderboardData result 1: " + result.Leaderboard[0].Profile.PublisherId);
                Debug.Log("GetLeaderboardData result 2: " + result.Leaderboard[0].DisplayName);
                operationFinish = true;
            }, error =>
            {
                Debug.Log("GetLeaderboardData error: " + error.Error);
                operationFinish = true;
            });
            
            await UniTask.WaitUntil(() => operationFinish);
            return operationResult;
        }
    }
}