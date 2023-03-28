using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using I2.Loc;
using PlayFab.ClientModels;
using Playstel;
using Project.Scripts.Addressable;
using Project.Scripts.Network;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class UiScore : MonoBehaviour
{
    [Inject] private Player player;

    [Header("Score")]
    private ObscuredInt score;
    [SerializeField] private TMP_Text textScore;
    [SerializeField] private UiDistance distance;
    [SerializeField] private TMP_Text textBestDistance;
    [SerializeField] private Button buttonRestart;
    
    [Header("Result")]
    [SerializeField] private TMP_Text textScoreResult;
    [SerializeField] private TMP_Text textBestScoreResult;
    [SerializeField] private GameObject results;
    
    [Header("Score Items")]
    [SerializeField] private Transform scoreItem;
    [SerializeField] private AssetReference scoreItemReference;
    [SerializeField] private UiScoreAttractor scoreAttractor;
    
    [Header("Audio")]
    [Inject] private EffectSound Sound;
    [Inject] private EffectMusic Music;

    [Inject] private AddressableHandler _addressableHandler;
    [Inject] private NetworkUserStatistic _networkUserStatistic;
    [Inject] private NetworkPayload _networkPayload;
    [Inject] private HandlerVibration _handlerVibration;
    [Inject] private HandlerAd _handlerAd;

    private void Awake()
    {
        player.ScoreEvent.AddListener(AddScore);
        player.GameOverEvent.AddListener(GameOver);
    }

    private void Start()
    {
        results.SetActive(false);
    }

    private void AddScore()
    {
        score++;
        textScore.text = score.ToString();
    }
    
    private bool isAlreadyOver;
    private bool readyToRestart;
    private async void GameOver()
    {
        if(isAlreadyOver) return;
        
        isAlreadyOver = true;
        results.SetActive(true);
        
        Debug.Log("Game Over");
        Sound.PlaySound(Sound.sounds.ClipGameOver);
        Music.PlayMusic(null);
        
        _handlerVibration.Death();

        var username = _networkPayload.GetUsername();
        
        var savedScore = _networkPayload.SavedScore;
        
        var bestDistance = _networkUserStatistic
            .GetStatistic(NetworkUserStatistic.StatisticType.Distance);
        
        var distanceInt = distance.GetDistance();

        if (string.IsNullOrEmpty(username))
        {
            textScoreResult.GetComponent<Localize>().SetTerm("Game-Result-TextFormat-DistanceAnon");
        }
        else
        {
            textScoreResult.GetComponent<Localize>().SetTerm("Game-Result-TextFormat-Distance");
            textScoreResult.GetComponent<LocalizationParamsManager>()
                .SetParameterValue("name", username);
        }
        
        textScoreResult.GetComponent<LocalizationParamsManager>()
            .SetParameterValue("local_distance", distanceInt.ToString());
        
        var Statistics = new List<StatisticUpdate>() { };
        
        if (distanceInt > bestDistance)
        {
            var distanceStat = new StatisticUpdate()
            {
                StatisticName = NetworkUserStatistic.StatisticType.Distance.ToString(),
                Value = distanceInt
            };
            
            Statistics.Add(distanceStat);
            
            textBestDistance.GetComponent<Localize>()
                .SetTerm("Game-Result-TextFormat-RecordNew");
        }
        else
        {
            textBestDistance.GetComponent<Localize>()
                .SetTerm("Game-Result-TextFormat-Record");

            textBestDistance.GetComponent<LocalizationParamsManager>()
                .SetParameterValue("global_distance",bestDistance.ToString());
        }

        var scoreStat = new StatisticUpdate()
        {
            StatisticName = NetworkUserStatistic.StatisticType.Score.ToString(),
            Value = savedScore + score
        };
            
        Statistics.Add(scoreStat);
        _networkUserStatistic.SetStatistic(Statistics);
        
        textBestScoreResult.text = $"{savedScore}";
        scoreAttractor.SetScore(savedScore);
        
        await UniTask.Delay(200);
        
        Sound.PlaySound(Sound.sounds.ClipScore);
        
        await UniTask.Delay(200);
        
        readyToRestart = true;

        for (var i = 0; i < score; i++)
        {
            Sound.PlaySound(Sound.sounds.ClipThrow);
            
            var item = await _addressableHandler
                .CreateGameObject(scoreItemReference);

            if(scoreItem == null) return;
            
            item.transform.position = scoreItem.position;
            
            await UniTask.Delay(40);
        }
        
        await UniTask.Delay(100);
    }

    public void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space) && readyToRestart)
        {
            Restart();
        }
    }

    private async void Restart()
    {
        _handlerVibration.Button();
        Sound.PlaySound(Sound.sounds.ClipClickButton);
        buttonRestart.interactable = false;
        await UniTask.Delay(100);
        await _handlerAd.Check(distance.GetDistance());
        SceneManager.LoadScene(2);
    }
}
