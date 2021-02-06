using UnityEngine;

public class WeaponAnimHelper : MonoBehaviour
{
    private PlayerAnimator pa;

    private void Awake()
    {
        pa = Player.Active.Animator;
    }

    public void Muzzle()
    {
        pa.Muzzle();
    }

    public void TryDealDamage()
    {
        pa.TryDealDamage();
    }

    public void ResetMagazine()
    {
        pa.ResetMagazine();
    }
}
