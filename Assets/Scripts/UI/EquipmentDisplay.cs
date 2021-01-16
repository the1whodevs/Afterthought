public class EquipmentDisplay : ItemDisplay<EquipmentData>
{
    public int RelatedSlot = 0;

    public void SetLoadoutEquipment()
    {
        LoadoutEditor.Instance.SetLoadoutEquipment(itemToDisplay, RelatedSlot);
    }
}
