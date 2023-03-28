using CodeStage.AntiCheat.ObscuredTypes;
using Lean.Gui;
using Project.Scripts.Network;
using UnityEngine;

namespace Playstel
{
    public class UiChestItem : MonoBehaviour
    {
        [SerializeField] private LeanToggle toggleState;
        [SerializeField] private NetworkUserData.UserDataType itemType;

        public NetworkUserData.UserDataType GetItem()
        {
            return itemType;
        }

        public ObscuredBool GetState()
        {
            return toggleState.On;
        }

        public void SetState(bool state)
        {
            toggleState.On = state;
        }
        
        public void ActiveToggle(bool state)
        {
            toggleState.gameObject.SetActive(state);
        }
    }
}