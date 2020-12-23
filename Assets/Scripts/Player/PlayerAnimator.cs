using System;
using System.Collections;
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

    private UIManager uiManager;
    
    private float targetLayerWeight = 0.0f;
    private float startingLayerWeight = 0.0f;
    
    private void Start()
    {
        uiManager = UIManager.Instance;
        pe = Player.Instance.Equipment;
        StartCoroutine(AdjustLayerWeight());
    }

    private IEnumerator AdjustLayerWeight()
    {
        const float speed = 5.0f;
        const float tolerance = 0.001f;
        
        while (!pe.CurrentAnimator) yield return new WaitForEndOfFrame();
        
        while (true)
        {
            if (Math.Abs(pe.CurrentAnimator.GetLayerWeight(1) - targetLayerWeight) <= tolerance) yield return new WaitForEndOfFrame();
            else
            {
                var weight = Mathf.Lerp(startingLayerWeight, targetLayerWeight, Time.deltaTime * speed);
                pe.CurrentAnimator.SetLayerWeight(1, weight);
            }
            
            yield return new WaitForEndOfFrame();
        }
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
        if (status)
        {
            startingLayerWeight = pe.CurrentAnimator.GetLayerWeight(1);
            targetLayerWeight = 1.0f;
        }
        else 
        {
            startingLayerWeight = pe.CurrentAnimator.GetLayerWeight(1);
            targetLayerWeight = 0.0f;
        }
        
        uiManager.ToggleCrosshair(!status);

        if (pe.CurrentWeapon.hasScope) Player.Instance.Camera.ToggleZoom(status);
        
        pe.CurrentAnimator.SetBool(aimingAnimHash, status);
    }

    public void Fire()
    {
        pe.CurrentAnimator.ResetTrigger(attackAnimHash);
        pe.CurrentAnimator.SetTrigger(attackAnimHash);
    }
    
    [UsedImplicitly]
    public void Muzzle()
    {
        pe.Muzzle();
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
        pe.ResetMagazine();
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
