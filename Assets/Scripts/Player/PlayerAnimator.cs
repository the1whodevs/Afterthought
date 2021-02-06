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

    private PlayerMelee meleeAnims;
    private PlayerPistol pistolAnims;
    private PlayerShotgun shotgunAnims;
    private PlayerEquipment equipmentAnims;

    private PlayerWeaponAnimator activeAnim;
    private PlayerDamage pd;
    private PlayerController pc;
    private PlayerLoadout pl;

    private UIManager uiManager;
  
    private void Start()
    {
        meleeAnims = GetComponent<PlayerMelee>();
        pistolAnims = GetComponent<PlayerPistol>();
        shotgunAnims = GetComponent<PlayerShotgun>();
        equipmentAnims = GetComponent<PlayerEquipment>();

        uiManager = UIManager.Active;
        pl = Player.Active.Loadout;
        pc = Player.Active.Controller;
        pl.OnWeaponEquipped += OnWeaponEquipped;
        pd = Player.Active.Damage;
    }

    private void Update()
    {
        if (pc.IsInUI || !activeAnim) return;

        activeAnim.Animate();
    }

    public void AimDownSights(bool status)
    {       
        uiManager.ToggleCrosshair(!status);
        uiManager.ToggleHealthBar(!status);

        if (pl.CurrentWeapon.hasScope) Player.Active.Camera.ToggleZoom(status);

        if (!activeAnim.Equals(meleeAnims)) activeAnim.ADSCheck(status);
        // pl.CurrentAnimator.SetBool(aimingAnimHash, status);
    }

    public void Fire()
    {
        activeAnim.Fire();
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

            case PlayerWeaponAnimator.WeaponAnimatorType.Equipment:
                activeAnim = equipmentAnims;
                break;
        }

        activeAnim.ActivateAnim();
    }
}
