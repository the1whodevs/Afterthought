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

    // For shotgun, this starts the reload animation...
    public override void Reload()
    {
        anim.SetBool(isReloading, true);

        var lastAmmo = (pl.CurrentWeapon.weaponTypeData.ammoType.currentAmmo == 1);
        var lastForMag = (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity - 1);

        if (lastForMag || lastAmmo)
        {
            anim.ResetTrigger(lastReload);
            anim.SetTrigger(lastReload);
        }
    }

    public override void CancelReload()
    {
        base.CancelReload();

        anim.SetBool(isReloading, false);
    }

    // Called through animation event.
    public void ReloadBullet()
    {
        // Needed for fast reload speeds to work properly.
        if (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity || 
            pl.CurrentWeapon.weaponTypeData.ammoType.currentAmmo == 0) return;

        pl.CurrentWeapon.ammoInMagazine++;
        pl.CurrentWeapon.weaponTypeData.ammoType.currentAmmo--;

        var lastAmmo = (pl.CurrentWeapon.weaponTypeData.ammoType.currentAmmo == 1);
        var lastForMag = (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity - 1);

        if (lastForMag || lastAmmo)
        {
            anim.SetBool(isReloading, false);

            anim.ResetTrigger(lastReload);
            anim.SetTrigger(lastReload);
        }
        // Check if the lastReload animation called this.
        else if (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity ||
            pl.CurrentWeapon.weaponTypeData.ammoType.currentAmmo == 0)
        {
            // Setting this false in case only 1 bullet is reloaded.
            anim.SetBool(isReloading, false);

            pl.ResetMagazine();
        }
    }
}
