using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiSettings : MonoBehaviour
    {
        [Header("Cache")]
        [Inject] private CacheUserSettings _cacheUserSettings;
        
        [Header("Slider")]
        [SerializeField] private Slider sliderMusicLevel;
        [SerializeField] private Slider sliderSoundLevel;
        
        [Header("Dropdown")]
        [SerializeField] private TMP_Dropdown dropdownLocalization;
        [SerializeField] private TMP_Dropdown dropdownNotification;
        [SerializeField] private TMP_Dropdown dropdownVibration;

        private void Start()
        {
            LoadValues();
            
            sliderMusicLevel.onValueChanged.AddListener(SetMusic);
            sliderSoundLevel.onValueChanged.AddListener(SetSound);
            
            dropdownLocalization.onValueChanged.AddListener(SetLoc);
            dropdownNotification.onValueChanged.AddListener(SetNotifications);
            dropdownVibration.onValueChanged.AddListener(SetVibration);
        }

        private void LoadValues()
        {
            sliderMusicLevel.value = _cacheUserSettings.musicLevel;
            sliderSoundLevel.value = _cacheUserSettings.soundLevel;

            dropdownLocalization.value = GetDropdownValue(LocalizationManager.CurrentLanguage);
            dropdownNotification.value = (int)_cacheUserSettings.NotificationType;
            dropdownVibration.value = (int)_cacheUserSettings.VibrationType;
        }

        private void SetMusic(float value)
        {
            _cacheUserSettings.SetMusicLevel(value);
        }
        
        private void SetSound(float value)
        {
            _cacheUserSettings.SetSoundLevel(value);
        }
        
        private void SetNotifications(int value)
        {
            _cacheUserSettings.SetPushAlarms(value);
        }
        
        private void SetVibration(int value)
        {
            _cacheUserSettings.SetVibrations(value);
        }
        
        #region Localization
        
        private void SetLoc(int dropdownValue)
        {
            _cacheUserSettings.SetLanguage(GetLanguageName(dropdownValue));
        }

        private string GetLanguageName(int dropdownValue)
        {
            switch (dropdownValue)
            {
                case 0: return "English(en)";
                case 1: return "Russian(ru)";
                default: return null;
            }
        }
        
        private int GetDropdownValue(string languageName)
        {
            Debug.Log("GetDropdownValue: " + languageName);
            switch (languageName)
            {
                case "English(en)": return 0;
                case "Russian(ru)": return 1;
                default: return 0;
            }
        }
        
        #endregion
        
    }
}