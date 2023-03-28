using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class HandlerNetworkError : MonoBehaviour
    {
        public GameObject errorPanel;
        public Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePanel);
        }

        public void StartTimer(bool state)
        {
            _startTimer = state;
            if (!state) _waitTime = 0;
        }

        private float _waitTime;
        private float _maxWaitTime = 12;
        private bool _startTimer;
        private void Update()
        {
            if (!_startTimer) return;
            
            _waitTime += Time.deltaTime;

            if (_waitTime > _maxWaitTime)
            {
                _startTimer = false;
                _waitTime = 0;
                ActivePanel(true);
            }
        }

        public void ActivePanel(bool state)
        {
            errorPanel.SetActive(state);

            if (state)
            {
                //HandlerLoading.OpenLoadingPopup(false);
                //HandlerLoading.OpenLoadingScreen(false);
            }
        }

        public void ClosePanel()
        {
            ActivePanel(false);
            //BootstrapInstaller.DataBoot();
        }
    }
}
