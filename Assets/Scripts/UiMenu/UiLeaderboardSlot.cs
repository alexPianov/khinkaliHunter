using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using static Project.Scripts.Network.NetworkUserStatistic;

namespace Playstel
{
    public class UiLeaderboardSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textValue;
         
        public void SetPlayerInfo(PlayerLeaderboardEntry data, StatisticType statisticType)
        {
            textName.text = $"{data.Position + 1}. {data.Profile.DisplayName}";

            if (statisticType == StatisticType.Distance)
            {
                textValue.text = $"{data.StatValue} m";
            }

            if (statisticType == StatisticType.Score)
            {
                textValue.text = $"{data.StatValue}";
            }
        }
    }
}