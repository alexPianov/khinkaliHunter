using System;
using Project.Scripts.Scriptable_Store;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class EffectSound : MonoBehaviour
    {
        [Header("Setup")]
        public AudioSounds sounds;

        [Header("Source")] 
        [SerializeField] private AudioSource source;

        [Header("Mixer")] 
        [SerializeField] private AudioMixer mixer;
        
        public float normalVolume = 0;
        private float normalGain = 1;

        public enum Type
        {
            Volume, Gain
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null)
            {
                source.mute = true;
                return;
            }
            
            source.mute = false;
            source.clip = clip;
            source.Play();
        }


        public bool MusicIsPlaying()
        {
            if (source.mute) return true;
            return source.isPlaying;
        }

        public void PlaySound(AudioClip clip)
        {
            if(clip == null) return;
            
            source.pitch = RandomValue(0.1f);
            source.PlayOneShot(clip, RandomValue(0.2f, 1));
        }

        private float RandomValue(float amplitude, float volume = 1)
        {
            return Random.Range(volume - amplitude, volume + amplitude);
        }

        public void Disable(bool state)
        {
            var value = normalVolume;
            
            if (state) value = -80; 
            
            mixer.SetFloat(Type.Volume.ToString(), value);
        }

        public void SetMuffle(bool state)
        {
            var value = normalGain;
            
            if (state) value = normalGain / 3; 
            
            mixer.SetFloat(Type.Gain.ToString(), value);
        }
    }
}