using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiClickEffect : MonoBehaviour
    {
        [SerializeField]
        private bool buttonBack;
        
        [Inject] private HandlerVibration vibration;
        [Inject] private EffectSound sound;

        private void Start()
        {
            if (GetComponent<Button>())
            {
                GetComponent<Button>().onClick.AddListener(Button);
                return;
            }
            
            if (GetComponent<TMP_InputField>())
            {
                GetComponent<TMP_InputField>().onValueChanged.AddListener(Input);
                return;
            }
            
            if (GetComponent<TMP_Dropdown>())
            {
                GetComponent<TMP_Dropdown>().onValueChanged.AddListener(Dropdown);
                return;
            }
            
            if (GetComponent<Toggle>())
            {
                GetComponent<Toggle>().onValueChanged.AddListener(Toggle);
                return;
            }
        }

        private void Input(string value)
        {
            ClickEffect();
        }

        private void Dropdown(int value)
        {
            ClickEffect();
        }

        private void Toggle(bool value)
        {
            ClickEffect();
        }

        private void Button()
        {
            ClickEffect();
        }

        public void ClickEffect()
        {
            if (buttonBack)
            {
                sound.PlaySound(sound.sounds.ClipClickButtonBack);
            }
            else
            {
                sound.PlaySound(sound.sounds.ClipClickButton);
            }
            
            vibration.Button();
        }
    }
}