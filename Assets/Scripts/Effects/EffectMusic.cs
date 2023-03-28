using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playstel
{
    public class EffectMusic : EffectSound
    {
        [Header("Mode")]
        private bool playMenuMusic;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private int currentScene;
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            currentScene = scene.buildIndex;
            Play();
        }

        private void Update()
        {
            if (currentScene != 0 && !MusicIsPlaying()) Play(); 
        }
        
        private void Play()
        {
            if (currentScene == 0)
            {
                PlayMusic(null);
            }
            if (currentScene == 1)
            {
                PlayMusic(sounds.ClipMainMenu);
            }
            if (currentScene == 2)
            {
                PlayMusic(sounds.GetRandomTrack());
            }
        }
    }
}