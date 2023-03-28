using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using I2.Loc;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiInfo : MonoBehaviour
    {
        [Header("Annotation")]
        [SerializeField] private TMP_Text textAnnotation;
        [SerializeField] private UiTransparency annotationTransparency; 

        [Header("Resources")] 
        [SerializeField] private List<UiResource> resources;
        [SerializeField] private GameObject buttonTimeStone;
        
        [Header("Characters")] 
        [SerializeField] private List<UiCharacter> characters;

        private int _annotationTime = 2000;
        private bool _annotationIsShowing;
        
        public async UniTask ShowInfo(InfoType infoType, int formatValue = 0, InfoFormat formatKey = InfoFormat.None, bool Instant = false)
        {
            if (infoType == InfoType.None)
            {
                annotationTransparency.Transparency(true);
                _annotationIsShowing = false;
                return;
            }
            
            textAnnotation.text = null;

            var term = GetLocalizationId(infoType);
            textAnnotation.GetComponent<Localize>().SetTerm(term);

            if (formatKey == InfoFormat.Bonus)
            {
                textAnnotation.GetComponent<LocalizationParamsManager>()
                    .SetParameterValue("bonus_score", formatValue.ToString());
            }

            if (formatKey == InfoFormat.Contract)
            {
                textAnnotation.GetComponent<LocalizationParamsManager>()
                    .SetParameterValue("contract_score", formatValue.ToString());
            }

            annotationTransparency.Transparency(false);
        
            if(_annotationIsShowing) return;
            _annotationIsShowing = true;
        
            await Task.Delay(_annotationTime);
        
            annotationTransparency.Transparency(true);
            _annotationIsShowing = false;
        }

        public void ActiveResource(Obstacle.Type type, bool state)
        {
            foreach (var resource in resources)
            {
                if (resource.CurrentType == type)
                {
                    resource.gameObject.SetActive(state);
                }
            }
        }

        public void ActiveTimeStone(bool state)
        {
            buttonTimeStone.SetActive(state);
        }

        public void ActiveCharacter(UiCharacter.Type type, bool state)
        {
            foreach (var character in characters)
            {
                if (character.CurrentType == type)
                {
                    character.Active(state);
                }
            }
        }
        
        public Transform GetCharacter(UiCharacter.Type type)
        {
            foreach (var character in characters)
            {
                if (character.CurrentType == type)
                {
                    return character.transform;
                }
            }

            return null;
        }

        public enum InfoFormat
        {
            None, Bonus, Contract
        }

        public enum InfoType
        {
            None, BaliGodBonus, BaliEnter, BaliExit, SatanEnter,SatanExit, SpikesDestroy, 
            PotionSpikesDefenceOn,PotionSpikesDefenceOff, 
            PotionDoubleJumpOn,PotionDoubleJumpOff,
            PotionDietKhinkaliOn, PotionDietKhinkaliOff,
            PauseOn
        }
        
        private string GetLocalizationId(InfoType type)
        {
            switch (type)
            {
                case InfoType.BaliGodBonus:
                    return "Info-Bali-God-Bonus";
                case InfoType.BaliEnter:
                    return "Info-Bali-Enter";
                case InfoType.BaliExit:
                    return "Info-Bali-Exit";
                case InfoType.SatanEnter:
                    return "Info-Devil-Enter";
                case InfoType.SatanExit:
                    return "Info-Devil-Exit";
                case InfoType.SpikesDestroy:
                    return "Info-Spikes-Destroy";
                case InfoType.PotionSpikesDefenceOn:
                    return "Info-Potion-SpikesDefence-On";
                case InfoType.PotionSpikesDefenceOff:
                    return "Info-Potion-SpikesDefence-Off";
                case InfoType.PotionDoubleJumpOn:
                    return "Info-Potion-DoubleJump-On";
                case InfoType.PotionDoubleJumpOff:
                    return "Info-Potion-DoubleJump-Off";
                case InfoType.PotionDietKhinkaliOn:
                    return "Info-Potion-DietKhinkali-On";
                case InfoType.PotionDietKhinkaliOff:
                    return "Info-Potion-DietKhinkali-Off";
                case InfoType.PauseOn:
                    return "Info-Pause-On";
                default:
                    return "";
            }
        }
    }
}