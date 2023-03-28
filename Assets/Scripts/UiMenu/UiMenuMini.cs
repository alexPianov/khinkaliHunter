using System;
using Project.Scripts.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class UiMenuMini : MonoBehaviour
    {
        [Header("Refs")] [SerializeField] private UiMenuWindows Windows;
        
        [Header("Buttons")]
        [SerializeField] private Button buttonMenu;
        [SerializeField] private Button buttonSettings; 
        [SerializeField] private Button buttonReview;
        [SerializeField] private Button buttonPolicy;
        [SerializeField] private Button buttonDeveloper;
        [SerializeField] private Button buttonLogin;

        [Inject] private NetworkLogin _networkLogin;

        private void Start()
        {
            buttonMenu.onClick.AddListener(OpenMenu);
            buttonSettings.onClick.AddListener(OpenSettings); 
            buttonReview.onClick.AddListener(OpenReview);
            buttonPolicy.onClick.AddListener(OpenPolicy);
            buttonDeveloper.onClick.AddListener(OpenDeveloper);
            buttonLogin.onClick.AddListener(OpenLogin);
        }

        public void OpenMenu()
        {
            Windows.Active(UiMenuWindows.Windows.Menu);
        }

        public void OpenSettings()
        {
            Windows.Active(UiMenuWindows.Windows.Settings);
        }
 
        public void OpenReview()
        {
            Application.OpenURL(KeyStore.REF_FEEDBACK);
        }

        public void OpenPolicy()
        {
            Application.OpenURL(KeyStore.REF_PRIVACY);
        }

        public void OpenDeveloper()
        {
            Application.OpenURL(KeyStore.REF_DEVELOPER);
        }

        public void OpenLogin()
        {
            _networkLogin.CurrentLoginMethod = NetworkLogin.LoginMethod.ChangeAccount;
            SceneManager.LoadScene(0);
        }
    }
}