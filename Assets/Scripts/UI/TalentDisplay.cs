public class TalentDisplay : ItemDisplay<TalentData>
{
    public int RelatedSlot = 0;

    public void SetLoadoutTalent()
    {
        LoadoutEditor.Instance.SetLoadoutTalent(itemToDisplay, RelatedSlot);
    }
}
