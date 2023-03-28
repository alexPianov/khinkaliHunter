using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Playstel
{
    public class UiCharacter : UiTransparency
    {
        public Type CurrentType;
        public SpriteRenderer Effect;
        
        public enum Type
        {
            Satan, God
        }

        public void Active(bool state)
        {
            Transparency(!state);
            
            if (state)
            {
                transform.SetSiblingIndex(0);
            }

            if (Effect) Effect.enabled = state;
        }
    }
}