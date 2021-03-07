using UnityEngine;

public class InteractableHealthPack : InteractableObject
{
    [SerializeField, Range(0,100)] private int healthAmount;
    [SerializeField, Range(0.0f, 1.0f)] private float maxHpPercentage;

    [SerializeField] private bool usePercentage;
    [SerializeField] private GameObject despawnEffect;

    private void Start()
    {
        if (usePercentage)
        {
            this.name = this.name + $"(+{(maxHpPercentage * 100):F0}% HP)";
        }
        else
        {
            this.name = this.name + $"(+{healthAmount:F0} HP)";
        }
    }
    public override string GetActionPronoun()
    {
        return "the";
    }

    public override string GetActionVerb()
    {
        return "get";
    }

    public override void Interact()
    {
        if (Player.Active.Health.CurrentHP == Player.Active.Health.MaxHP) return;

        if (usePercentage) Player.Active.Health.AddHealth(maxHpPercentage);
        else Player.Active.Health.AddHealth(healthAmount);
        Destroy(Instantiate(despawnEffect, transform.position, Quaternion.identity, null), 2.0f);
        Loot();
    }
}
