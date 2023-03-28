using System;
using UnityEngine;

namespace Project.Scripts.Addressable
{
    public class NotifyOnDestroy : MonoBehaviour
    {
        public event Action<GameObject, NotifyOnDestroy> Destroyed;
        public GameObject instanceRef { get; set; }
        public bool OnBecomeInvisible;

        public void OnDestroy()
        {
            if(OnBecomeInvisible) return;
            Destroyed?.Invoke(instanceRef, this);
        }

        public void OnBecameInvisible()
        {
            if (OnBecomeInvisible)
            {
                Destroyed?.Invoke(instanceRef, this);
            }
        }
    }
}