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
    public class UiLogin : MonoBehaviour
    {
        [Header("Input")] 
        [SerializeField] private TMP_InputField inputEmail;
        [SerializeField] private TMP_InputField inputPassword;

        [Header("Buttons")] 
        [SerializeField] private Button buttonEnter;
        [SerializeField] private Button buttonEnterFacebook;
        [SerializeField] private Button buttonEnterGoogle;
        [SerializeField] private Button buttonRegistration;
        [SerializeField] private Button buttonForgotPassword;

        [Header("Refs")] 
        [SerializeField] private UiMenuWindows Windows;

        [Inject] private CacheUserSettings CacheUserSettings;

        [Inject] private NetworkLogin _networkLogin;

        private void Start()
        {
            inputEmail.onValueChanged.AddListener(InputEmail);
            inputPassword.onValueChanged.AddListener(InputPassword);
            
            buttonEnter.onClick.AddListener(EnterByEmail);
            buttonEnterFacebook.onClick.AddListener(EnterFacebook);
            buttonEnterGoogle.onClick.AddListener(EnterGoogle);
            buttonRegistration.onClick.AddListener(Registration);
            buttonForgotPassword.onClick.AddListener(ForgotPassword);
            buttonEnter.interactable = false;

            CheckPrefsLogin();
        }

        private void CheckPrefsLogin()
        {
            valueEmail = PreferenceHandler.GetValue(PreferenceHandler.StringKey.Email);
            valuePassword = PreferenceHandler.GetValue(PreferenceHandler.StringKey.Password);

            if (_networkLogin.CurrentLoginMethod == NetworkLogin.LoginMethod.ChangeAccount)
            {
                Login();
                return;
            }
            
            if(CheckAccess())
            {
                EnterByEmail(); return;
            }
            
            EnterByDeviceId();
        }

        private string valueEmail;
        private string valuePassword;
        private void InputEmail(string value)
        {
            valueEmail = value;
            CheckAccess();
        }

        private void InputPassword(string value)
        {
            valuePassword = value;
            CheckAccess();
        }
        
        private int maxLenght = 24;
        private bool CheckAccess()
        {
            var access = valueEmail.Length > 0 && valueEmail.Length <= maxLenght &&
                                              valuePassword.Length > 0 && valuePassword.Length <= maxLenght;

            buttonEnter.interactable = access;
            return access;
        }

        private async void EnterByEmail()
        {
            buttonEnter.interactable = false;
            var request = new LoginWithEmailAddressRequest();
            request.Email = valueEmail;
            request.Password = valuePassword;
            request.TitleId = CacheUserSettings.GetPlayFabTitleId();
            var result = await _networkLogin.LoginByEmail(request);
            buttonEnter.interactable = true;
            if (result == null) 
            { 
                Login(); 
                Debug.Log("Login error"); 
                return; 
            }
            SceneManager.LoadScene(1);
        }

        private async void EnterByDeviceId()
        {
            var result = await _networkLogin.LoginByDeviceId();
            if (result == null)
            {
                Login();
                Debug.Log("Login error"); 
                return;
            }
            
            SceneManager.LoadScene(1);
        }

        private void EnterFacebook()
        {

        }

        private void EnterGoogle()
        {

        }

        public void Registration()
        {
            Debug.Log("Registration");
            Windows.Active(UiMenuWindows.Windows.Registration);
        }
        
        public void Login()
        {
            Debug.Log("Login");
            Windows.Active(UiMenuWindows.Windows.Login);
        }

        public void ForgotPassword()
        {
            Debug.Log("ForgotPassword");
            Windows.Active(UiMenuWindows.Windows.ForgotPassword);
        }
    }
}