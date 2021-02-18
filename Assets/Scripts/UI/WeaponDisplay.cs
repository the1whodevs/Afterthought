public class WeaponDisplay : ItemDisplay<WeaponData>
{
    public int RelatedSlot = 0;

    public void SetLoadoutWeapon()
    {
        LoadoutEditor.Active.SetLoadoutWeapon(itemToDisplay, RelatedSlot);
    }
}
