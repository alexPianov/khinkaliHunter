using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UiSatan : MonoBehaviour
    {
        public Animator animator;
        public TMP_Text textSatanScore;
        
        [Inject] private Player player;

        public void Start()
        {
            player.ContractEvent.AddListener(NewContract);
        }

        private void NewContract()
        {
            textSatanScore.text = $"{player.contractMaxCount}";
        }
        
        public void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Obstacle obstacle))
            {
                if (obstacle.moveToSatan)
                {
                    player.SatanScore();
                    Destroy(obstacle.gameObject);
                    
                    if (player.contractCount < 0 || !player.inContract)
                    {
                        textSatanScore.text = "0";
                    }
                    else
                    {
                        textSatanScore.text = $"{player.contractCount}";
                    }
                    
                    animator.Play("Eat");
                }
            }
        }
    }
}