using System;
using System.Collections.Generic;
using Project.Scripts.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Playstel
{
    public class PlayerFX : MonoBehaviour
    {
        [Header("Conatiner")]
        public Transform effectContainer;
        public ObjectPool Pool;

        [Header("Assets")]
        public AssetReference effectSpawn;
        public AssetReference effectContract;
        public AssetReference effectEat;
        public AssetReference effectPortal;
        public AssetReference effectDamage;
        public AssetReference effectSpeed;
        public AssetReference effectDefence;
        public AssetReference effectJump;

        [Header("Refs")] 
        [Inject] private EffectPostFX PostProcess;

        [Inject] private AddressableHandler _addressableHandler;
        
        public enum EffectType
        {
            Spawn, Contract, Eat, Portal, Damage, Speed, Defence, Jump, JumpDust
        }

        public void ActiveEffect(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.Spawn:
                    PostProcess.ActiveAbberation(true);
                    _addressableHandler.CreateGameObject(effectSpawn, effectContainer);
                    break;
                case EffectType.Contract:
                    PostProcess.ActiveAbberation(true);
                    _addressableHandler.CreateGameObject(effectContract, effectContainer);
                    break;
                case EffectType.Eat:
                    _addressableHandler.CreateGameObject(effectEat, effectContainer);
                    break;
                case EffectType.Portal:
                    PostProcess.ActiveAbberation(true);
                    _addressableHandler.CreateGameObject(effectPortal, effectContainer);
                    break;
                case EffectType.Damage:
                    PostProcess.ActiveAbberation(true);
                    _addressableHandler.CreateGameObject(effectDamage, effectContainer);
                    break;
                case EffectType.Speed:
                    PostProcess.ActiveAbberation(true);
                    _addressableHandler.CreateGameObject(effectSpeed, effectContainer);
                    break;
                case EffectType.Defence:
                    PostProcess.ActiveAbberation(true);
                    _addressableHandler.CreateGameObject(effectDefence, effectContainer);
                    break;
                case EffectType.Jump:
                    PostProcess.ActiveAbberation(true);
                    _addressableHandler.CreateGameObject(effectJump, effectContainer);
                    break;
                case EffectType.JumpDust:
                    var obj = Pool.GetFromPool(ObjectPool.Pool.ObjectType.JumpDust, null);
                    obj.transform.position = effectContainer.position + Vector3.up;
                    break;
                default:
                    break;
            }
        }
    }
}