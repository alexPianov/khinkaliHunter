using System;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Playstel;
using Project.Scripts.Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.Enter
{
    public class UiRegistration : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private TMP_InputField inputNickname;
        [SerializeField] private TMP_InputField inputEmail;
        [SerializeField] private TMP_InputField inputPassword;
        
        [Header("Button")]
        [SerializeField] private Button buttonRegistration;
        
        [Header("Toggle")]
        [SerializeField] private Toggle toggleTerms;
        [SerializeField] private Button buttonOpenTerms;

        [Inject] private CacheUserSettings CacheUserSettings;
        [Inject] private HandlerMessages _handlerMessages;
        [Inject] private NetworkLogin _networkLogin;

        private void Start()
        {
            inputNickname.onValueChanged.AddListener(InputNickname);
            inputEmail.onValueChanged.AddListener(InputEmail);
            inputPassword.onValueChanged.AddListener(InputPassword);
            buttonRegistration.onClick.AddListener(ButtonRegistration);
            toggleTerms.onValueChanged.AddListener(ToggleTerms);
            buttonOpenTerms.onClick.AddListener(OpenTerms);
            
            buttonRegistration.interactable = false;
            toggleTerms.isOn = false;
        }
        
        [Header("Data")]
        public string valueNickname = "";
        public string valueEmail = "";
        public string valuePassword = "";
        public bool terms = false;
        private void InputNickname(string value)
        {
            valueNickname = value;
            CheckButton();
        }
        
        private void InputEmail(string value)
        {
            valueEmail = value;
            CheckButton();
        }

        private void InputPassword(string value)
        {
            valuePassword = value;
            CheckButton();
        }

        private void ToggleTerms(bool state)
        {
            terms = state;
            CheckButton();
        }

        private int maxLenght = 24;
        private void CheckButton()
        {
            buttonRegistration.interactable = valueNickname.Length > 0 && valueNickname.Length <= maxLenght &&
                                              valueEmail.Length > 0 && valueEmail.Length <= maxLenght &&
                                              valuePassword.Length > 0 && valuePassword.Length <= maxLenght &&
                                              terms;
        }

        private async void ButtonRegistration()
        {
            buttonRegistration.interactable = false;
            var request = new RegisterPlayFabUserRequest
            {
                Username = valueNickname,
                Password = valuePassword,
                Email = valueEmail,
                TitleId = CacheUserSettings.GetPlayFabTitleId()
            };
            
            var result = await RegistrationRequest(request);
            if (result == null) 
            { 
                buttonRegistration.interactable = true; 
                return; 
            }

            var nameRequest = new UpdateUserTitleDisplayNameRequest()
            {
                DisplayName = valueNickname
            };
            
            var resultName = await UserTitleDisplayNameRequest(nameRequest);
            
            if (resultName == null) 
            { 
                buttonRegistration.interactable = true; 
                return; 
            }
            
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Email, valueEmail);
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Password, valuePassword);
            PreferenceHandler.SetValue(PreferenceHandler.StringKey.Nickname, valueNickname);
            
            _networkLogin.CurrentLoginMethod = NetworkLogin.LoginMethod.Null;
            
            await UniTask.Delay(200);
            
            buttonRegistration.interactable = true;
            SceneManager.LoadScene(0);
        }
        
        private static bool getRegistrationSuccess;
        public async UniTask<RegisterPlayFabUserResult> RegistrationRequest
            (RegisterPlayFabUserRequest request)
        {
            getRegistrationSuccess = false;
            RegisterPlayFabUserResult operationResult = null;

            PlayFabClientAPI.RegisterPlayFabUser(request, 
                result =>
            {
                operationResult = result;
                getRegistrationSuccess = true;
            }, OnLoginError);
            
            await UniTask.WaitUntil(() => getRegistrationSuccess);
            return operationResult;
        }
        
        private static bool operationFinish;
        public async UniTask<UpdateUserTitleDisplayNameResult> UserTitleDisplayNameRequest
            (UpdateUserTitleDisplayNameRequest request)
        {
            operationFinish = false;
            UpdateUserTitleDisplayNameResult operationResult = null;

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, 
                result =>
                {
                    operationResult = result;
                    operationFinish = true;
                }, OnLoginError);
            
            await UniTask.WaitUntil(() => operationFinish);
            return operationResult;
        }
        
        private void OnLoginError(PlayFabError error)
        {
            getRegistrationSuccess = true;
            _handlerMessages.ShowError(error.Error);
        }
        
        private void OpenTerms()
        {
            Application.OpenURL(KeyStore.REF_TERMS);
        }
    }
}