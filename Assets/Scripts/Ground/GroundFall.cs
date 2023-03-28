using System;
using System.Collections;
using System.Collections.Generic;
using Playstel;
using UnityEngine;
using Zenject;

public class GroundFall : MonoBehaviour
{
    public float fallSpeed = 1;
    public List<Obstacle> obstacles = new();

    private bool shouldFall = false;
    private bool fallIsStarted;
    
    [HideInInspector] public Player Player;
    [HideInInspector] public EffectSound Sound;

    private void Start()
    {
        Player = GetComponent<Ground>().Player;
        Sound = GetComponent<Ground>().Sound;
        
        fallIsStarted = false;
    }

    public void SetObstacle(Obstacle obstacle)
    {
        obstacles.Add(obstacle);
    }

    public bool Active { get; set; }

    private void FixedUpdate()
    {
        if (!Active) return;
        
        if (shouldFall)
        {
            Vector2 pos = transform.position;
            float fallAmount = fallSpeed * Time.fixedDeltaTime;
            pos.y -= fallAmount;

            if (Player != null)
            {
                if (Player.isFalling)
                {
                    Player.SetFallGround(fallAmount);
                }

                if (!fallIsStarted)
                {
                    fallIsStarted = true;
                    Sound.PlaySound(Sound.sounds.GetGroundFall());
                    Player.HandlerVibration.GroundFall();
                }
            }
            
            foreach (Obstacle o in obstacles)
            {
                if (o != null)
                {
                    Vector2 oPos = o.transform.position;
                    oPos.y -= fallAmount;
                    o.transform.position = oPos;
                }
            }
            
            transform.position = pos;
        }
        else
        {
            if (Player != null)
            {
                shouldFall = true;
            }
        }

    }
}
