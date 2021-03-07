using EmeraldAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOfRuinsManager : MonoBehaviour
{
    [Header("OnObjectiveComplete Actions")]
    [SerializeField] private OnObjectiveCompleteWrapper[] onObjectiveCompleteActions;

    [Header("First Defence")]
    [SerializeField] private GameObject[] firstDefenceTargets;
    [SerializeField] private float spawnInterval = 2.0f;
    [SerializeField] private GameObject[] enemiesToSpawn;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private EmeraldAI.EmeraldAISystem[] availableOrcTargets;


    private Coroutine spawnChargingOrcsCoroutine;

    public void Awake()
    {
        foreach (var obj in onObjectiveCompleteActions)
            obj.Initialize();

        for (var i = 0; i < firstDefenceTargets.Length; i++)
            firstDefenceTargets[i].SetActive(false);

        spawnChargingOrcsCoroutine = StartCoroutine(SpawnChargingOrcs());
    }

    public void OnReachFirstDefense()
    {
        SpawnEnemiesInFirstDefence();
    }

    private void SpawnEnemiesInFirstDefence()
    {
        for (int i = 0; i < firstDefenceTargets.Length; i++)
        {
            firstDefenceTargets[i].SetActive(true);
        }
    }

    public void StopChargingOrcs()
    {
        SpawnEnemiesInFirstDefence();
        StopCoroutine(spawnChargingOrcsCoroutine);
    }

    public void OpenWayIntoRuins()
    {

    }

    private IEnumerator SpawnChargingOrcs()
    {
        var timer = 0.0f;

        while (Application.isPlaying)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                timer = 0.0f;

                var orcPrefabToSpawn = enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)];
                var spawnedOrc = Instantiate(orcPrefabToSpawn, spawnPosition.position, Quaternion.identity, null).GetComponent<EmeraldAISystem>();
                var cyborgToAttack = availableOrcTargets[Random.Range(0, availableOrcTargets.Length)];

                spawnedOrc.CurrentTarget = cyborgToAttack.transform;
            }
            yield return new WaitForEndOfFrame();
        }

    }
}
