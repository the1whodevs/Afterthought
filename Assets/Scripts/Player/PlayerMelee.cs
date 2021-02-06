public class PlayerMelee : PlayerWeaponAnimator
{
    public override void Init()
    {
        pl = Player.Active.Loadout;
        pc = Player.Active.Controller;
    }

    public override void Animate()
    {
        SetAnimParameters();

        var currentState = pc.CurrentMoveState;

        var sprinting = currentState == PlayerController.PlayerMoveState.Sprint;
        var moving = currentState == PlayerController.PlayerMoveState.CrouchRun ||
            currentState == PlayerController.PlayerMoveState.Run;

        anim.SetBool(isSprinting, sprinting);
        anim.SetBool(isMoving, moving);
    }

    protected override void SetAnimParameters()
    {
        if (!pa) pa = Player.Active.Animator;
        if (!pl) pl = Player.Active.Loadout;

        anim.SetFloat(switch_speed, pa.WeaponSwitch_Speed);
        anim.SetFloat(attack_speed, pl.CurrentWeapon.fireRate);
    }

    public override void Fire()
    {
        anim.SetInteger(attackNum, 0);
        anim.ResetTrigger(attack);
        anim.SetTrigger(attack);
    }

    public void AltFire()
    {
        anim.SetInteger(attackNum, 1);
        anim.ResetTrigger(attack);
        anim.SetTrigger(attack);
    }
}
