using System;
using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Project.Scripts.Addressable;
using Project.Scripts.Scriptable_Store;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Playstel
{
    public class UiDistance : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private AssetReference godHinkaliReference;

        [Header("Refs")] 
        public UiInfo info;
        
        [Header("Text")]
        [SerializeField] private TMP_Text textDistance;
        [SerializeField] private TMP_Text textBonus;
        
        [Inject] private Player player;
        [Inject] private EffectSound sounds;
        [Inject] private AddressableHandler addressableHandler;
        [Inject] private HandlerVibration handlerVibration;
        
        private ObscuredInt _distanceGoalProgress;
        private ObscuredInt _distanceGoalStep = 500;
        private ObscuredInt _distanceGoalBonus = 5;
        private int _delay = 200;

        private void Start()
        {
            _distanceGoalProgress = _distanceGoalStep;
        }

        private void Update()
        {
            int distance = GetDistance();
            textDistance.text = distance + " m";

            if (distance > _distanceGoalProgress)
            {
                Bonus();
                _distanceGoalProgress += _distanceGoalStep;
            }
        }
    
        private async void Bonus()
        {
            info.ShowInfo(UiInfo.InfoType.BaliGodBonus, _distanceGoalBonus, UiInfo.InfoFormat.Bonus);
            info.ActiveCharacter(UiCharacter.Type.God, true);
            sounds.PlaySound(sounds.sounds.ClipGodAppear);
            handlerVibration.DistanceBonus();
            
            var god = info.GetCharacter(UiCharacter.Type.God);

            var bonusItems = _distanceGoalBonus;
            for (var i = 0; i < _distanceGoalBonus; i++)
            {
                sounds.PlaySound(sounds.sounds.ClipThrow);
                var item = await addressableHandler
                    .CreateGameObject(godHinkaliReference);

                item.transform.position = god.position;
                
                //Instantiate(prefabGodKhinkali, god.position, Quaternion.identity, null);
                textBonus.text = bonusItems.ToString();
                await UniTask.Delay(_delay);
                textBonus.text = (bonusItems--).ToString();
            }
        
            await UniTask.Delay(_delay);
        
            info.ActiveCharacter(UiCharacter.Type.God, false);
        }

        public int GetDistance()
        {
            return Mathf.FloorToInt(player.distance);
        }

    }
}