using System;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiScoreAttractor : MonoBehaviour
    {
        [SerializeField] private TMP_Text textScore;
        [SerializeField] private Animation textAnimation;
        
        [Inject] private EffectSound Sound;

        private ObscuredInt score;
        public void SetScore(int _score = 1)
        {
            score = _score;
            textScore.text = score.ToString();
        }

        private int localScore;
        public void IncrementScore()
        {
            score++;
            localScore++;
            //textScore.text = $"{score} <+{localScore}>";
            textScore.text = $"{score}";
        }
        
        public void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Obstacle obstacle))
            {
                if (obstacle.currentType == Obstacle.Type.ScoreKhinkaly)
                {
                    Destroy(obstacle.gameObject);
                    IncrementScore();
                    textAnimation.Play();
                    Sound.PlaySound(Sound.sounds.GetEat());
                }
            }
        }
    }
}