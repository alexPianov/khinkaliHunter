using System.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Playstel;
using Project.Scripts.Network;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using static Project.Scripts.Network.NetworkUserData;
using static Project.Scripts.Network.NetworkUserData.UserDataType;

public class Player : MonoBehaviour
{
    [Header("Main States")]
    public bool isDead = false;
    public bool isGrounded = false;
    public bool isHoldingJump = false;
    public bool onJumpButton = false;
    
    [Header("Extra States")]
    private ObscuredBool doubleJump;
    private ObscuredBool speedReverse;
    private ObscuredBool spikesProtection;
    private ObscuredBool timeStone;
    public ObscuredBool inContract { get; private set; }
    public ObscuredBool attractor { get; private set; }
    public ObscuredBool baliMode { get; private set; }

    [Header("Values")]
    public float gravity;
    public float maxXVelocity = 100;
    public float maxAcceleration = 10;
    public float acceleration = 10;
    public ObscuredFloat distance = 0;
    public float jumpVelocity = 20;
    public float groundHeight = 10;
    public float maxHoldJumpTime = 0.4f;
    public float maxMaxHoldJumpTime = 0.4f;
    public float holdJumpTimer = 0.0f;
    public float jumpGroundThreshold = 1;
    
    [Header("Velocity")]
    public Vector2 velocity;

    [Header("Mask")]
    public LayerMask groundLayerMask;
    public LayerMask obstacleLayerMask;

    [Header("Refs")] 
    public UiInfo Info;
    [SerializeField] private UiParallax Parallax;
    [SerializeField] private UiBackground Background;
    [SerializeField] private PlayerFX EffectSpawner;
    [SerializeField] private UiButtons Buttons;
    
    [Header("Events")]
    public UnityEvent ScoreEvent;
    public UnityEvent BaliEvent;
    public UnityEvent ContractEvent;
    public UnityEvent JumpEvent;
    public UnityEvent SpeedEvent;
    public UnityEvent SpikesEvent;
    public UnityEvent GameOverEvent;

    [Inject] private NetworkUserData NetworkUserData;
    [Inject] private EffectSound Sound;
    [Inject] private EffectMusic Music;
    
    [HideInInspector]
    [Inject] public HandlerVibration HandlerVibration;

    Animator Animator;
    GroundFall fall;
    PlayerCamera cameraController;

    void Start()
    {
        Animator = GetComponent<Animator>();
        cameraController = Camera.main.GetComponent<PlayerCamera>();
        GameOverEvent.AddListener(GameOver);

        speedReverse = NetworkUserData.GetUserData(DietKhinkali);
        doubleJump = NetworkUserData.GetUserData(UserDataType.DoubleJump);
        spikesProtection = NetworkUserData.GetUserData(SpikeProtection);
        timeStone = NetworkUserData.GetUserData(TimeStone);

        Info.ActiveResource(Obstacle.Type.SpeedReverse, speedReverse);
        Info.ActiveResource(Obstacle.Type.SpikeProtection, spikesProtection);
        Info.ActiveResource(Obstacle.Type.DoubleJump, doubleJump);
        Info.ActiveTimeStone(timeStone);

        if (inContract)
        {
            Info.ActiveCharacter(UiCharacter.Type.Satan, true);
        }
        
        EffectSpawner.ActiveEffect(PlayerFX.EffectType.Spawn);
        
        Sound.PlaySound(Sound.sounds.ClipSpawn);
    }

    private void GameOver()
    {
        cameraController.StopShaking();
    }

    public int jumpNumber;
    private float groundDistance;
    
    void Update()
    {
        Vector2 pos = transform.position; 
        groundDistance = Mathf.Abs(pos.y - groundHeight);

        if (isGrounded || groundDistance <= jumpGroundThreshold || DoubleJump())
        {
            if (Input.GetKeyDown(KeyCode.Space)) Jump(); 
        }

        if (isGrounded)
        {
            jumpNumber = 0;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump = false;
            Animator.Play("Run");
        }
    }

