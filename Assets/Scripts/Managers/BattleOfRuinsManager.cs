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
    [SerializeField] private GameObject gateEffectToSpawn;
    [SerializeField] private GameObject gateToDestroy;
    [SerializeField] private float secondsToWaitForCover;

    [Header("Second Defence")]
    [SerializeField] private GameObject[] secondDefenceTargets;

    [Header("Ruins Defence")]
    [SerializeField] private GameObject[] ruinsDefenceTargets;

    [Header("Stairs Defence")]
    [SerializeField] private GameObject[] stairsDefenceTargets;


    private Coroutine spawnChargingOrcsCoroutine;

    public void Start()
    {
        foreach (var obj in onObjectiveCompleteActions)
            obj.Initialize();

        spawnChargingOrcsCoroutine = StartCoroutine(SpawnChargingOrcs());

        for (var i = 0; i < firstDefenceTargets.Length; i++)
            firstDefenceTargets[i].SetActive(false);

        for (var i = 0; i < secondDefenceTargets.Length; i++)
            secondDefenceTargets[i].SetActive(false);

        for (var i = 0; i < ruinsDefenceTargets.Length; i++)
            ruinsDefenceTargets[i].SetActive(false);

        for (var i = 0; i < stairsDefenceTargets.Length; i++)
            stairsDefenceTargets[i].SetActive(false);

        gateEffectToSpawn.SetActive(false);
    }

    public void OnReachFirstDefense()
    {
        SpawnEnemiesInFirstDefence();
    }

    public void OnReachFirstRuins()
    {
        SpawnEnemiesInRuinsDefence();
    }

    public void OnReachStairsDestination()
    {
        SpawnStairEnemiesDefence();
    }

    private void SpawnEnemiesInFirstDefence()
    {
        for (int i = 0; i < firstDefenceTargets.Length; i++)
        { 
            firstDefenceTargets[i].SetActive(true);
            firstDefenceTargets[i].GetComponent<EmeraldAISystem>().EmeraldEventsManagerComponent.ResetAI();
        }
    }

    private void SpawnEnemiesInSecondDefence()
    {
        for (int i = 0; i < secondDefenceTargets.Length; i++)
        {
            secondDefenceTargets[i].SetActive(true);
            secondDefenceTargets[i].GetComponent<EmeraldAISystem>().EmeraldEventsManagerComponent.ResetAI();
        }
    }

    private void SpawnEnemiesInRuinsDefence()
    {
        for (int i = 0; i < ruinsDefenceTargets.Length; i++)
        {
            ruinsDefenceTargets[i].SetActive(true);
            ruinsDefenceTargets[i].GetComponent<EmeraldAISystem>().EmeraldEventsManagerComponent.ResetAI();
        }
    }

    private void SpawnStairEnemiesDefence()
    {
        for (int i = 0; i < stairsDefenceTargets.Length; i++)
        {
            stairsDefenceTargets[i].SetActive(true);
            stairsDefenceTargets[i].GetComponent<EmeraldAISystem>().EmeraldEventsManagerComponent.ResetAI();
        }
    }

    public void StopChargingOrcs()
    {
        SpawnEnemiesInFirstDefence();
        StopCoroutine(spawnChargingOrcsCoroutine);
    }

    public void OpenWayIntoRuins()
    {
        SpawnEnemiesInSecondDefence();
        StartCoroutine(GateEffect());
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

    private IEnumerator GateEffect()
    {
        yield return new WaitForSeconds(secondsToWaitForCover);

        gateEffectToSpawn.SetActive(true);
        gateToDestroy.SetActive(false);
    }
}
