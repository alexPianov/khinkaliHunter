using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Project.Scripts.Network;
using UnityEngine;
using Zenject;
using static Project.Scripts.Network.NetworkUserData.UserDataType;

namespace Playstel
{
    public class HandlerAd : HandlerIronSource
    {
        [SerializeField]
        private ObscuredInt _sessions;
        private ObscuredInt _sessionsWithNoAd = 3;
        private ObscuredInt _sessionMinDistance = 450;

        [Inject] private NetworkUserData _userData;
        [Inject] private HandlerMessages _handlerMessages;

        public async UniTask Check(int distance)
        {
            var _adBlindness = _userData.GetUserData(AdBlindness);
            Debug.Log("Ad Blindness: " + _adBlindness);
            if(_adBlindness) return; 
            
            if(distance < _sessionMinDistance) return;

            _sessions++;

            if (_sessions >= _sessionsWithNoAd)
            {
                _sessions = 0;
                
                Debug.Log("ShowAd");
                
                await ShowAd();
            }
        }

        private async UniTask ShowAd()
        {
            var result = await ShowInterstitial();

            if (result == AdStatus.Error)
            {
                //_handlerMessages.ShowMessage("Ad error");
            }
        }
    }
}