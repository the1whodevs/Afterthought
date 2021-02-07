public class PlayerEquipment : PlayerWeaponAnimator
{
    private bool thrown = false;

    public void ResetThrow()
    {
        thrown = false;
    }

    // This will only be called if the player was cooking the grenade!
    public override void Fire()
    {
        if (thrown) return;

        thrown = true;

        anim.SetBool(isCooking, false);
    }

    public void Cook()
    {
        if (thrown) return;

        anim.SetBool(isCooking, true);
    }
}
