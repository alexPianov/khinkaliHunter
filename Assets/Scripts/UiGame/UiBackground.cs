using System;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class UiBackground : MonoBehaviour
    {
        [Header("Background")] 
        [SerializeField] private RawImage background;
        [SerializeField] private Texture spriteBali;
        [SerializeField] private GameObject backgroundWater;
        private Texture spriteStandart;

        private void Awake()
        {
            spriteStandart = background.texture;
        }

        public void BackgroundBali(bool state)
        {
            backgroundWater.SetActive(state);
        
            if (state)
            {
                background.texture = spriteBali;
            }
            else
            {
                background.texture = spriteStandart;
            }
        }
    }
}