using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Scriptable_Store
{
    [CreateAssetMenu(menuName = "Audio/Sounds")]
    public class AudioSounds : ScriptableObject
    {
        [Header("Ui")]
        public AudioClip ClipClickStart;
        public AudioClip ClipClickButton;
        public AudioClip ClipClickButtonBack;
        public AudioClip ClipGameOver;
        public AudioClip ClipScore;
        
        [Header("Hero")]
        public AudioClip ClipSpawn;
        public List<AudioClip> ClipEat;
        public List<AudioClip> ClipImpact;
        public List<AudioClip> ClipJump;
        public List<AudioClip> ClipDamage;
        public List<AudioClip> ClipGroundFall;
        public AudioClip ClipTimeSlow;
        public AudioClip ClipTimeNormalize;
        
        [Header("Characters")]
        public AudioClip ClipGodAppear;
        public List<AudioClip> ClipSatanEat;
        public List<AudioClip> ClipSatanHide;
        public AudioClip ClipThrow;
        public AudioClip ClipPortal;
        public AudioClip ClipContract;
        
        [Header("Menu")]
        public AudioClip ClipMainMenu;
        
        [Header("Game")]
        public List<AudioClip> ClipGameTracks;
        public AudioClip ClipResults;

        public AudioClip GetRandomTrack()
        {
            return ClipGameTracks[Random.Range(0, ClipGameTracks.Count)];
        }
        
        public AudioClip GetDamage()
        {
            return ClipDamage[Random.Range(0, ClipDamage.Count - 1)];
        }
        
        public AudioClip GetSatanEat()
        {
            return ClipSatanEat[Random.Range(0, ClipSatanEat.Count - 1)];
        }
        
        public AudioClip GetEat()
        {
            return ClipEat[Random.Range(0, ClipEat.Count - 1)];
        }
        
        public AudioClip GetImpact()
        {
            return ClipImpact[Random.Range(0, ClipImpact.Count - 1)];
        }
        
        public AudioClip GetSatanHide()
        {
            return ClipSatanHide[Random.Range(0, ClipSatanHide.Count - 1)];
        }
        
        public AudioClip GetJump()
        {
            return ClipJump[Random.Range(0, ClipJump.Count - 1)];
        }

        public AudioClip GetGroundFall()
        {
            return ClipGroundFall[Random.Range(0, ClipGroundFall.Count)];
        }
    }
}