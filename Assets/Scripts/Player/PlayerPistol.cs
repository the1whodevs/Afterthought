public class PlayerPistol : PlayerWeaponAnimator
{
    public override void Animate()
    {
        SetAnimParameters();

        var currentState = pc.CurrentMoveState;
        
        var sprinting = currentState == PlayerController.PlayerMoveState.Sprint;
        var moving = currentState == PlayerController.PlayerMoveState.CrouchRun ||
            currentState == PlayerController.PlayerMoveState.Run;

        anim.SetBool(isSprinting, sprinting);
        anim.SetBool(isMoving, moving);

        startingLayerWeight = anim.GetLayerWeight(1);
    }

    public override void ADSCheck(bool status)
    {
        var sprinting = pc.CurrentMoveState == PlayerController.PlayerMoveState.Sprint;

        var shouldGetInADS = (status && !sprinting);

        if (shouldGetInADS)
        {
            targetLayerWeight = 1;
            anim.SetBool(isADS, true);
        }
        else
        {
            targetLayerWeight = 0;
            anim.SetBool(isADS, false);
        }
    }

    protected override void SetAnimParameters() 
    {
        if (!pa) pa = Player.Active.Animator;
        if (!pl) pl = Player.Active.Loadout;

        anim.SetFloat(switch_speed, pa.WeaponSwitch_Speed);
        anim.SetFloat(attack_speed, pl.CurrentWeapon.fireRate);
        anim.SetFloat(reload_speed, pl.CurrentWeapon.reloadSpeed);
    }
}
