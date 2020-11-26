using UnityEngine;

/// <summary>
/// This should only be access through Player.instance.Animator!
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int RunAnimHash = Animator.StringToHash("isRunning");
    private int SprintAnimHash = Animator.StringToHash("isSprinting");
    private int AttackAnimHash = Animator.StringToHash("attack");

    public void Run()
    {
       animator.SetBool(RunAnimHash, true);
       animator.SetBool(SprintAnimHash, false);
    }

    public void Sprint()
    {
        animator.SetBool(RunAnimHash, false);
        animator.SetBool(SprintAnimHash, true);
    }

    public void Idle()
    {
        animator.SetBool(RunAnimHash, false);
        animator.SetBool(SprintAnimHash, false);
    }

    public void Fire()
    {
        animator.ResetTrigger(AttackAnimHash);
        animator.SetTrigger(AttackAnimHash);
    }
}
