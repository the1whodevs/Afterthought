using Five.MoreMaths;
using System.Collections;
using UnityEngine;

public class LootCube : InteractableObject
{
    [SerializeField] private GameObject[] loot;
    [SerializeField] private GameObject despawnEffect;

    [SerializeField] private float throwDist = 1.0f;
    [SerializeField] private float throwY = 0.5f;
    [SerializeField] private float throwSpeed = 1.0f;
    [SerializeField] private Vector2 xOffset = new Vector2(-1.0f, 1.0f);

    [SerializeField] private float despawnEffectLifetime = 2.0f;

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

        for (var i = 0; i < loot.Length; i++)
        {
            var spawnedLoot = Instantiate(loot[i], transform.position, Quaternion.identity, null);

            // Start coroutine on the Player monobehaviour as this will be destroyed!
            Player.Active.StartCoroutine(ThrowLoot(spawnedLoot));
        }

        Destroy(gameObject);
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
