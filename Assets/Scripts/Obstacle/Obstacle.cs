using DG.Tweening;
using Playstel;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

public class Obstacle : MonoBehaviour
{
    public Type currentType;
    public AssetReference reference;  
    
    private Transform _transformPlayer;
    private Transform _transformSatan;
    private Transform _transformScoreAttractor;

    [Inject] private Player player;

    public enum Type
    {
        Khinkaly, Bali, Contract, Spikes, DoubleJump, SpeedReverse, SpikeProtection, GodsKhinkaly, ScoreKhinkaly, Dust
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "2_Level")
        {
            Debug.Log("Wrong scene! Destroy: " + gameObject.name);
            Destroy(gameObject);
        }
        
        if (player)
        {
            _transformPlayer = player.transform;
            _transformSatan = player.Info.GetCharacter(UiCharacter.Type.Satan);
        }

        if (currentType == Type.ScoreKhinkaly)
        {
            var attractor = GameObject.FindWithTag("Attractor");
            if(attractor) _transformScoreAttractor = attractor.transform;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (currentType == Type.Spikes)
        {
            if (other.TryGetComponent(out Obstacle obstacle))
            {
                if(obstacle.currentType == Type.GodsKhinkaly 
                   || obstacle.currentType == Type.Spikes) return;
                
                Destroy(obstacle.gameObject);
            }
        }
    }

    public bool moveToSatan;
    public void MoveToSatan()
    {
        moveToSatan = true;
    }

    private float attractDistance = 10f;
    public bool CanBeAttract()
    {
        var distance = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log("Distance: " + distance);
        return distance < attractDistance;
    }

    private void FixedUpdate()
    {
        if (currentType == Type.ScoreKhinkaly && _transformScoreAttractor)
        {
            Move(_transformScoreAttractor.position);
            return;
        }
        
        if (player == null) return;

        if (player.isDead)
        {
            if (currentType == Type.GodsKhinkaly)
            {
                Move(_transformPlayer.position);
            }
            
            return;
        }
        
        if (player.attractor && !moveToSatan && CanBeAttract())
        {
            if (currentType == Type.GodsKhinkaly || currentType == Type.Khinkaly)
            {
                Move(_transformPlayer.position);
                return;
            }
        }
        
        if (moveToSatan)
        {
            Move(_transformSatan.position); 
            return;
        }
        
        if (currentType == Type.GodsKhinkaly)
        {
            Move(_transformPlayer.position); return;
        }
        
        Vector2 pos = transform.position;

        pos.x -= player.velocity.x * Time.fixedDeltaTime;
        
        if (pos.x < -100)
        {
            if (currentType == Type.Dust)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        transform.position = pos;
    }

    private float timer;
    private float tweenInterval = 0.02f;
    private float moveSpeed = 0.2f;
    private void Move(Vector3 pos)
    {
        timer += Time.deltaTime;

        if (timer > tweenInterval)
        {
            transform.DOMove(pos, moveSpeed).SetEase(Ease.Flash);
            timer = 0;
        }
    } 
}
