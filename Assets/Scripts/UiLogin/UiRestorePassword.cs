using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Playstel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.Enter
{
    public class UiRestorePassword : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private TMP_InputField inputEmail;
        
        [Header("Button")]
        [SerializeField] private Button buttonRestore;

        [Inject] private HandlerMessages _handlerMessages;
        [Inject] private CacheUserSettings _cacheUserSettings;
        
        [Header("Refs")] 
        [SerializeField] private UiMenuWindows Windows;
        
        private void Start()
        {
            inputEmail.onValueChanged.AddListener(InputEmail);
            buttonRestore.onClick.AddListener(Restore);
            buttonRestore.interactable = false;
        }

        private string valueEmail = "";
        private void InputEmail(string email)
        {
            valueEmail = email;
            CheckButton();
        }
        
        private int maxLenght = 24;
        private void CheckButton()
        {
            buttonRestore.interactable = valueEmail.Length > 0 && valueEmail.Length <= maxLenght;
        }

        private async void Restore()
        {
            buttonRestore.interactable = false;
            
            SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest();

            request.Email = valueEmail;
            request.TitleId = _cacheUserSettings.GetPlayFabTitleId();

            var result = await RecoveryEmail(request);

            buttonRestore.interactable = true;
            
            if (result != null)
            {
                _handlerMessages.ShowMessage("Restore message was sent to your email");
                Windows.Active(UiMenuWindows.Windows.Login);
            }
        }
        
        private static bool getRegistrationSuccess;
        public async UniTask<SendAccountRecoveryEmailResult> RecoveryEmail
            (SendAccountRecoveryEmailRequest request)
        {
            getRegistrationSuccess = false;
            SendAccountRecoveryEmailResult operationResult = null;

            PlayFabClientAPI.SendAccountRecoveryEmail(request, 
                result =>
                {
                    operationResult = result;
                    getRegistrationSuccess = true;
                }, OnLoginError);
            
            await UniTask.WaitUntil(() => getRegistrationSuccess);
            return operationResult;
        }

        private void OnLoginError(PlayFabError error)
        {
            getRegistrationSuccess = true;
            _handlerMessages.ShowError(error.Error);
        }
    }
}