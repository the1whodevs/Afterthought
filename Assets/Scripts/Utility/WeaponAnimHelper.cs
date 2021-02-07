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

    // Called from reload animation's event.
    public void ResetMagazine()
    {
        pa.ResetMagazine();
    }

    // Called by single-bullet reload animations. (i.e. shotgun)
    public void ReloadBullet()
    {
        pa.ReloadBullet();
    }
}
