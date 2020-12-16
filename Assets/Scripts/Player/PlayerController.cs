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
     [SerializeField] private float stickToGround = 2.0f;
     [SerializeField] private float stateChangeSpeed = 1.0f;
     [SerializeField] private PlayerStandState standSetting;
     [SerializeField] private PlayerStandState crouchSetting;
     [SerializeField] private AnimationCurve stateChangeCurve;
    

     private float attackInterval => 1.0f / pe.CurrentWeapon.fireRate;

     private Vector3 moveDir;
     
     private Vector3 playerVelocity = Vector3.zero;
    
     private float attackTimer = 0.0f;
     private float standStateBlend;
    
     private bool CanAttack => attackTimer >= attackInterval;
     private bool aimingDownSights;

     private CharacterController cc;
    
     private PlayerAnimator pa;
     private PlayerEquipment pe;
     
     [System.Serializable]
     public class PlayerStandState
     {
         public float controlCameraHeight;
         public float colliderHeight;
         public float colliderCenterHeight;
     }
     
     private void Start()
     {
         cc = GetComponent<CharacterController>();
         pa = Player.instance.Animator;
         pe = Player.instance.Equipment;
     }
    
     private void Update()
     {
         var fixedDelta = Time.deltaTime;
         
         UpdatePlayer();
         WeaponControls(fixedDelta);
     }

     private void UpdatePlayer()
     {
         var fixedDelta = Time.deltaTime;
         
         if (cc.isGrounded)
         {
             MoveControls(fixedDelta);
         }
         else
         {
             ApplyGravity();
         }
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
         
         if (fire && CanAttack)
         {
             if (pe.CurrentWeapon.weaponType == WeaponData.WeaponType.Melee)
             {
                 attackTimer = 0.0f;
                 pa.Fire();
             }
             else
             {
                 if (pe.CurrentWeapon.currentAmmo > 0)
                 {
                     pe.CurrentWeapon.currentAmmo--;
                     attackTimer = 0.0f;
                     pa.Fire();
                 }
                 else
                 {
                    pa.Reload();
                 }
             }
         }
         else pa.ResetAttack();
         
         pa.AimDownSights(aim);
     }
    
     private void MoveControls(float dTime)
     { 
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        var isSprinting = Input.GetAxisRaw("Sprint") >= 1.0f;
        var isCrouching = Input.GetAxisRaw("Crouch") >= 1.0f;
        var isJumping = Input.GetAxisRaw("Jump") >= 1.0f;
        
        var isMoving = !Mathf.Approximately(v, 0.0f) ||
                       !Mathf.Approximately(h, 0.0f);
         
        if (isJumping && cc.isGrounded)
        {
            CurrentMoveState = PlayerMoveState.Jumping;
        }
        else if (isMoving)
        {
            if (isSprinting && v > 0.0f) CurrentMoveState = PlayerMoveState.Sprint;
            else CurrentMoveState = isCrouching ? PlayerMoveState.CrouchRun : PlayerMoveState.Run;
        }
        else
        {
            CurrentMoveState = PlayerMoveState.Idle;
        }
         
        switch (CurrentMoveState)
        {
            case PlayerMoveState.Idle:
                pa.Idle();
                break;
             
            case PlayerMoveState.CrouchIdle:
                pa.Idle();
                break;
             
            case PlayerMoveState.Run:
                pa.Run();
                break;
             
            case PlayerMoveState.CrouchRun:
                pa.Run();
                break;
             
            case PlayerMoveState.Sprint:
                pa.Sprint();
                break;
        }
        
        var forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        var moveVector = forward * v + transform.right * h;

        var speed = runSpeed;
        if (isSprinting && !isCrouching && v > 0.0f)
        {
            speed = sprintSpeed;
        }
        else if (isCrouching)
        {
            speed = crouchSpeed;
        }
        
        StandStateChange(isCrouching);
        
        var r = new Ray(transform.position, Vector3.down);

        Physics.SphereCast(r, cc.radius, out var hitInfo, cc.height / 2f, groundLayer);

        var desiredVelocity = Vector3.ProjectOnPlane(moveVector, hitInfo.normal) * speed;
        playerVelocity.x = desiredVelocity.x;
        playerVelocity.z = desiredVelocity.z;
        playerVelocity.y = -stickToGround;

        if (!cc.isGrounded) return;
        
        if (isJumping && !isCrouching)
        {
            playerVelocity.y = jumpHeight;
        }
        
        cc.Move(playerVelocity * Time.deltaTime);
    }
     
     private void StandStateChange(bool isCrouching)
    {
        
        standStateBlend = Mathf.MoveTowards(standStateBlend, isCrouching ? 1f : 0f, Time.deltaTime * stateChangeSpeed);

        cc.height = Mathf.Lerp(
            standSetting.colliderHeight,
            crouchSetting.colliderHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );


        var colliderCenter = cc.center;

        colliderCenter.y = Mathf.Lerp(
            standSetting.colliderCenterHeight,
            crouchSetting.colliderCenterHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );
        cc.center = colliderCenter;

        var myCamera = Camera.main.transform;
        var cameraPos = myCamera.localPosition;
        
        cameraPos.y = Mathf.Lerp(
            standSetting.controlCameraHeight,
            crouchSetting.controlCameraHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );
        myCamera.localPosition = cameraPos;
    }
     private void ApplyGravity()
     {
         playerVelocity += Vector3.down * (-Physics.gravity.y * Time.deltaTime);
         cc.Move(playerVelocity * Time.deltaTime);
     }
}
