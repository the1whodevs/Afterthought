public class WeaponDisplay : ItemDisplay<WeaponData>
{
    public int RelatedSlot = 0;

    public void SetLoadoutWeapon()
    {
        LoadoutEditor.Instance.SetLoadoutWeapon(itemToDisplay, RelatedSlot);
    }
}
