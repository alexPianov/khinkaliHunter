using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Playstel;
using Project.Scripts.Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class UiMenuMain : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text textBestScore;
    
    [Header("Buttons")]
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonMarket;
    [SerializeField] private Button buttonLeaders; 
    [SerializeField] private Button buttonRegistration;
    
    [Header("Audio")]
    [Inject] private EffectSound Sound;
    [Inject] private EffectMusic Music;

    [Header("Refs")] [SerializeField] private UiMenuWindows Windows;

    [SerializeField] private Canvas[] cavases;

    private void Start()
    {
        SetFrameRate();
        
        SetScore();

        buttonStart.onClick.AddListener(Play);
        buttonLeaders.onClick.AddListener(OpenLeaderboard);
        buttonMarket.onClick.AddListener(OpenMarket);
        buttonRegistration.onClick.AddListener(OpenRegistration);
    }

    private static void SetFrameRate()
    {
        Application.targetFrameRate = 120;
    }

    [Inject] private NetworkPayload _networkPayload;
    [Inject] private NetworkUserStatistic _networkStatistic;
    [Inject] private HandlerVibration _handlerVibration;

    private void SetScore()
    {
        //var bestScore = PlayerPrefs.GetInt("Score");

        var userName = _networkPayload.GetUsername();

        if (string.IsNullOrEmpty(userName))
        {
            textBestScore.GetComponent<Localize>().SetTerm("MainMenu-TextFormat-StatisticAnon");
        }
        else
        {
            textBestScore.GetComponent<Localize>().SetTerm("MainMenu-TextFormat-Statistic");
            
            textBestScore.GetComponent<LocalizationParamsManager>()
                .SetParameterValue("name", userName);
        }

        var bestScore = _networkStatistic.GetStatistic(NetworkUserStatistic.StatisticType.Score);
        
        textBestScore.GetComponent<LocalizationParamsManager>()
            .SetParameterValue("global_score", bestScore.ToString());
    }

    public async void Play()
    {
        Sound.PlaySound(Sound.sounds.ClipClickStart);
        _handlerVibration.Button();
        ClickEffect();
        
        foreach (var canvas in cavases) 
        { 
            canvas.gameObject.SetActive(false);
        }
        
        await UniTask.Delay(400);
        SceneManager.LoadScene(2);
    }

    public void OpenLoading()
    {
        Windows.Active(UiMenuWindows.Windows.Loading);
    }
    
    private void OpenMarket()
    {
        Windows.Active(UiMenuWindows.Windows.Crate);
    }

    private void OpenLeaderboard()
    {
        Windows.Active(UiMenuWindows.Windows.Leaders);
    } 

    private void OpenRegistration()
    {
        Windows.Active(UiMenuWindows.Windows.Registration);
    }

    public void CloseWindow()
    {
        Windows.Active(UiMenuWindows.Windows.None);
    }
    private void ClickEffect()
    {
        buttonStart.interactable = false;
        Music.PlayMusic(null);
    }
}
