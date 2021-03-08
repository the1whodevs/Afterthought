public class MobLoot : LootCube
{
    private EmeraldAI.EmeraldAISystem attachedSystem;

    protected override void Start()
    {
        base.Start();

        attachedSystem = GetComponent<EmeraldAI.EmeraldAISystem>();

        name = attachedSystem.AIName;
    }

    public override string GetActionVerb()
    {
        return "search";
    }

    public override string GetActionPronoun()
    {
        return "";
    }

    public override void Interact()
    {
        if (attachedSystem.CurrentHealth <= 0)
        {
            base.Interact();
        }
    }

    public override void Loot()
    {
        IsLooted = true;
    }
}
