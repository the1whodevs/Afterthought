using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMoveState { Idle, CrouchIdle, Run, CrouchRun, Sprint, Jumping }

    public PlayerMoveState CurrentMoveState = PlayerMoveState.Idle;
    
    [SerializeField] private float runSpeed = 10.0f;
    [SerializeField] private float crouchSpeed = 7.0f;
    [SerializeField] private float sprintSpeed = 20.0f;
    [SerializeField] private float jumpHeight = 15.0f;

    private bool canJump = true;
    
    private Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        var j = Input.GetAxis("Jump") > 0.0f;
        var s = Input.GetAxis("Sprint") > 0.0f;
        var c = Input.GetAxis("Crouch") > 0.0f;

        var moveDir = Vector3.zero;

        if (v != 0.0f) moveDir += transform.forward;
        if (h != 0.0f) moveDir += transform.right;

        moveDir.Normalize();

        if (j && canJump)
        {
            canJump = false;
            CurrentMoveState = PlayerMoveState.Jumping;
        }
        else if (c && !s)
        {
            if (v != 0.0f || h != 0.0f) CurrentMoveState = PlayerMoveState.CrouchRun;
            else CurrentMoveState = PlayerMoveState.CrouchIdle;
        }
        else if (v != 0.0f || h != 0.0f)
        {
            CurrentMoveState = s ? PlayerMoveState.Sprint : PlayerMoveState.Run;
        }
        else CurrentMoveState = PlayerMoveState.Idle;

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
                rb.velocity = transform.forward * runSpeed;
                return;
            
            case PlayerMoveState.CrouchRun:
                Player.instance.Animator.Run();
                rb.velocity = transform.forward * crouchSpeed;
                return;
            
            case PlayerMoveState.Sprint:
                Player.instance.Animator.Sprint();
                rb.velocity = transform.forward * sprintSpeed;
                return;
            
            case PlayerMoveState.Jumping:
                Player.instance.Animator.Idle();
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                return;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
