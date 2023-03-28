using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiButtons : MonoBehaviour
    {
        [Header("Result")] 
        [SerializeField] private Button buttonQuit;
        [SerializeField] private Button buttonRestart;
        [SerializeField] private UiDistance uiDistance;
        
        [Header("Pause")] 
        [SerializeField] private Button buttonPause;
        [SerializeField] private Image imagePauseButton;
        [SerializeField] private Sprite[] spritePauseMode;
        [SerializeField] public bool IsPaused;
        [SerializeField] public UiInfo Info;
    
        [Header("Music")] 
        [SerializeField] private Button buttonMusic;
        [SerializeField] private Image imageMusicButton;
        [SerializeField] private Sprite[] spriteMusicMode;
        [SerializeField] private bool IsMusicPaused;
        
        [Header("Refs")] 
        [Inject] private EffectSound Sound;
        [Inject] private EffectMusic Music;

        [Inject] private CacheUserSettings _cacheUserSettings;
        [Inject] private HandlerAd _handlerAd;

        private void Awake()
        {
            buttonPause.onClick.AddListener(Pause);
            buttonMusic.onClick.AddListener(Sounds); 
            buttonQuit.onClick.AddListener(Quit);
            buttonRestart.onClick.AddListener(Restart);
        }

        public void Pause()
        {
            if (IsPaused)
            {
                Info.ShowInfo(UiInfo.InfoType.None);
                Time.timeScale = 1;
                imagePauseButton.sprite = spritePauseMode[1];
                IsPaused = false;
                //Music.SetMuffle(false);
            }
            else
            {
                Info.ShowInfo(UiInfo.InfoType.PauseOn);
                Time.timeScale = 0;
                imagePauseButton.sprite = spritePauseMode[0];
                IsPaused = true;
                //Music.SetMuffle(true);
            }
        }
    
        public void Sounds()
        {
            if (IsMusicPaused)
            {
                Sound.Disable(false);
                Music.Disable(false);
                imageMusicButton.sprite = spriteMusicMode[1];
                IsMusicPaused = false;
                //PlayerPrefs.SetInt("Sound", 0);
                
                _cacheUserSettings.DisableAudioMaster(IsMusicPaused);
            }
            else
            {
                Sound.Disable(true);
                Music.Disable(true);
                imageMusicButton.sprite = spriteMusicMode[0];
                IsMusicPaused = true;
                //PlayerPrefs.SetInt("Sound", 1);
                
                _cacheUserSettings.DisableAudioMaster(IsMusicPaused);
            }
        }
        
        public async void Quit()
        {
            await UniTask.Delay(100);
            await _handlerAd.Check(uiDistance.GetDistance());
            SceneManager.LoadScene(1);
        }

        public async void Restart()
        {
            await UniTask.Delay(100);
            await _handlerAd.Check(uiDistance.GetDistance());
            SceneManager.LoadScene(2);
        }

        private void OnDisable()
        {
            _cacheUserSettings.DisableAudioMaster(false);
        }
    }
}