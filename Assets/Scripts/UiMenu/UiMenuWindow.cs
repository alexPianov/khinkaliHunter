using System;
using UnityEngine;

namespace Playstel
{
    public class UiMenuWindow : MonoBehaviour
    {
        public UiMenuWindows.Windows Window;
        
        public void Active(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}