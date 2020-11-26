using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMoveState { Idle, CrouchIdle, Run, CrouchRun, Sprint, Jumping }

    public PlayerMoveState CurrentMoveState = PlayerMoveState.Idle;

    [Header("Move Settings")]
    [SerializeField] private float runSpeed = 10.0f;
    [SerializeField] private float crouchSpeed = 7.0f;
    [SerializeField] private float sprintSpeed = 20.0f;
    [SerializeField] private float jumpHeight = 15.0f;

    [Header("Attack Settings")] 
    [SerializeField] private float attackInterval = 1.0f;

    private float attackTimer = 0.0f;

    private bool CanAttack => attackTimer >= attackInterval;
    private bool canJump = true;
    
    private Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        var tryEquipBoth = Input.GetAxisRaw("EquipBoth") > 0.0f;
        
        // Debug.LogFormat("Fire: {0} / Aim: {1} / Primary: {2} / Secondary: {3} / Both: {4}",fire,aim,tryEquipPrimary,tryEquipSecondary,tryEquipBoth);

        var pe = Player.instance.Equipment;
        
        if (tryEquipBoth && pe.CanDualWield) pe.EquipBoth();
        else if (tryEquipPrimary) pe.EquipPrimaryWeapon();
        else if (tryEquipSecondary) pe.EquipSecondaryWeapon();
        
        // TODO: Check attack speed.
        if (fire && CanAttack)
        {
            attackTimer = 0.0f;
            Player.instance.Animator.Fire();
        }
        
        // TODO: ADS/Block on aim.
    }

    private void MoveControls(float dTime)
    {
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        var j = Input.GetAxisRaw("Jump") >= 1.0f;
        var s = Input.GetAxisRaw("Sprint") >= 1.0f;
        var c = Input.GetAxisRaw("Crouch") >= 1.0f;
    
        var moveDir = Vector3.zero;
    
        var m = Mathf.Abs(v) + Mathf.Abs(h);
        if (m > 1.0f) m = 1.0f;
        
        if (v > 0.0f) moveDir += transform.forward;
        else if (v < 0.0f) moveDir += -transform.forward;
        
        if (h > 0.0f) moveDir += transform.right;
        else if (h < 0.0f) moveDir += -transform.right;
    
        moveDir.Normalize();
    
        var isMoving = !Mathf.Approximately(v, 0.0f) ||
                       !Mathf.Approximately(h, 0.0f);
        
        if (j && canJump)
        {
            canJump = false;
            CurrentMoveState = PlayerMoveState.Jumping;
        }
        else if (isMoving)
        {
            if (s) CurrentMoveState = PlayerMoveState.Sprint;
            else CurrentMoveState = c ? PlayerMoveState.CrouchRun : PlayerMoveState.Run;
        }
        else
        {
            CurrentMoveState = PlayerMoveState.Idle;
        }
    
        switch (CurrentMoveState)
        {
            case PlayerMoveState.Idle:
                Player.instance.Animator.Idle();
                //rb.velocity = Vector3.zero;
                return;
            
            case PlayerMoveState.CrouchIdle:
                Player.instance.Animator.Idle();
                //rb.velocity = Vector3.zero;
                return;
            
            case PlayerMoveState.Run:
                Player.instance.Animator.Run();
                rb.position += moveDir * (m * runSpeed * dTime);
                return;
            
            case PlayerMoveState.CrouchRun:
                Player.instance.Animator.Run();
                rb.position += moveDir * (m * crouchSpeed * dTime);
                return;
            
            case PlayerMoveState.Sprint:
                Player.instance.Animator.Sprint();
                rb.position += moveDir * (m * sprintSpeed * dTime);
                return;
            
            case PlayerMoveState.Jumping:
                //TODO: Check is grounded.
                Player.instance.Animator.Idle();
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                return;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
