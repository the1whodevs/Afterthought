using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
     public enum PlayerMoveState { Idle, CrouchIdle, Run, CrouchRun, Sprint, Jumping }
    
     public PlayerMoveState CurrentMoveState = PlayerMoveState.Idle;
    
     [Header("Move Settings")]
     [SerializeField] private float runSpeed = 10.0f;
     [SerializeField] private float crouchSpeed = 7.0f;
     [SerializeField] private float sprintSpeed = 20.0f;
     [SerializeField] private float jumpHeight = 15.0f;
     [SerializeField] private LayerMask groundLayer;
     [SerializeField] private float stickToGround = 9.81f;
    
     [Header("Attack Settings")] 
     [SerializeField] private float attackInterval = 1.0f;
    
     private Vector3 moveDir;
     private Vector3 playerVelocity = Vector3.zero;
    
     private float attackTimer = 0.0f;
    
     private bool CanAttack => attackTimer >= attackInterval;
     private bool aimingDownSights;
    
     private CharacterController cc;
    
     private PlayerAnimator pa;
     private PlayerEquipment pe;
     
     private void Start()
     {
         cc = GetComponent<CharacterController>();
         pa = Player.instance.Animator;
         pe = Player.instance.Equipment;
     }
    
     private void FixedUpdate()
     {
         var fixedDelta = Time.fixedDeltaTime;
         
         MoveControls(fixedDelta);
         WeaponControls(fixedDelta);
     }
    
     private void WeaponControls(float dTime)
     {
         attackTimer += dTime;
         
         var fire = Input.GetAxisRaw("Fire1") > 0.0f;
         var aim = Input.GetAxisRaw("Fire2") > 0.0f;
         var tryEquipPrimary = Input.GetAxisRaw("EquipPrimary") < 0.0f;
         var tryEquipSecondary = Input.GetAxisRaw("EquipSecondary") > 0.0f;
         
         if (tryEquipPrimary) pe.EquipPrimaryWeapon();
         else if (tryEquipSecondary) pe.EquipSecondaryWeapon();
         
         // TODO: Check attack speed.
         if (fire && CanAttack)
         {
             attackTimer = 0.0f;
             pa.Fire();
         }
         else pa.ResetAttack();
         
         pa.AimDownSights(aim);
     }
    
     private void MoveControls(float dTime)
     {

        //  var j = Input.GetAxisRaw("Jump") >= 1.0f;
        //  var s = Input.GetAxisRaw("Sprint") >= 1.0f;
        //  var c = Input.GetAxisRaw("Crouch") >= 1.0f;
        //  var groundedPlayer = cc.isGrounded;
        //
        //  if (groundedPlayer && moveDir.y < 0.0f) moveDir.y = 0.0f;
        //  
        // //var m = Mathf.Abs(v) + Mathf.Abs(h);
        //  //if (m > 1.0f) m = 1.0f;
        //  
        //  if (v > 0.0f) moveDir += transform.forward;
        //  else if (v < 0.0f) moveDir += -transform.forward;
        //  
        //  if (h > 0.0f) moveDir += transform.right;
        //  else if (h < 0.0f) moveDir += -transform.right;
        //
        //  moveDir.Normalize();
        //
        //  var isMoving = !Mathf.Approximately(v, 0.0f) ||
        //                 !Mathf.Approximately(h, 0.0f);
        //  
        //  if (j && groundedPlayer)
        //  {
        //      CurrentMoveState = PlayerMoveState.Jumping;
        //  }
        //  else if (isMoving)
        //  {
        //      if (s) CurrentMoveState = PlayerMoveState.Sprint;
        //      else CurrentMoveState = c ? PlayerMoveState.CrouchRun : PlayerMoveState.Run;
        //  }
        //  else
        //  {
        //      CurrentMoveState = PlayerMoveState.Idle;
        //  }
        //
        //  var speedToUse = runSpeed;
        //  
        //  switch (CurrentMoveState)
        //  {
        //      case PlayerMoveState.Idle:
        //          pa.Idle();
        //          speedToUse = 0.0f;
        //          break;
        //      
        //      case PlayerMoveState.CrouchIdle:
        //          pa.Idle();
        //          speedToUse = 0.0f;
        //          break;
        //      
        //      case PlayerMoveState.Run:
        //          pa.Run();
        //          speedToUse = runSpeed;
        //          //cc.SimpleMove(moveDir * (m * runSpeed * dTime));
        //          //rb.MovePosition(rb.position + moveDir * (m * runSpeed * dTime));
        //          break;
        //      
        //      case PlayerMoveState.CrouchRun:
        //          pa.Run();
        //          speedToUse = crouchSpeed;
        //          //cc.SimpleMove(moveDir * (m * crouchSpeed * dTime));
        //          //rb.MovePosition(rb.position + moveDir * (m * crouchSpeed * dTime));
        //          break;
        //      
        //      case PlayerMoveState.Sprint:
        //          pa.Sprint();
        //          speedToUse = sprintSpeed;
        //          //cc.SimpleMove(moveDir * (m * sprintSpeed * dTime));
        //          //rb.MovePosition(rb.position + moveDir * (m * sprintSpeed * dTime));
        //          break;
        //      
        //      case PlayerMoveState.Jumping:
        //          // Player.instance.Animator.Idle();
        //          //moveDir.y += Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
        //          playerVelocity.y = jumpHeight;
        //          //rb.SimpleMove(Vector3.up * jumpHeight);
        //          //rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        //          break;
        //      
        //      default:
        //          throw new ArgumentOutOfRangeException();
        //  }
        //  moveDir += Physics.gravity;
        //  var r = new Ray(transform.position, Vector3.down);
        //  
        //  Physics.SphereCast(r, cc.radius, out var hitInfo, cc.height / 2f, groundLayer);
        //  var desiredVelocity = Vector3.ProjectOnPlane(moveDir, hitInfo.normal) * speedToUse;
        //  
        //  playerVelocity.x = desiredVelocity.x;
        //  playerVelocity.z = desiredVelocity.z;
        //  playerVelocity.y = -stickToGround;
        //  
        //  cc.Move(playerVelocity * dTime);
        
        
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        var isSprinting = Input.GetAxisRaw("Sprint") >= 1.0f;
        var isCrouching = Input.GetAxisRaw("Crouch") >= 1.0f;
        var isJumping = Input.GetAxisRaw("Jump") >= 1.0f;
        
        var forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        var moveVector = forward * v + transform.right * h;

        var speed = runSpeed;
        if (isSprinting && !isCrouching)
        {
            speed = sprintSpeed;
        }
        else if (isCrouching)
        {
            speed = crouchSpeed;
        }
        
        StandStateChange();
        
        var r = new Ray(transform.position, Vector3.down);

        Physics.SphereCast(r, cc.radius, out var hitInfo, cc.height / 2f, groundLayer);

        var desiredVelocity = Vector3.ProjectOnPlane(moveVector, hitInfo.normal) * speed;
        playerVelocity.x = desiredVelocity.x;
        playerVelocity.z = desiredVelocity.z;
        playerVelocity.y = -stickToGround;

        var calculatedVelocity = playerVelocity;
        calculatedVelocity.y = 0;
        
        if (!cc.isGrounded) return;
        
        if (isJumping && !isCrouching)
        {
            playerVelocity.y = jumpHeight;
        }
        
        cc.Move(playerVelocity * Time.deltaTime);
    }

     
    
    // public GameObject control;
    //
    // public string forwardAxis = "Vertical";
    // public string strafeAxis = "Horizontal";
    // public Transform directionReference;
    // public float crouchSpeedMultiplier = 0.75f;
    // public float runSpeedMultiplier = 1.5f;
    // public float runIncreaseSpeedTime = 1f;
    // public float runSpeedThreshold = 1f;
    // public AnimationCurve runIncreaseSpeedCurve;
    // public float collisionScale = 1f;
    //
    // public float speed = 1f;
    // public LayerMask groundLayer;
    // public float threshold = 0.1f;
    // public float gravity = 9.81f;
    // public float stickToGround = 9.81f;
    //
    //  public enum PlayerMoveState { Idle, CrouchIdle, Run, CrouchRun, Sprint, Jumping }
    //
    //  public float stateChangeSpeed = 3.666f;
    //  public AnimationCurve stateChangeCurve;
    //  public float maxSpeed = 1f;
    //  public float weightSmooth = 3f;
    //  public float jumpSpeed = 5f;
    //  public Camera playerCamera;
    //  public Transform controlCamera;
    //
    //  public class PlayerStandState
    //  {
    //      public float controlCameraHeight;
    //      public float colliderHeight;
    //      public float colliderCenterHeight;
    //  }
    //
    //  public Vector3 PlayerVelocity
    //  {
    //      get
    //      {
    //          return controller.velocity;
    //      }
    //  }
    //  
    //  public bool isRunning { get; private set; }
    //  public bool isCrouching { get; private set; }
    //
    //  public UnityAction runStartEvent;
    //  public UnityAction jumpStartEvent;
    //  public UnityAction jumpFallEvent;
    //  public UnityAction jumpEndEvent;
    //  public UnityAction crouchEvent;
    //  public UnityAction standUpEvent;
    //
    //  private CapsuleCollider characterCollider;
    //  private CharacterController controller;
    //
    //  Vector3 playerVelocity = Vector3.zero;
    //  Vector3 oldPlayerVelocity = Vector3.zero;
    //  
    //  float standStateBlend;
    //
    //  float runTime = 0f;
    //  float standChangeTime;
    //
    //  bool oldIsGrounded = false;
    //
    //  private void Start()
    //  {
    //      characterCollider = GetComponent<CapsuleCollider>();
    //      controller = GetComponent<CharacterController>();
    //      
    //  }
    //
    //  private void Update()
    //  {
    //      
    //  }
    //
    //  private void UpdatePlayer()
    //  {
    //      
    //  }
    //
    //  private void Move()
    //  {
    //      var h = Input.GetAxis((strafeAxis));
    //      var v = Input.GetAxis(forwardAxis);
    //      
    //  }
    private void StandStateChange()
    {
        standStateBlend = Mathf.MoveTowards(standStateBlend, IsCrouching ? 1f : 0f, Time.deltaTime * stateChangeSpeed);

        charactarCollider.height = Mathf.Lerp(
            standState.colliderHeight,
            crouchState.colliderHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );


        var colliderCenter = charactarCollider.center;

        colliderCenter.y = Mathf.Lerp(
            standState.colliderCenterHeight,
            crouchState.colliderCenterHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );
        charactarCollider.center = colliderCenter;

        controller.height = charactarCollider.height;
        controller.center = charactarCollider.center;

        controlCameraPosition.y = Mathf.Lerp(
            standState.controlCameraHeight,
            crouchState.controlCameraHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );
    }
}
