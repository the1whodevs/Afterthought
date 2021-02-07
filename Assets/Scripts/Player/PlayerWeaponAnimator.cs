using System.Collections;
using UnityEngine;

public class PlayerWeaponAnimator : MonoBehaviour
{
    public enum WeaponAnimatorType { Melee, Pistol, Shotgun, Equipment }

    protected Animator anim;

    protected PlayerLoadout pl;
    protected PlayerController pc;
    protected PlayerAnimator pa;

    protected bool isAnimating;
    protected bool isAds;

    protected float targetLayerWeight = 0;
    protected float startingLayerWeight = 0;

    protected readonly int attack = Animator.StringToHash("attack");
    protected readonly int isSprinting = Animator.StringToHash("isSprinting");
    protected readonly int isMoving = Animator.StringToHash("isMoving");
    protected readonly int isReloading = Animator.StringToHash("isReloading");
    protected readonly int isFiring = Animator.StringToHash("isFiring");
    protected readonly int isADS = Animator.StringToHash("isADS");
    protected readonly int switch_weapon = Animator.StringToHash("switch_weapon");
    protected readonly int switch_speed = Animator.StringToHash("switch_speed");
    protected readonly int attack_speed = Animator.StringToHash("attack_speed");
    protected readonly int reload_speed = Animator.StringToHash("reload_speed");
    protected readonly int reload = Animator.StringToHash("reload");
    protected readonly int lastReload = Animator.StringToHash("last_reload");
    protected readonly int attackNum = Animator.StringToHash("attackNum");
    protected readonly int throwHash = Animator.StringToHash("throw");

    public virtual void Init()
    {
        pl = Player.Active.Loadout;
        pc = Player.Active.Controller;

        StartCoroutine(AdjustLayerWeight());
    }

    protected IEnumerator AdjustLayerWeight()
    {
        const float tolerance = 0.001f;

        while (!anim) yield return new WaitForEndOfFrame();

        while (true)
        {
            if (isAnimating)
            {
                if (pl.UsingEquipment || Player.Active.Controller.IsInUI || !anim) yield return new WaitForEndOfFrame();
                else if (Mathf.Abs(anim.GetLayerWeight(1) - targetLayerWeight) <= tolerance) 
                {
                    var roundedInt = Mathf.RoundToInt(targetLayerWeight);
                    pl.CurrentAnimator.SetLayerWeight(1, roundedInt);
                    yield return new WaitForEndOfFrame(); 
                }
                else
                {
                    var weight = Mathf.Lerp(startingLayerWeight, targetLayerWeight, Time.deltaTime * pa.ADS_Speed);
                    pl.CurrentAnimator.SetLayerWeight(1, weight);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public virtual void Fire()
    {
        anim.ResetTrigger(attack);
        anim.SetTrigger(attack);
    }

    public virtual void Reload()
    {
        anim.ResetTrigger(reload);
        anim.SetTrigger(reload);
    }

    public virtual void CancelReload()
    {
        anim.ResetTrigger(reload);
    }

    public virtual void SwitchWeapon()
    {
        anim.ResetTrigger(switch_weapon);
        anim.SetTrigger(switch_weapon);
    }

    public void ActivateAnim()
    {
        anim = pl.CurrentAnimator;
        isAnimating = true;
    }

    public void DeactivateAnim()
    {
        isAnimating = false;
        anim = null;
    }

    public virtual void Animate()
    {
        
    }

    public virtual void ADSCheck(bool status)
    {

    }

    protected virtual void SetAnimParameters() { }
}
