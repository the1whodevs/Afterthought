public class TalentDisplay : ItemDisplay<TalentData>
{
    public int RelatedSlot = 0;

    public void SetLoadoutTalent()
    {
        LoadoutEditor.Active.SetLoadoutTalent(itemToDisplay, RelatedSlot);
    }
}
