using EmeraldAI;
using System.Collections;
using UnityEngine;

public class BattleOfRuinsManager : MonoBehaviour
{
    [Header("OnObjectiveComplete Actions")]
    [SerializeField] private OnObjectiveCompleteWrapper[] onObjectiveCompleteActions;

    [Header("First Defence")]
    [SerializeField] private GameObject[] firstDefenceTargets;
    [SerializeField] private float spawnInitialDelay = 2.5f;
    [SerializeField] private float spawnInterval = 2.0f;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private EmeraldAISystem[] chargingOrcs;
    [SerializeField] private EmeraldAI.EmeraldAISystem[] availableOrcTargets;
    [SerializeField] private GameObject gateEffectToSpawn;
    [SerializeField] private GameObject gateToDestroy;
    [SerializeField] private GameObject portalToHub;
    [SerializeField] private float secondsToWaitForCover;

    [Header("Second Defence")]
    [SerializeField] private GameObject[] secondDefenceTargets;

    [Header("Ruins Defence")]
    [SerializeField] private GameObject[] ruinsDefenceTargets;

    [Header("Stairs Defence")]
    [SerializeField] private GameObject[] stairsDefenceTargets;

    [Header("Last Defence")]
    [SerializeField] private GameObject[] lastDefenceTargets;
    [SerializeField] private GameObject[] companionCyborgs;

    [Header("Boss Fight")]
    [SerializeField] private GameObject miniBoss;

    private Coroutine spawnChargingOrcsCoroutine;

    public void Start()
    {
        portalToHub.SetActive(false);

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

        for (var i = 0; i < lastDefenceTargets.Length; i++)
            lastDefenceTargets[i].SetActive(false);

        for (var i = 0; i < companionCyborgs.Length; i++)
            companionCyborgs[i].SetActive(false);

        miniBoss.SetActive(false);

        gateEffectToSpawn.SetActive(false);
    }

    public void OnKilledEliteOrc()
    {
        portalToHub.SetActive(true);
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

    public void OnKillStairTargets()
    {
        SpawnLastDefenseEnemies();
        SpawnCompanionAI();
    }

    public void OnKillLastDefenceTargets()
    {
        SpawnMiniBoss();
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

    private void SpawnLastDefenseEnemies()
    {
        for (int i = 0; i < lastDefenceTargets.Length; i++)
        {
            lastDefenceTargets[i].SetActive(true);
            lastDefenceTargets[i].GetComponent<EmeraldAISystem>().EmeraldEventsManagerComponent.ResetAI();
        }
    }

    private void SpawnCompanionAI()
    {
        for (int i = 0; i < companionCyborgs.Length; i++)
        {
            companionCyborgs[i].SetActive(true);
            companionCyborgs[i].GetComponent<EmeraldAISystem>().EmeraldEventsManagerComponent.ResetAI();
        }
    }

    private void SpawnMiniBoss()
    {
        miniBoss.SetActive(true);
        miniBoss.GetComponent<EmeraldAISystem>().EmeraldEventsManagerComponent.ResetAI();
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
        var index = 0;

        yield return new WaitForSeconds(spawnInitialDelay);

        while (index < chargingOrcs.Length)
        {
            yield return new WaitForSeconds(spawnInterval);

            var spawnedOrc = chargingOrcs[index];
            index++;

            spawnedOrc.transform.position = spawnPosition.position;

            spawnedOrc.EmeraldEventsManagerComponent.ResetAI();

            var cyborgToAttack = availableOrcTargets[Random.Range(0, availableOrcTargets.Length)];

            spawnedOrc.EmeraldEventsManagerComponent.SetCombatTarget(cyborgToAttack.transform);
        }
    }

    private IEnumerator GateEffect()
    {
        yield return new WaitForSeconds(secondsToWaitForCover);

        gateEffectToSpawn.SetActive(true);
        gateToDestroy.SetActive(false);
    }
}
