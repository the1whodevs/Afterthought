public class PlayerEquipment : PlayerWeaponAnimator
{
    private bool thrown = false;

    public override void Fire()
    {
        if (thrown) return;

        thrown = true;

        anim.ResetTrigger(throwHash);
        anim.SetTrigger(throwHash);
    }
}
