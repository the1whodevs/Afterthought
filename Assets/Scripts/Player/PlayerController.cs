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

    private Vector3 moveDir;
    
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
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        var j = Input.GetAxisRaw("Jump") >= 1.0f;
        var s = Input.GetAxisRaw("Sprint") >= 1.0f;
        var c = Input.GetAxisRaw("Crouch") >= 1.0f;
        var groundedPlayer = cc.isGrounded;

        if (groundedPlayer && moveDir.y < 0.0f) moveDir.y = 0.0f;
        
        var m = Mathf.Abs(v) + Mathf.Abs(h);
        if (m > 1.0f) m = 1.0f;
        
        if (v > 0.0f) moveDir += transform.forward;
        else if (v < 0.0f) moveDir += -transform.forward;
        
        if (h > 0.0f) moveDir += transform.right;
        else if (h < 0.0f) moveDir += -transform.right;
    
        moveDir.Normalize();
    
        var isMoving = !Mathf.Approximately(v, 0.0f) ||
                       !Mathf.Approximately(h, 0.0f);
        
        if (j && groundedPlayer)
        {
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

        var speedToUse = runSpeed;
        
        switch (CurrentMoveState)
        {
            case PlayerMoveState.Idle:
                pa.Idle();
                speedToUse = 0.0f;
                break;
            
            case PlayerMoveState.CrouchIdle:
                pa.Idle();
                speedToUse = 0.0f;
                break;
            
            case PlayerMoveState.Run:
                pa.Run();
                speedToUse = runSpeed;
                //cc.SimpleMove(moveDir * (m * runSpeed * dTime));
                //rb.MovePosition(rb.position + moveDir * (m * runSpeed * dTime));
                break;
            
            case PlayerMoveState.CrouchRun:
                pa.Run();
                speedToUse = crouchSpeed;
                //cc.SimpleMove(moveDir * (m * crouchSpeed * dTime));
                //rb.MovePosition(rb.position + moveDir * (m * crouchSpeed * dTime));
                break;
            
            case PlayerMoveState.Sprint:
                pa.Sprint();
                speedToUse = sprintSpeed;
                //cc.SimpleMove(moveDir * (m * sprintSpeed * dTime));
                //rb.MovePosition(rb.position + moveDir * (m * sprintSpeed * dTime));
                break;
            
            case PlayerMoveState.Jumping:
                // Player.instance.Animator.Idle();
                moveDir.y += Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
                //rb.SimpleMove(Vector3.up * jumpHeight);
                //rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        moveDir.y += Physics.gravity.y;
        cc.Move(moveDir * (speedToUse * dTime));
    }
}
