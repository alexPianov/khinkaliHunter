using I2.Loc;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Localization
{
    public class Localization : MonoBehaviour
    {
        public TMP_Dropdown inputLanguage;

        private void Start()
        {
            inputLanguage.onValueChanged.AddListener(SetLanguage);
        }

        private void SetLanguage(int value)
        {
            //var languageName = (Language)value;
            //SetLanguage(languageName.ToString()); 
        }

        private void SetLanguage(string language)
        {
            if(LocalizationManager.HasLanguage(language))
            { 
                Debug.Log("Set language: " + language);
                LocalizationManager.CurrentLanguage = language;
            }
        }
    }
}