    public void Jump()
    {
        if (Buttons.IsPaused) return;
        
        if (isGrounded || groundDistance <= jumpGroundThreshold || DoubleJump())
        {
            EffectSpawner.ActiveEffect(PlayerFX.EffectType.JumpDust);
            
            isGrounded = false;
            velocity.y = jumpVelocity;
            isHoldingJump = true;
            holdJumpTimer = 0;
            Animator.Play("Jump");
            HandlerVibration.Jump();

            if (fall != null && fall.Active)
            {
                fall.Player = null;
                fall = null;
                cameraController.StopShaking();
            }

            jumpNumber++;
            Sound.PlaySound(Sound.sounds.GetJump());
        }
    }

    public bool DoubleJump()
    {
        return jumpNumber < 2 && doubleJump;
    }

    public void ReleaseJumpButton()
    {
        if (Buttons.IsPaused) return;
        
        isHoldingJump = false;
        onJumpButton = false;
        Animator.Play("Run");
    }

    public bool isFalling;
    public void SetFallGround(float fallAmount)
    {
        groundHeight -= fallAmount;
        Vector2 playerPos = transform.position;
        playerPos.y -= fallAmount;
        transform.position = playerPos;
    }

    private bool up;
    private void FixedUpdate()
    {
        if (isDead)
        {
            if (Parallax._x >= 0)
            {
                Parallax._x -= Time.deltaTime * 0.4f;
            }
            
            return;
        }

        Parallax._x = velocity.x * 0.01f;

        Vector2 pos = transform.position;
        
        if (pos.y < -20)
        {
            isDead = true;
            
            EffectSpawner.ActiveEffect(PlayerFX.EffectType.Damage);
            
            GameOverEvent.Invoke();
        }

        if (pos.x > 60)
        {
            isGrounded = false;
            pos.y = 30;
            pos.x = 0;
        }

        if (!isGrounded)
        {
            if (isHoldingJump)
            {
                holdJumpTimer += Time.fixedDeltaTime;
                if (holdJumpTimer >= maxHoldJumpTime)
                {
                    isHoldingJump = false;
                }
            }
            
            pos.y += velocity.y * Time.fixedDeltaTime;
            
            if (!isHoldingJump)
            {
                velocity.y += gravity * Time.fixedDeltaTime;
            }

            Vector2 rayOrigin = new Vector2(pos.x + 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, groundLayerMask);
            if (hit2D.collider != null)
            {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    if (pos.y >= ground.groundHeight)
                    {
                        groundHeight = ground.groundHeight;
                        pos.y = groundHeight;
                        velocity.y = 0;
                        isGrounded = true;
                    }

                    fall = ground.GetComponent<GroundFall>();
                    
                    if (fall != null)
                    {
                        if (fall.Active)
                        {
                            fall.Player = this;
                            cameraController.StartShaking();
                            isFalling = true;
                        }
                        else
                        {
                            isFalling = false;
                            cameraController.StopShaking();
                        }
                    }
                }
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);


            Vector2 wallOrigin = new Vector2(pos.x, pos.y);
            Vector2 wallDir = Vector2.right;
            RaycastHit2D wallHit = Physics2D.Raycast(wallOrigin, wallDir, velocity.x * Time.fixedDeltaTime, groundLayerMask);
            if (wallHit.collider != null)
            {
                Ground ground = wallHit.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    if (pos.y < ground.groundHeight)
                    {
                        velocity.x = 0;
                    }
                }
            }
        }

        distance += velocity.x * Time.fixedDeltaTime;

        if (isGrounded)
        {
            float velocityRatio = velocity.x / maxXVelocity;
            acceleration = maxAcceleration * (1 - velocityRatio);
            maxHoldJumpTime = maxMaxHoldJumpTime * velocityRatio;

            velocity.x += acceleration * Time.fixedDeltaTime;
            if (velocity.x >= maxXVelocity)
            {
                velocity.x = maxXVelocity;
            }


            Vector2 rayOrigin = new Vector2(pos.x - 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            if (fall != null && fall.Active)
            {
                rayDistance = -fall.fallSpeed * Time.fixedDeltaTime;
            }
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if (hit2D.collider == null)
            {
                isGrounded = false;
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.yellow);

        }

        Vector2 obstOrigin = new Vector2(pos.x, pos.y);
        RaycastHit2D obstHitX = Physics2D.Raycast(obstOrigin, Vector2.right, velocity.x * Time.fixedDeltaTime, obstacleLayerMask);
        if (obstHitX.collider != null)
        {
            Obstacle obstacle = obstHitX.collider.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                hitObstacle(obstacle);
            }
        }

        RaycastHit2D obstHitY = Physics2D.Raycast(obstOrigin, Vector2.up, velocity.y * Time.fixedDeltaTime, obstacleLayerMask);
        if (obstHitY.collider != null)
        {
            Obstacle obstacle = obstHitY.collider.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                hitObstacle(obstacle);
            }
        }
        
        transform.position = pos;
    }

    public async void hitObstacle(Obstacle obstacle)
    {
        if (obstacle.currentType == Obstacle.Type.Khinkaly || 
            obstacle.currentType == Obstacle.Type.GodsKhinkaly)
        {
            if (inContract)
            {
                Sound.PlaySound(Sound.sounds.ClipThrow);
                obstacle.MoveToSatan(); return;
            }
            
            HandlerVibration.GrabScore();
            
            ScoreEvent.Invoke();
            Animator.Play("Eat");
            Destroy(obstacle.gameObject);
            
            EffectSpawner.ActiveEffect(PlayerFX.EffectType.Eat);
            Sound.PlaySound(Sound.sounds.GetEat());

            if (obstacle.currentType == Obstacle.Type.GodsKhinkaly) return; 
            
            if (attractor) return; 

            if (speedReverse)
            {
                velocity.x *= 0.95f;
                return;
            }
            
            velocity.x *= 0.75f;
        }
        
        if (obstacle.currentType == Obstacle.Type.Bali)
        {
            Destroy(obstacle.gameObject);
            baliMode = !baliMode;
            velocity.x *= 0.85f;
            BaliEvent.Invoke();
            HandlerVibration.GrabCharacter();
            Background.BackgroundBali(baliMode);
            EffectSpawner.ActiveEffect(PlayerFX.EffectType.Portal);
            Sound.PlaySound(Sound.sounds.ClipPortal);
            Animator.Play("Use");
            
            if (baliMode)
            {
                Info.ShowInfo(UiInfo.InfoType.BaliEnter);
            }
            else
            {
                Info.ShowInfo(UiInfo.InfoType.BaliExit);
            }
        }
        
        if (obstacle.currentType == Obstacle.Type.Contract)
        {
            ContractEvent.Invoke();
            HandlerVibration.GrabCharacter();
            Destroy(obstacle.gameObject);
            inContract = !inContract;
            velocity.x *= 0.85f;
            EffectSpawner.ActiveEffect(PlayerFX.EffectType.Contract);
            Animator.Play("Use");
            
            if (inContract)
            {
                Info.ShowInfo(UiInfo.InfoType.SatanEnter, contractCount, UiInfo.InfoFormat.Contract);
                Info.ActiveCharacter(UiCharacter.Type.Satan, true);
                Sound.PlaySound(Sound.sounds.ClipContract);
                contractCount = contractMaxCount;
            }
            else
            {
                Info.ShowInfo(UiInfo.InfoType.SatanExit);
                Info.ActiveCharacter(UiCharacter.Type.Satan, false);
                Sound.PlaySound(Sound.sounds.GetSatanHide());
                contractCount = contractMaxCount;
            }
        }
        
        if (obstacle.currentType == Obstacle.Type.Spikes)
        {
            EffectSpawner.ActiveEffect(PlayerFX.EffectType.Damage);
            Sound.PlaySound(Sound.sounds.GetDamage());
            Animator.Play("Use");
            HandlerVibration.Spikes();
            
            if (spikesProtection)
            {
                Info.ShowInfo(UiInfo.InfoType.SpikesDestroy);
                velocity.x *= 0.70f;
                Destroy(obstacle.gameObject);
                return;
            }
            
            isGrounded = false;
            velocity.y = jumpVelocity;
            velocity.x = 0;
            isHoldingJump = true;
            holdJumpTimer = 0;
            
            cameraController.StartShaking();
            SpikesEvent.Invoke();
            
            if (cameraController) cameraController.StopShaking();

            Music.PlayMusic(null);
            
            Destroy(obstacle.gameObject);
            
            await UniTask.Delay(500);
            
            GameOverEvent.Invoke();
        }

        if (obstacle.currentType == Obstacle.Type.SpikeProtection)
        {
            spikesProtection = !spikesProtection;
            Destroy(obstacle.gameObject);
            HandlerVibration.GrabPotion();
            velocity.x *= 0.85f;
            
            EffectSpawner.ActiveEffect(PlayerFX.EffectType.Defence);
            Sound.PlaySound(Sound.sounds.GetImpact());
            Animator.Play("Use");
            
            if (spikesProtection)
            {
                Info.ShowInfo(UiInfo.InfoType.PotionSpikesDefenceOn);
                Info.ActiveResource(Obstacle.Type.SpikeProtection, true);
            }
            else
            {
                Info.ShowInfo(UiInfo.InfoType.PotionSpikesDefenceOff);
                Info.ActiveResource(Obstacle.Type.SpikeProtection, false);
            }
        }
        
        if (obstacle.currentType == Obstacle.Type.DoubleJump)
        {
            doubleJump = !doubleJump;
            Destroy(obstacle.gameObject);
            HandlerVibration.GrabPotion();
            JumpEvent.Invoke();
            velocity.x *= 0.85f;

            EffectSpawner.ActiveEffect(PlayerFX.EffectType.Jump);
            Sound.PlaySound(Sound.sounds.GetImpact());
            Animator.Play("Use");
            
            if (doubleJump)
            {
                Info.ShowInfo(UiInfo.InfoType.PotionDoubleJumpOn);
                Info.ActiveResource(Obstacle.Type.DoubleJump, true);
            }
            else
            {
                Info.ShowInfo(UiInfo.InfoType.PotionDoubleJumpOff);
                Info.ActiveResource(Obstacle.Type.DoubleJump, false);
            }
        }
        
        if (obstacle.currentType == Obstacle.Type.SpeedReverse)
        {
            speedReverse = !speedReverse;
            Destroy(obstacle.gameObject);
            HandlerVibration.GrabPotion();
            SpeedEvent.Invoke();
            velocity.x *= 0.85f;
            
            EffectSpawner.ActiveEffect(PlayerFX.EffectType.Speed);
            Sound.PlaySound(Sound.sounds.GetImpact());
            Animator.Play("Use");
            
            if (speedReverse)
            {
                Info.ShowInfo(UiInfo.InfoType.PotionDietKhinkaliOn);
                Info.ActiveResource(Obstacle.Type.SpeedReverse, true);
            }
            else
            {
                Info.ShowInfo(UiInfo.InfoType.PotionDietKhinkaliOff);
                Info.ActiveResource(Obstacle.Type.SpeedReverse, false);
            }
        }
    }

    [HideInInspector]public int contractMaxCount = 5;
    [HideInInspector]public int contractCount = 5;
    public async void SatanScore()
    {
        contractCount--;
        Sound.PlaySound(Sound.sounds.GetSatanEat());

        if (contractCount == 0)
        {
            inContract = false;
            await UniTask.Delay(200);
            Info.ActiveCharacter(UiCharacter.Type.Satan, false);
            contractCount = contractMaxCount;
            Sound.PlaySound(Sound.sounds.GetSatanHide());
            HandlerVibration.GrabCharacter();
        }
    }
}
