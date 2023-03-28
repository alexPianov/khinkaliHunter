using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Playstel
{
    public class EffectPostFX : MonoBehaviour
    {
        public EditValues postVolume;
        
        private bool activeTimeVignette;
        private bool activeAbberation;
        
        public void TimeVignette(bool state)
        {
            activeTimeVignette = state;
        }

        public void ActiveAbberation(bool state)
        {
            activeAbberation = state;
        }
        
        private void FixedUpdate()
        {
            Vignette();
            Abberation();
        }

        private void Abberation()
        {
            if (activeAbberation)
            {
                postVolume.Offset = Mathf.PingPong(Time.time * 10f,1f);
                
                if (postVolume.Offset >= 1)
                {
                    ActiveAbberation(false);
                }
            }
            else
            {
                if (postVolume.Offset <= 0.1)
                {
                    return;
                }
                
                postVolume.Offset = Mathf.PingPong(Time.time * 10f, 0.1f);
            }
        }
        
        private void Vignette()
        {
            if (activeTimeVignette)
            {
                if (postVolume.VignetteAmount >= 0.12f)
                {
                    postVolume.VignetteAmount = 0.12f;
                    return;
                }

                postVolume.VignetteAmount += Time.deltaTime;
            }
            else
            {
                if (postVolume.VignetteAmount <= 0.005)
                {
                    return;
                }

                postVolume.VignetteAmount -= Time.deltaTime;
            }
        }
    }
}