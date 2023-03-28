using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Lean.Gui;
using Newtonsoft.Json;
using Project.Scripts.Network;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Purchasing;
using static Project.Scripts.Network.NetworkUserData.UserDataType;

namespace Playstel
{
    public class UiChest : MonoBehaviour
    {
        [SerializeField] private List<UiChestItem> items = new();
        [SerializeField] private Button buttonOpen;
        [SerializeField] private Button buttonClose;
        [SerializeField] private UiMenuMain MenuMain;

        [Inject] private NetworkUserData _user;
        [Inject] private HandlerMessages _handlerMessages;

        private void Start()
        {
            buttonOpen.onClick.AddListener(StartPurchase);
            buttonClose.onClick.AddListener(SaveChest);
            UpdateChest();
        }

        public void UpdateChest()
        {
            var open = _user.GetUserData(ChestIsOpen);
            Debug.Log("ChestIsOpened: " + open);
            buttonOpen.gameObject.SetActive(!open);

            foreach (var item in items)
            {
                item.ActiveToggle(open);
                
                if (open)
                {
                    var state = _user.GetUserData(item.GetItem());
                    item.SetState(state);
                }
            }
        }

        private async void SaveChest()
        {
            var open = _user.GetUserData(ChestIsOpen);
            
            if (open)
            {
                MenuMain.OpenLoading();
                
                var chestItems = new NetworkUserData.ChestItems();
                
                chestItems.AdBlindness = GetState(AdBlindness);
                chestItems.DietKhinkali = GetState(DietKhinkali);;
                chestItems.DoubleJump = GetState(DoubleJump);;
                chestItems.SpikeProtection = GetState(SpikeProtection);;
                chestItems.TimeStone = GetState(TimeStone);;
                await _user.UpdateChestItems(chestItems);
            }
            
            MenuMain.CloseWindow();
        }

        private bool GetState(NetworkUserData.UserDataType userDataType)
        {
            return items.Find(item => item.GetItem() == userDataType).GetState();
        }

        #region Purchase
        
        private ObscuredString chestDefinitionId = "com.playstel.khinkaliHunter.openChest";

        private void StartPurchase()
        {
            buttonOpen.interactable = false;
        }

        public async void OnPurchaseComplete(Product product)
        {
            Debug.Log("OnPurchaseComplete | storeSpecificId: " + 
                      product.definition.storeSpecificId);
            
            if (product.definition.id == chestDefinitionId)
            {
                await _user.OpenChest();
                UpdateChest();
                
                _handlerMessages.ShowMessage("Chest was opened!");
            
                buttonOpen.interactable = true;
            }
            else
            {
                OnPurchaseFailed(product, PurchaseFailureReason.ProductUnavailable);
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.Log("OnPurchaseFailed");
            
            _handlerMessages.ShowError(reason);
            
            buttonOpen.interactable = true;
        }

        #endregion
    }
}