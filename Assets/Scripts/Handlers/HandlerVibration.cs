using MoreMountains.NiceVibrations;
using UnityEngine;
using Zenject;
using static Playstel.CacheUserSettings;

namespace Playstel
{
    public class HandlerVibration : MonoBehaviour
    {
        [Inject] private CacheUserSettings _cacheUserSettings;

        public void GrabScore()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return;
            //MMVibrationManager.Haptic(HapticTypes.Selection, false, true, this);
        }

        public void GrabPotion()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return; 
            MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
            //MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
        }

        public void GrabCharacter()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return; 
            MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
            //MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
        }
        
        public void GroundFall()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return; 
            //MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
        }

        public void DistanceBonus()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return; 
            //MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
        }

        public void Button()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return; 
            //MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
        }

        public void Death()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return; 
            //MMVibrationManager.Haptic(HapticTypes.Warning, false, true, this);
        }

        public void Spikes()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return; 
            MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
            //MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
        }

        public void Jump()
        {
            if (_cacheUserSettings.VibrationType == Vibration.Disabled) return; 
            //MMVibrationManager.Haptic(HapticTypes.LightImpact, false, true, this);
        }
    }
}