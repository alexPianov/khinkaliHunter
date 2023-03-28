using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiTime : MonoBehaviour
    {
        public Button buttonSlowMo;
        public Image imageSlowMo;
        public Sprite[] spriteSlowMo;
        public Image sliderSlowMo;
        public GameObject panelEffect;

        [SerializeField] private UiButtons Buttons;
        
        private float maxValue = 1f;
        private float value;
        
        [Inject] private EffectPostFX PostProcess;
        [Inject] private EffectSound Sound;
        [Inject] private EffectMusic Music;
        
        private void Start()
        {
            value = maxValue;
        }

        public void Enable()
        {
            if(Buttons.IsPaused) return;
            if(value < maxValue) return;

            SlowMotion(true);
            imageSlowMo.sprite = spriteSlowMo[0];
            PostProcess.TimeVignette(true);
            Sound.PlaySound(Sound.sounds.ClipTimeSlow);
            Music.SetMuffle(true);
            panelEffect.SetActive(true);
        }

        public void Disable()
        {
            if(Buttons.IsPaused) return;
            
            SlowMotion(false);
            imageSlowMo.sprite = spriteSlowMo[1];
            PostProcess.TimeVignette(false);
            buttonSlowMo.interactable = value == maxValue;
            
            Sound.PlaySound(Sound.sounds.ClipTimeNormalize);
            Music.SetMuffle(false);
            panelEffect.SetActive(false);
        }
        
        private Mode mode;
        public enum Mode
        {
            Fix, Slow, Normal
        }
        public void SlowMotion(bool state)
        {
            if (state)
            {
                mode = Mode.Slow;
            }
            else
            {
                mode = Mode.Normal;
            }
        }

        private float minTimeScale = 0.55f;
        private float maxTimeScale = 1f;
        private float decreaseSpeed = 1.2f;
        private float increaseSpeed = 0.09f;
        
        private void FixedUpdate()
        {
            if (mode == Mode.Slow)
            {
                Time.timeScale -= Time.deltaTime * 4;
                
                if (Time.timeScale <= minTimeScale)
                {
                    Time.timeScale = minTimeScale;
                    mode = Mode.Fix;
                }
            }
            
            if (mode == Mode.Normal)
            {
                Time.timeScale += Time.deltaTime;
                
                if (Time.timeScale >= maxTimeScale)
                {
                    Time.timeScale = maxTimeScale;
                    mode = Mode.Fix;
                }
            }

            if (mode == Mode.Fix)
            {
                if (Time.timeScale >= maxTimeScale)
                {
                    value += Time.deltaTime * increaseSpeed;
                }

                if (Time.timeScale <= minTimeScale)
                {
                    value -= Time.deltaTime * decreaseSpeed;
                }
                
                if (value < 0)
                {
                    value = 0;
                    Disable();
                }
            
                if (value > maxValue)
                {
                    value = maxValue;
                    buttonSlowMo.interactable = true;
                }

                sliderSlowMo.fillAmount = value;
            }
        }
    }
}