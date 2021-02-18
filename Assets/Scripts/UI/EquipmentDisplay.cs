public class EquipmentDisplay : ItemDisplay<EquipmentData>
{
    public int RelatedSlot = 0;

    public void SetLoadoutEquipment()
    {
        LoadoutEditor.Active.SetLoadoutEquipment(itemToDisplay, RelatedSlot);
    }
}
