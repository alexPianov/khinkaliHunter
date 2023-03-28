using System;
using UnityEngine;
using UnityEngine.UI;
using static Project.Scripts.Network.NetworkUserStatistic;

namespace Playstel
{
    public class UiLeaderboardButtons : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button buttonDistance;
        [SerializeField] private Button buttonScore;
        [SerializeField] private Button buttonOwnStat;

        [Header("Sprite")] 
        [SerializeField] private Sprite spriteEnable;
        [SerializeField] private Sprite spriteDisable;
        
        [Header("Refs")]
        [SerializeField] private UiLeaderboard UiLeaderboard;
        [SerializeField] private UiLeaderboardOwn UiLeaderboardOwn;

        [Header("Panels")] 
        [SerializeField] private GameObject panelGlobal;
        [SerializeField] private GameObject panelOwnStat;

        private void Start()
        {
            buttonDistance.onClick.AddListener(ShowDistance);
            buttonScore.onClick.AddListener(ShowScore);
            buttonOwnStat.onClick.AddListener(ShowOwn);
            ShowDistance();
        }
        
        public void ShowDistance()
        {
            panelOwnStat.SetActive(false);
            panelGlobal.SetActive(true);
            
            UiLeaderboard.UpdateTable(StatisticType.Distance);
            
            buttonDistance.GetComponent<Image>().sprite = spriteEnable;
            buttonScore.GetComponent<Image>().sprite = spriteDisable;
            buttonOwnStat.GetComponent<Image>().sprite = spriteDisable;
        }

        public void ShowScore()
        {
            panelOwnStat.SetActive(false);
            panelGlobal.SetActive(true);
            
            UiLeaderboard.UpdateTable(StatisticType.Score);
            
            buttonDistance.GetComponent<Image>().sprite = spriteDisable;
            buttonScore.GetComponent<Image>().sprite = spriteEnable;
            buttonOwnStat.GetComponent<Image>().sprite = spriteDisable;
        }

        public void ShowOwn()
        {
            panelOwnStat.SetActive(true);
            panelGlobal.SetActive(false);
            
            UiLeaderboardOwn.UpdateTable();
            
            buttonDistance.GetComponent<Image>().sprite = spriteDisable;
            buttonScore.GetComponent<Image>().sprite = spriteDisable;
            buttonOwnStat.GetComponent<Image>().sprite = spriteEnable;
        }

    }
}