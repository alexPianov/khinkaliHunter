using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using I2.Loc;
using PlayFab;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Playstel
{
    public class UserSettings : MonoBehaviour
    {
        private readonly ObscuredString playFabTitleID = "";
        
        private readonly int targetFrameRate = 120;
        private readonly int vSyncCount = 0; 

        [Header("Audio")] 
        public float musicLevel;
        public float soundLevel;
        
        [Header("Post Processing")] 
        public bool PostProcessing;
        
        [Header("Vibration")] 
        public Vibration VibrationType;
        public enum Vibration
        {
            Enabled, Disabled
        }
        
        [Header("Notifications")] 
        public Notifications NotificationType;
        public enum Notifications
        {
            Enabled, Disabled
        }
        
        [Header("Audio")]
        public AudioMixer SoundMixer;
        public AudioMixer MusicMixer;

        [Header("Debugging")] 
        public StackTraceLogType StackTrace = StackTraceLogType.ScriptOnly;

        private void Start()
        {
            LoadSettings();
        }

        public async UniTask LoadSettings()
        {
            SetDebugStackTraces();
            SetPlayFabId();
            SetGraphicSetting();
            
            LoadLanguage();
            LoadSoundMixer();
            LoadMusicMixer();
            //LoadPostProcessing();
            LoadPushAlarms();
            LoadVibration();
        }
        
        public string GetPlayFabTitleId()
        {
            return playFabTitleID;
        }
        
        #region Set Values

        private void SetDebugStackTraces()
        {
            Application.stackTraceLogType = StackTrace;
        }

        private void SetPlayFabId()
        {
            PlayFabSettings.TitleId = playFabTitleID;
        }
        
        private void SetGraphicSetting()
        {
            QualitySettings.vSyncCount = vSyncCount;
            Application.targetFrameRate = targetFrameRate;
        }

        public void SetLanguage(string languageName, bool saveInPrefs = true)
        {
            if (string.IsNullOrEmpty(languageName) || !LocalizationManager.HasLanguage(languageName))
            {
                Debug.Log("LocalizationManager has no language: " + languageName);
                languageName = "English";
            }
            
            LocalizationManager.CurrentLanguage = languageName;

            if (saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.StringKey.Language, languageName);
        }

        private const float _logCoeff = 30;
        public void SetMusicLevel(float value, bool saveInPrefs = true)
        {
            musicLevel = value;
            
            if (musicLevel <= 1) musicLevel = 1;
            
            MusicMixer.SetFloat("Volume", Mathf.Log10(musicLevel * 0.01f) * _logCoeff);
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.FloatKey.MusicFx, value);
        }
        
        public void SetSoundLevel(float value, bool saveInPrefs = true)
        {
            soundLevel = value;
            if (soundLevel <= 1) soundLevel = 1;
            SoundMixer.SetFloat("Volume", Mathf.Log10(soundLevel *  0.01f) * _logCoeff);
            
            if (saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.FloatKey.SoundFx, value);
        }

        private Volume _volume;
        public void SetPostProcessingVolume(Volume volume)
        {
            _volume = volume;
        }
        
        public void SetPostProcessing(int value, bool saveInPrefs = true)
        {
            PostProcessing = value == 0;
            
            _volume.enabled = PostProcessing;
            
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.PostProcessing, value);
        }

        public void SetVibrations(int value, bool saveInPrefs = true)
        {
            VibrationType = (Vibration)value;
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.Vibration, value);
        }

        public void SetPushAlarms(int value, bool saveInPrefs = true)
        {
            NotificationType = (Notifications)value;
            if(saveInPrefs) PreferenceHandler.SetValue(PreferenceHandler.IntKey.Notifications, value);
        }

        private const int minDb = -80;
        public bool disableAudio;
        public void DisableAudioMaster(bool state)
        {
            disableAudio = state;

            if (disableAudio)
            {
                SoundMixer.SetFloat("Volume", minDb);
                MusicMixer.SetFloat("Volume", minDb);
            }
            else
            {
                LoadSoundMixer();
                LoadMusicMixer();
            }
        }

        #endregion

        #region Load Values
        public void LoadLanguage()
        {
            var language = PreferenceHandler.GetValue(PreferenceHandler.StringKey.Language);
            
            if (string.IsNullOrEmpty(language))
            {
                Debug.Log("Curr device language: " + LocalizationManager.GetCurrentDeviceLanguage());
                language = LocalizationManager.GetCurrentDeviceLanguage();
                SetLanguage(language, true);
                return;
            }
            
            SetLanguage(language, false);
        }

        public void LoadSoundMixer()
        {
            var sound = PreferenceHandler.GetValue(PreferenceHandler.FloatKey.SoundFx);
            if (sound == 0) sound = 80;
            SetSoundLevel(sound, false);
        }

        public void LoadMusicMixer()
        {
            var music = PreferenceHandler.GetValue(PreferenceHandler.FloatKey.MusicFx);
            if (music == 0) music = 60;
            SetMusicLevel(music, false);
        }

        public void LoadPostProcessing()
        {
            var postProcessingValue = PreferenceHandler.GetValue(PreferenceHandler.IntKey.PostProcessing);
            SetPostProcessing(postProcessingValue, false);
        }

        public void LoadPushAlarms()
        {
            var pushAlarms = PreferenceHandler.GetValue(PreferenceHandler.IntKey.Notifications);
            SetPushAlarms(pushAlarms, false);
        }
        
        public void LoadVibration()
        {
            var vibration = PreferenceHandler.GetValue(PreferenceHandler.IntKey.Vibration);
            SetVibrations(vibration, false);
        }

        #endregion

        [ContextMenu("Clear Prefs")]
        public void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
