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
    
    private void Update()
    {
        var v = Input.GetKey(KeyCode.W) ? 1.0f : Input.GetKey(KeyCode.S) ? -1.0f : 0.0f; //Input.GetAxis("Vertical");
        var h = Input.GetKey(KeyCode.D) ? 1.0f : Input.GetKey(KeyCode.A) ? -1.0f : 0.0f; //Input.GetAxis("Horizontal");
        var j = Input.GetKey(KeyCode.Space); //Input.GetAxis("Jump") > 0.0f;
        var s = Input.GetKey(KeyCode.LeftShift); //Input.GetAxis("Sprint") > 0.0f;
        var c = Input.GetKey(KeyCode.LeftControl); //Input.GetAxis("Crouch") > 0.0f;

        var moveDir = Vector3.zero;

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
                rb.position += moveDir * (runSpeed * Time.deltaTime);
                return;
            
            case PlayerMoveState.CrouchRun:
                Player.instance.Animator.Run();
                rb.position += moveDir * (crouchSpeed * Time.deltaTime);
                return;
            
            case PlayerMoveState.Sprint:
                Player.instance.Animator.Sprint();
                rb.position += moveDir * (sprintSpeed * Time.deltaTime);
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
