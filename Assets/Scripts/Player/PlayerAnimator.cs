using System;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// This should only be access through Player.instance.Animator!
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    public readonly  int runAnimHash = Animator.StringToHash("isRunning");
    public readonly  int sprintAnimHash = Animator.StringToHash("isSprinting");
    public readonly  int aimingAnimHash = Animator.StringToHash("isAiming");
    public readonly int attackAnimHash = Animator.StringToHash("attack");
    public readonly  int reloadAnimHash = Animator.StringToHash("reload");
    public readonly  int unequipAnimHash = Animator.StringToHash("unequip");

    private PlayerEquipment pe;

    private void Start()
    {
        pe = Player.instance.Equipment;
    }

    public void Run()
    {
        pe.CurrentAnimator.SetBool(runAnimHash, true);
        pe.CurrentAnimator.SetBool(sprintAnimHash, false);
    }

    public void Sprint()
    {
        pe.CurrentAnimator.SetBool(runAnimHash, true);
        pe.CurrentAnimator.SetBool(sprintAnimHash, true);
        pe.CurrentAnimator.SetBool(aimingAnimHash, false);
        pe.CurrentAnimator.SetBool(attackAnimHash, false);
    }

    public void Idle()
    {
        pe.CurrentAnimator.SetBool(runAnimHash, false);
        pe.CurrentAnimator.SetBool(sprintAnimHash, false);
    }

    public void AimDownSights(bool status)
    {
        if (status) pe.CurrentAnimator.SetLayerWeight(1, 1.0f);
        else pe.CurrentAnimator.SetLayerWeight(1, 0.0f);
        
        pe.CurrentAnimator.SetBool(aimingAnimHash, status);
    }

    public void Fire()
    {
        pe.CurrentAnimator.SetBool(attackAnimHash, true);
    }
    
    [UsedImplicitly]
    public void Muzzle()
    {
        var firePoint = pe.CurrentWeaponObject.transform.Find("FirePoint");

        const float muzzleLifetime = 2.0f;
        Destroy(Instantiate(pe.CurrentWeapon.muzzleEffect, firePoint.position, firePoint.rotation, null), muzzleLifetime);
    }
    
    [UsedImplicitly]
    // Called by the animation event within each attack animation.
    public void TryDealDamage()
    {
        pe.TryDealDamage();
    }

    public void Reload()
    {
        pe.CurrentAnimator.ResetTrigger(reloadAnimHash);
        pe.CurrentAnimator.SetTrigger(reloadAnimHash);
        pe.CurrentAnimator.SetBool(aimingAnimHash, false);
    }
    
    [UsedImplicitly]
    // Called by the animation event within each reload animation.
    public void ResetMagazine()
    {
        pe.CurrentWeapon.ReloadMag(ref pe.ammoAvailable);
    }

    public void Unequip()
    {
        pe.CurrentAnimator.ResetTrigger(unequipAnimHash);
        pe.CurrentAnimator.SetTrigger(unequipAnimHash);
        pe.CurrentAnimator.SetBool(aimingAnimHash, false);
    }

    public void ResetAttack()
    {
        pe.CurrentAnimator.SetBool(attackAnimHash, false);
    }
}
