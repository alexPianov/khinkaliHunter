using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class NotifyOnDisable : MonoBehaviour
    {
        public event Action<GameObject, Queue<GameObject>> Disabled;
        public GameObject instanceRef { get; set; }
        public Queue<GameObject> queue { get; set; }

        public void OnDisable()
        {
            Disabled.Invoke(instanceRef, queue);
        }
    }
}
