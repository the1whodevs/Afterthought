using UnityEngine;

public class WeaponAnimHelper : MonoBehaviour
{
    private PlayerAnimator pa;

    private void Awake()
    {
        pa = Player.Instance.Animator;
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
