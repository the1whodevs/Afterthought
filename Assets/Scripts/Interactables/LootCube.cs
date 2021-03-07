using Five.MoreMaths;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCube : InteractableObject
{
    [SerializeField] protected GameObject[] loot;
    [SerializeField] protected GameObject despawnEffect;

    [SerializeField] protected float throwDist = 1.0f;
    [SerializeField] protected float throwY = 0.5f;
    [SerializeField] protected float throwSpeed = 1.0f;
    [SerializeField] protected Vector2 xOffset = new Vector2(-1.0f, 1.0f);

    [SerializeField] protected float despawnEffectLifetime = 2.0f;

    protected List<GameObject> objectsToSpawn = new List<GameObject>();

    protected virtual void Start()
    {
        for (var i = 0; i < loot.Length; i++)
        {
            var spawnedLoot = Instantiate(loot[i], transform.position, Quaternion.identity, null);

            objectsToSpawn.Add(spawnedLoot);
            spawnedLoot.SetActive(false);
        }
    }

    public override string GetActionPronoun()
    {
        return "the";
    }

    public override string GetActionVerb()
    {
        return "open";
    }

    public override void Interact()
    {
        if (despawnEffect) Destroy(Instantiate(despawnEffect, transform.position, Quaternion.identity, null), despawnEffectLifetime);

        for (var i = 0; i < objectsToSpawn.Count; i++)
        {
            var spawnedLoot = objectsToSpawn[i];
            spawnedLoot.transform.position = transform.position;
            spawnedLoot.SetActive(true);

            // Start coroutine on the Player monobehaviour as this will be destroyed!
            Player.Active.StartCoroutine(ThrowLoot(spawnedLoot));
        }

        Loot();
    }

    public override void SetLootStatus(bool status)
    {
        base.SetLootStatus(status);

        if (status) Interact();
    }

    private IEnumerator ThrowLoot(GameObject lootToThrow)
    {
        var t = 0.0f;

        var startPos = lootToThrow.transform.position;
        var offset = Random.Range(xOffset.x, xOffset.y);
        var endPos = startPos + transform.forward * throwDist + transform.right * offset;

        while (t <= 1.0f)
        {
            t += Time.deltaTime * throwSpeed;

            if (!lootToThrow) break;

            lootToThrow.transform.position = MoreMaths.Blerp(startPos, endPos, throwY, t);

            yield return new WaitForEndOfFrame();
        }
    }
}
