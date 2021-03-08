using Five.MoreMaths;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCube : InteractableObject
{
    [SerializeField] protected LootData[] loot;
    [SerializeField] protected GameObject despawnEffect;

    [SerializeField] protected float throwDist = 1.0f;
    [SerializeField] protected float throwY = 0.5f;
    [SerializeField] protected float throwSpeed = 1.0f;
    [SerializeField] protected float throwPosYoffset = 0.5f;

    [SerializeField] protected Vector2 xOffset = new Vector2(-1.0f, 1.0f);

    [SerializeField] protected float despawnEffectLifetime = 2.0f;

    protected List<GameObject> objectsToSpawn = new List<GameObject>();

    [System.Serializable] 
    protected class LootData
    {
        public float lootChance = 0.5f;
        public GameObject lootPrefab;
    }

    protected virtual void Start()
    {
        for (var i = 0; i < loot.Length; i++)
        {
            var spawnedLoot = Instantiate(loot[i].lootPrefab, transform.position, Quaternion.identity, null);

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

        var chance = Random.Range(0.0f, 1.0f);

        for (var i = 0; i < loot.Length; i++)
        {
            if (chance <= loot[i].lootChance)
            {
                var spawnedLoot = objectsToSpawn[i];
                spawnedLoot.transform.position = transform.position;
                spawnedLoot.SetActive(true);

                // Start coroutine on the Player monobehaviour as this will be destroyed!
                Player.Active.StartCoroutine(ThrowLoot(spawnedLoot));
            }
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
        var endPos = startPos + transform.forward * throwDist + transform.right * offset + Vector3.up * throwPosYoffset;

        while (t <= 1.0f)
        {
            t += Time.deltaTime * throwSpeed;

            if (!lootToThrow) break;

            lootToThrow.transform.position = MoreMaths.Blerp(startPos, endPos, throwY, t);

            yield return new WaitForEndOfFrame();
        }
    }
}
