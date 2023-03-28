using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class Effect : MonoBehaviour
    {
        private CancellationToken token;
        public bool disableAfterDelay;
        
        private async void Start()
        {
            await UniTask.Delay(500, cancellationToken: token);
            
            if (disableAfterDelay)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if(gameObject != null) Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            token.WaitHandle.Dispose();
        }
    }
}