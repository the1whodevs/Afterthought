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
        Debug.Log("4> PlayerShotgun reload...");

        anim.SetBool(isReloading, true);
        Debug.LogError("PAUSE");

        var lastAmmo = (pl.CurrentWeapon.ammoType.currentAmmo == 1);
        var lastForMag = (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity - 1);

        if (lastForMag || lastAmmo)
        {
            Debug.LogFormat("4a> lastForMag({0}) OR lastAmmo({1})",
                lastForMag,
                lastAmmo);

            anim.ResetTrigger(lastReload);
            anim.SetTrigger(lastReload);
        }
    }

    // Called through animation event.
    public void ReloadBullet()
    {
        Debug.Log("5> ReloadBullet");

        pl.CurrentWeapon.ammoInMagazine++;
        pl.CurrentWeapon.ammoType.currentAmmo--;

        var lastAmmo = (pl.CurrentWeapon.ammoType.currentAmmo == 1);
        var lastForMag = (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity - 1);

        if (lastForMag || lastAmmo)
        {
            Debug.LogFormat("5a> lastForMag({0}) OR lastAmmo({1})",
                lastForMag,
                lastAmmo);

            anim.SetBool(isReloading, false);

            anim.ResetTrigger(lastReload);
            anim.SetTrigger(lastReload);
        }
        // Check if the lastReload animation called this.
        else if (pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity ||
            pl.CurrentWeapon.ammoType.currentAmmo == 0)
        {
            Debug.LogFormat("5b> magFull({0}) OR noAmmoLeft({1})",
                pl.CurrentWeapon.ammoInMagazine == pl.CurrentWeapon.magazineCapacity,
                pl.CurrentWeapon.ammoType.currentAmmo == 0);

            // Setting this false in case only 1 bullet is reloaded.
            anim.SetBool(isReloading, false);

            pl.ResetMagazine();
        }
    }
}
