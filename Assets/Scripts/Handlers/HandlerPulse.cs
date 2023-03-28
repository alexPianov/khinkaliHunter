using System;
//using Lean.Gui;
using PlayFab;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class HandlerPulse : MonoBehaviour
    {
        //public LeanPulse leanPulse;
        public TextMeshProUGUI pulseMessage;

        public void OpenTextNote(string text)
        {
            Debug.Log("Pulse Text: " + text);
            
            SetPulse();
        }

        private void SetPulse()
        {
            // leanPulse.RemainingPulses = 1;
            // leanPulse.RemainingTime = 1.2f;
            // leanPulse.Pulse();
        }
    }
}
