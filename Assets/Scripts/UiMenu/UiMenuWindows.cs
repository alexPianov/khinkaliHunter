using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playstel
{
    public class UiMenuWindows : MonoBehaviour
    {
        public List<UiMenuWindow> menuWindows = new();
        public bool closeAllOnAwake;
        public enum Windows
        {
            None, Settings, Leaders, Crate, Menu, Login, Registration, ForgotPassword, Loading
        }

        private void Awake()
        {
            if(closeAllOnAwake) Active(Windows.None);
        }

        public void Active(Windows windowStatus)
        {
            foreach (var menuWindow in menuWindows)
            {
                menuWindow.Active(false);
            }
            
            if (windowStatus == Windows.None)
            {
                return;
            }

            var window = menuWindows.Find(menuWindow => menuWindow.Window == windowStatus);
            
            if(window) window.Active(true);
        }
    }
}