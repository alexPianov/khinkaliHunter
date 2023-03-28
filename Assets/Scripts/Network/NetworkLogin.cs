using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Playstel;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Network
{
    public class NetworkLogin : MonoBehaviour
    {
        public LoginMethod CurrentLoginMethod = LoginMethod.Null;
        public LoginStatus CurrentLoginStatus = LoginStatus.Null;
        
        [Inject] private CacheUserSettings CacheUserSettings;
        [Inject] private NetworkPayload networkPayload;
        [Inject] private HandlerMessages _handlerMessages;
        
        public enum LoginMethod
        {
            Null, DeviceId, Credentials, ChangeAccount
        }
        
        public enum LoginStatus
        {
            Null, Failed
        }

        public async UniTask<LoginResult> LoginByEmail(LoginWithEmailAddressRequest request)
        {
            CurrentLoginMethod = LoginMethod.Credentials;
            CurrentLoginStatus = LoginStatus.Null;

            request.InfoRequestParameters = networkPayload.PayloadAll();

            LoginResult loginResult = null;

            PlayFabClientAPI.LoginWithEmailAddress(request,
                result => loginResult = result, OnLoginError);

            await UniTask.WaitUntil(() 
                => loginResult != null || CurrentLoginStatus == LoginStatus.Failed);

            if (loginResult != null)
            {
                networkPayload.SetPayload(loginResult.InfoResultPayload);
            }
            
            CurrentLoginStatus = LoginStatus.Null;

            return loginResult;
        }
        
        public async UniTask<LoginResult> LoginByDeviceId()
        { 
            CurrentLoginMethod = LoginMethod.DeviceId;
            CurrentLoginStatus = LoginStatus.Null;
            
            LoginResult loginResult = null;
            
#if UNITY_STANDALONE_OSX
            
            Debug.Log("Login by Device Id (OsX Id): " + ReturnDeviceID());

            var requestIOS = new LoginWithIOSDeviceIDRequest
            {
                TitleId = CacheUserSettings.GetPlayFabTitleId(),
                DeviceId = ReturnDeviceID(),
                InfoRequestParameters = PlayFabHandler.PayloadAll(),
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, 
                result => loginResult = result, OnLoginError);
#endif
            
#if UNITY_ANDROID

            Debug.Log("Login by Device Id (Android): " + ReturnDeviceID());

            var requestAndroid = new LoginWithAndroidDeviceIDRequest
            {
                TitleId = CacheUserSettings.GetPlayFabTitleId(),
                AndroidDeviceId = ReturnDeviceID(),
                InfoRequestParameters = PlayFabHandler.PayloadAll(),
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid,
                result => loginResult = result, OnLoginError);
#endif

#if UNITY_IOS
            
            Debug.Log("Login by Device Id (Ios Id): " + ReturnDeviceID());

            var requestIOS = new LoginWithIOSDeviceIDRequest
            {
                TitleId = CacheUserSettings.GetPlayFabTitleId(),
                DeviceId = ReturnDeviceID(),
                InfoRequestParameters = PlayFabHandler.PayloadAll(),
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, 
                result => loginResult = result, OnLoginError);
#endif
            await UniTask.WaitUntil(() 
                => loginResult != null || CurrentLoginStatus == LoginStatus.Failed);
            
            CurrentLoginStatus = LoginStatus.Null;
            
            if (loginResult != null)
            {
                networkPayload.SetPayload(loginResult.InfoResultPayload);
            }
            
            return loginResult;
        }
        
        private static string ReturnDeviceID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        private void OnLoginError(PlayFabError obj)
        {
            Debug.Log("OnLoginError: " + obj.Error);

            CurrentLoginStatus = LoginStatus.Failed;
            
            _handlerMessages.ShowError(obj.Error);

            if (obj.Error == PlayFabErrorCode.ServiceUnavailable 
                || obj.Error == PlayFabErrorCode.ConnectionError
                || obj.Error == PlayFabErrorCode.Unknown)
            {
                return;
            }

            if (CurrentLoginMethod == LoginMethod.Credentials)
            {
                if (obj.Error == PlayFabErrorCode.AccountNotFound
                    || obj.Error == PlayFabErrorCode.AccountBanned
                    || obj.Error == PlayFabErrorCode.AccountDeleted
                    || obj.Error == PlayFabErrorCode.InvalidAccount)
                {
                    PlayerPrefs.DeleteKey(PreferenceHandler.StringKey.Email.ToString());
                    PlayerPrefs.DeleteKey(PreferenceHandler.StringKey.Password.ToString());
                }
            }
        }
    }
}