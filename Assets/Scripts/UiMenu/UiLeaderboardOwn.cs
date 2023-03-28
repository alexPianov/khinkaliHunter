using Project.Scripts.Network;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiLeaderboardOwn : MonoBehaviour
    {
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textDistance;
        [SerializeField] private TMP_Text textScore;

        [Inject] private NetworkUserStatistic _networkUserStatistic;
        [Inject] private NetworkPayload _networkPayload;
        
        public void UpdateTable()
        {
            textName.text = _networkPayload.GetUsername();
            
            textDistance.text = _networkUserStatistic
                .GetStatistic(NetworkUserStatistic.StatisticType.Distance).ToString();
            
            textScore.text = _networkUserStatistic
                .GetStatistic(NetworkUserStatistic.StatisticType.Score).ToString();
        }
    }
}