using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using Project.Scripts.Network;
using UnityEngine;
using Zenject;
using static Project.Scripts.Network.NetworkUserStatistic;

namespace Playstel
{
    public class UiLeaderboard : UiSlotFactory
    {
        [Inject] private NetworkLeaderboard _networkLeaderboard;
        public UnityEngine.AddressableAssets.AssetReference assetMineSlot;

        private Dictionary<StatisticType, GetLeaderboardResult> cacheData = new();
        private StatisticType currentType;

        [Inject] private NetworkPayload _networkPayload;
        
        public async void UpdateTable(StatisticType statisticType)
        {
            currentType = statisticType;
            DestroyAllSlots();
            
            var leaderboardData = await GetLeaderboardData(statisticType);

            CreateSlots(leaderboardData);
        }

        private async UniTask<GetLeaderboardResult> GetLeaderboardData(StatisticType statisticType)
        {
            GetLeaderboardResult leaderboardData = new();

            if (cacheData.ContainsKey(statisticType))
            {
                cacheData.TryGetValue(statisticType, out var data);
                leaderboardData = data;
            }
            else
            {
                leaderboardData = await _networkLeaderboard.GetLeaderboardData(statisticType);
                cacheData.Remove(statisticType);
                cacheData.Add(statisticType, leaderboardData);
            }

            return leaderboardData;
        }

        private async UniTask CreateSlots(GetLeaderboardResult leaderboardData)
        {
            var userName = _networkPayload.GetUsername();
            Debug.Log("User name: " + userName);
            
            for (var i = 0; i < leaderboardData.Leaderboard.Count; i++)
            {
                var player = leaderboardData.Leaderboard[i];
                
                Debug.Log("Player name: " + player.DisplayName);
                
                if(string.IsNullOrEmpty(player.DisplayName)) continue;

                var slotInstance = await SpawnSlot(player.DisplayName == userName);

                if (slotInstance.TryGetComponent(out UiLeaderboardSlot slot))
                {
                    slot.SetPlayerInfo(player, currentType);
                }
            }
        }

        private async Task<GameObject> SpawnSlot(bool slotIsMine)
        {
            GameObject slotInstance;

            if (slotIsMine)
            {
                slotInstance = await CreateSlot(assetMineSlot);
            }
            else
            {
                slotInstance = await CreateSlot();
            }

            return slotInstance;
        }
    }
}