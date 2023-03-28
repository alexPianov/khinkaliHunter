using System;
using System.Collections.Generic;
using Project.Scripts.Network;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiMenuButtons : MonoBehaviour
    {
        [SerializeField]
        private List<UiMenuButton> menuButtons = new();
        public enum MenuButton
        {
            Registration, Leaders, Market
        }

        [Inject] private NetworkPayload _networkPayload;

        private void Start()
        {
            var userName = _networkPayload.GetUsername();

            if (string.IsNullOrEmpty(userName))
            {
                ActiveButton(MenuButton.Registration, true);
                ActiveButton(MenuButton.Leaders, false);
            }
            else
            {
                ActiveButton(MenuButton.Registration, false);
                ActiveButton(MenuButton.Leaders, true);
            }
            
            ActiveButton(MenuButton.Market, true); 
        }

        public void ActiveButton(MenuButton button, bool state)
        {
            menuButtons.Find(menuButton => menuButton.currentButton == button)
                .gameObject.SetActive(state);
        }
    }
}