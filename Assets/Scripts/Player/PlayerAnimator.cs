using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This should only be access through Player.instance.Animator!
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    public float ADS_Speed => adsSpeed;
    public float WeaponSwitch_Speed => weaponSwitchSpeed;
    public float Reload_Speed => reloadSpeed;

    [SerializeField] private float adsSpeed = 10.0f;
    [SerializeField] private float weaponSwitchSpeed = 1.0f;
    [SerializeField] private float reloadSpeed = 1.0f;

    private bool hasInitialized = false;

    private PlayerMelee meleeAnims;
    private PlayerPistol pistolAnims;
    private PlayerShotgun shotgunAnims;
    private PlayerEquipment equipmentAnims;

    private PlayerWeaponAnimator activeAnim;
    private PlayerDamage pd;
    private PlayerController pc;
    private PlayerLoadout pl;

    private UIManager uiManager;
  
    public void Init()
    {
        meleeAnims = GetComponent<PlayerMelee>();
        meleeAnims.Init();

        pistolAnims = GetComponent<PlayerPistol>();
        pistolAnims.Init();

        shotgunAnims = GetComponent<PlayerShotgun>();
        shotgunAnims.Init();

        equipmentAnims = GetComponent<PlayerEquipment>();
        equipmentAnims.Init();

        uiManager = UIManager.Active;
        pl = Player.Active.Loadout;
        pc = Player.Active.Controller;
        pd = Player.Active.Damage;

        pl.OnWeaponEquipped += OnWeaponEquipped;
        pl.OnEquipmentEquipped += OnEquipEquipment;
        pc.OnReloadCancel += OnReloadCancel;

        hasInitialized = true;
    }

    private void Update()
    {
        if (!hasInitialized) return;

        if (pc.IsInUI || !activeAnim) return;

        activeAnim.Animate();
    }

    public void AimDownSights(bool status)
    {
        if (pl.CurrentWeapon.weaponType != WeaponData.WeaponType.Melee)
        {
            uiManager.ToggleCrosshair(!status);
            uiManager.ToggleHealthBar(!status);
        }

        if (pl.CurrentWeapon.hasScope) Player.Active.Camera.ToggleZoom(status);

        if (!activeAnim) return;

        if (!activeAnim.Equals(meleeAnims)) activeAnim.ADSCheck(status);
    }

    public void Fire()
    {
        activeAnim.Fire();
    }

    public void Cook()
    {
        if (activeAnim.Equals(equipmentAnims)) equipmentAnims.Cook();
    }

    public void AltFire()
    {
        var currentWeapon = pl.CurrentWeapon;

        switch (currentWeapon.weaponAnimType)
        {
            case PlayerWeaponAnimator.WeaponAnimatorType.Melee:
                meleeAnims.AltFire();
                break;
        }
    }

    public void Unequip()
    {
        activeAnim.SwitchWeapon();
    }

    public void Reload()
    {
        if (!activeAnim.Equals(meleeAnims)) activeAnim.Reload();
    }

    public void ReloadBullet()
    {
        if (activeAnim.Equals(shotgunAnims))
        {
            shotgunAnims.ReloadBullet();
            pl.SetAmmoUI();
        }
    }

    public void Muzzle()
    {
        pd.Muzzle();
    }
    
    // Called by the animation event within each attack animation.
    public void TryDealDamage()
    {
        pd.TryDealDamage();
    }
    
    // Called by the animation event within each reload animation.
    public void ResetMagazine()
    {
        pl.ResetMagazine();
    }

    private void OnReloadCancel()
    {
        if (!activeAnim) return;

        if (activeAnim.Equals(meleeAnims)) return;

        activeAnim.CancelReload();
        pl.ReloadCancel();
    }

    private void OnEquipEquipment(EquipmentData equippedEquipment)
    {
        if (!activeAnim) return;

        activeAnim.DeactivateAnim();

        activeAnim = equipmentAnims;
        equipmentAnims.ResetThrow();

        activeAnim.ActivateAnim();
    }

    private void OnWeaponEquipped(WeaponData weapon)
    {
        if (activeAnim) activeAnim.DeactivateAnim();

        switch (weapon.weaponAnimType)
        {
            case PlayerWeaponAnimator.WeaponAnimatorType.Melee:
                activeAnim = meleeAnims;
                break;

            case PlayerWeaponAnimator.WeaponAnimatorType.Pistol:
                activeAnim = pistolAnims;
                break;

            case PlayerWeaponAnimator.WeaponAnimatorType.Shotgun:
                activeAnim = shotgunAnims;
                break;
        }

        activeAnim.ActivateAnim();
    }
}
