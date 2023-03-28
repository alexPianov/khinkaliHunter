using I2.Loc;
using Lean.Gui;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Playstel
{
    public class HandlerMessages : MonoBehaviour
    {
        public LeanPulse leanPulse;
        public TMP_Text textMessage;
        public UiClickEffect ClickEffect;

        public void ShowMessage(string text)
        {
            ClickEffect.ClickEffect();
            textMessage.text = text;
            SetPulse();
        }

        public void ShowError(PlayFabErrorCode code)
        {
            ClickEffect.ClickEffect();
            Debug.Log("Error code: " + code);
            textMessage.text = code.ToString();
            //textMessage.GetComponent<Localize>().SetTerm("Error-" + code);
            SetPulse();
        }
        
        public void ShowError(PurchaseFailureReason code)
        {
            ClickEffect.ClickEffect();
            Debug.Log("Error code: " + code);
            textMessage.text = code.ToString();
            //textMessage.GetComponent<Localize>().SetTerm("Error-" + code);
            SetPulse();
        }


        private void SetPulse()
        {
            leanPulse.RemainingPulses = 1;
            leanPulse.RemainingTime = 1.2f;
            leanPulse.Pulse();
        }
    }
}