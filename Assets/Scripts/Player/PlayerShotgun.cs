using UnityEngine;

public class PlayerShotgun : PlayerWeaponAnimator
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

        targetLayerWeight = (status && !sprinting) ? 1 : 0;
    }

    protected override void SetAnimParameters()
    {
        if (!pa) pa = Player.Active.Animator;
        if (!pl) pl = Player.Active.Loadout;

        anim.SetFloat(switch_speed, pa.WeaponSwitch_Speed);
        anim.SetFloat(attack_speed, pl.CurrentWeapon.fireRate);
        anim.SetFloat(reload_speed, pl.CurrentWeapon.reloadSpeed);
    }

    // For shotgun, this starts the reload animation...
    public override void Reload()
    {
        anim.SetBool(isReloading, true);

        var lastAmmo = (pl.CurrentWeapon.ammoType.currentAmmo == 1);
        var lastForMag = (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity - 1);

        Debug.LogFormat("LastForMag: {0} // LastAmmo: {1} // CurrentAmmo: {2} // AmmoInMag: {3} // MagCapacity: {4}",
            lastForMag, lastAmmo, pl.CurrentWeapon.ammoType.currentAmmo, pl.CurrentWeapon.ammoInMagazine, pl.CurrentWeapon.magazineCapacity);


        if (lastForMag || lastAmmo)
        {
            anim.SetBool(isReloading, false);

            anim.ResetTrigger(lastReload);
            anim.SetTrigger(lastReload);
        }
    }

    // Called through animation event.
    public void ReloadBullet()
    {
        pl.CurrentWeapon.ammoInMagazine++;
        pl.CurrentWeapon.ammoType.currentAmmo--;

        var lastAmmo = (pl.CurrentWeapon.ammoType.currentAmmo == 1);
        var lastForMag = (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity - 1);

        Debug.LogFormat("LastForMag: {0} // LastAmmo: {1} // CurrentAmmo: {2} // AmmoInMag: {3} // MagCapacity: {4}",
            lastForMag, lastAmmo, pl.CurrentWeapon.ammoType.currentAmmo, pl.CurrentWeapon.ammoInMagazine, pl.CurrentWeapon.magazineCapacity);

        if (lastForMag || lastAmmo)
        {
            anim.SetBool(isReloading, false);

            anim.ResetTrigger(lastReload);
            anim.SetTrigger(lastReload);
        }
    }
}
