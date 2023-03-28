using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Playstel;
using Project.Scripts.Addressable;
using Project.Scripts.Zenject;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Ground : MonoBehaviour
{
    [HideInInspector][Inject] public Player Player;
    [HideInInspector][Inject] public EffectSound Sound;

    [HideInInspector] public float groundHeight;
    [HideInInspector] public float groundRight;
    [HideInInspector] public float screenRight;
    
    BoxCollider2D collider;

    bool didGenerateGround = false;

    [Header("Obstacles")]
    public Obstacle khinkalyTemplate;
    public Obstacle contractTemplate;
    public Obstacle portalTemplate;
    public Obstacle spikesTemplate;
    public Obstacle speedTemplate;
    public Obstacle spikeProtectionTemplate;
    public Obstacle doubleJumpTemplate;
    
    [Header("Decorations")]
    public List<GameObject> decorationList;
    public List<GameObject> decorationListBali;
    public Transform decorationContainer;

    [Header("Settings")] 
    [SerializeField] private ObscuredInt minFallSpeed = 1;
    [SerializeField] private ObscuredInt maxFallSpeed = 2;
    [SerializeField] private ObscuredInt fallProbability = 3;
    [SerializeField] private ObscuredInt maxKhinkaly = 3;
    [SerializeField] private ObscuredInt spikesProbability = 40;
    [SerializeField] private ObscuredInt charactersProbability = 6;
    [SerializeField] private ObscuredInt doubleJumpProbability = 10;
    [SerializeField] private ObscuredInt protectionProbability = 6;
    [SerializeField] private ObscuredInt dietProbability = 10;

    [Inject] private AddressableHandler _addressableHandler;
    [Inject] private EffectPostFX PostFX;

    private bool firstGenerate;
    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        screenRight = Camera.main.transform.position.x * 2;
        
        Player.SpikesEvent.AddListener(DisableGround);
        Player.BaliEvent.AddListener(RecreateGround);
    }

    public async void RecreateGround()
    {
        await GenerateDecoration();
    }
    
    public void DisableGround()
    {
        collider.enabled = false;
    }
    
    void Update()
    {
        if(Player.isDead) return;
        groundHeight = transform.position.y + (collider.size.y / 2);
    }

    private int destroyBorder = -15;
    private void FixedUpdate()
    {
        if(Player.isDead) return;
        
        Vector2 pos = transform.position;
        pos.x -= Player.velocity.x * Time.fixedDeltaTime;
        
        groundRight = transform.position.x + (collider.size.x / 2);

        if (groundRight < destroyBorder)
        {
            Destroy(gameObject);
            return;
        }

        if (!didGenerateGround)
        {
            if (groundRight < screenRight)
            {
                didGenerateGround = true;
                GenerateGround();
            }
        }

        transform.position = pos;
    }

    private const float minY = 5;
    private async void GenerateGround()
    {
        var go = await _addressableHandler
            .CreateGameObject(AddressableHandler.ObjectName.Ground);
        
        //GameObject go = Instantiate(gameObject);
        BoxCollider2D goCollider = go.GetComponent<BoxCollider2D>();
        Vector2 pos;
        
        float h1 = Player.jumpVelocity * Player.maxHoldJumpTime;
        float t = Player.jumpVelocity / -Player.gravity;
        float h2 = Player.jumpVelocity * t + (0.5f * (Player.gravity * (t * t)));
        float maxJumpHeight = h1 + h2;
        float maxY = maxJumpHeight * 0.7f;
        maxY += groundHeight;
        //float minY = 1;
        float actualY = Random.Range(minY, maxY);

        pos.y = actualY - goCollider.size.y / 2;
        if (pos.y > 2.7f)
            pos.y = 2.7f;

        float t1 = t + Player.maxHoldJumpTime;
        float t2 = Mathf.Sqrt((2.0f * (maxY - actualY)) / -Player.gravity);
        float totalTime = t1 + t2;
        float maxX = totalTime * Player.velocity.x;
        maxX *= 0.7f;
        maxX += groundRight;
        float minX = screenRight + 5;
        float actualX = Random.Range(minX, maxX);

        pos.x = actualX + goCollider.size.x / 2; 
        go.transform.position = pos;

        Ground goGround = go.GetComponent<Ground>();
        goGround.groundHeight = go.transform.position.y + (goCollider.size.y / 2);
        
        if (Random.Range(0, fallProbability) == 0)
        {
            var fall = GetComponent<GroundFall>();
            fall.Active = true;
            fall.fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        }
        
        go.GetComponent<Ground>().GenerateDecoration();
        
        if(portalTemplate) CreateObstacle(goGround, goCollider, go, null, portalTemplate, charactersProbability);
        if(contractTemplate) CreateObstacle(goGround, goCollider, go, null, contractTemplate, charactersProbability);
        if(spikesTemplate) CreateObstacle(goGround, goCollider, go, null, spikesTemplate, spikesProbability);
        if(khinkalyTemplate) CreateObstacle(goGround, goCollider, go, null, khinkalyTemplate);
        if(doubleJumpTemplate) CreateObstacle(goGround, goCollider, go, null, doubleJumpTemplate, doubleJumpProbability);
        if(spikeProtectionTemplate) CreateObstacle(goGround, goCollider, go, null, spikeProtectionTemplate, protectionProbability);
        if(speedTemplate) CreateObstacle(goGround, goCollider, go, null, speedTemplate, dietProbability);
    }

    private GameObject decoration;
    private int lastDecorationIndex;
    public async UniTask GenerateDecoration()
    {
        var decorations = decorationList;

        if (Player.baliMode)
        {
            decorations = decorationListBali;
        }
        
        var decorationIndex = Random.Range(1, decorations.Count);
        
        if (decorationIndex == lastDecorationIndex)
        {
            GenerateDecoration();
            return;
        }

        if (decorationContainer == null) return;
        
        if (decoration) Destroy(decoration);
        
        for (var i = 0; i < decorationContainer.childCount; i++)
        {
            Destroy(decorationContainer.GetChild(i).gameObject);
        }
        
        lastDecorationIndex = decorationIndex;

        if (Player.baliMode)
        {
            decoration = await _addressableHandler.GetDecoration
                (AddressableHandler.DecorationType.Bali, decorationIndex, 
                    decorationContainer);
        }
        else
        {
            decoration = await _addressableHandler.GetDecoration
                (AddressableHandler.DecorationType.Standart, decorationIndex, 
                    decorationContainer);
        }

        //decoration = Instantiate(decorations[decorationIndex], decorationContainer);
    }

    private async UniTask CreateObstacle(Ground goGround, BoxCollider2D goCollider, 
        GameObject go, GroundFall fall, Obstacle obstacle, int probability = 10)
    {
        int obstacleNum = 0;
        
        if (obstacle.currentType == Obstacle.Type.Khinkaly)
        {
            obstacleNum = Random.Range(0, maxKhinkaly);
        }
        else
        {
            var randomValue = Random.Range(1, 100);
            if (randomValue <= probability) obstacleNum = 1;
        }
        
        for (int i = 0; i < obstacleNum; i++)
        {
            GameObject box = await _addressableHandler
                .CreateGameObject(obstacle.reference);
            
            float y = goGround.groundHeight;
            float halfWidth = goCollider.size.x / 2 - 1;
            float left = go.transform.position.x - halfWidth;
            float right = go.transform.position.x + halfWidth;
            float x = Random.Range(left, right);
            Vector2 boxPos = new Vector2(x, y);
            box.transform.position = boxPos;

            Obstacle o = box.GetComponent<Obstacle>();
            go.GetComponent<GroundFall>().SetObstacle(o);
        }
    }
}
