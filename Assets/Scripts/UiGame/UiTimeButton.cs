using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Playstel
{
    public class UiTimeButton: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UiTime UiTime;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                UiTime.Enable();
            }
            
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                UiTime.Disable();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UiTime.Enable();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UiTime.Disable();
        } 
    }
}