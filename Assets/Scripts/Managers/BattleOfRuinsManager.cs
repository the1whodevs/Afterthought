using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOfRuinsManager : MonoBehaviour
{
    [Header("OnObjectiveComplete Actions")]
    [SerializeField] private OnObjectiveCompleteWrapper[] onObjectiveCompleteActions;

    [Header("First Defence")]
    [SerializeField] private GameObject[] firstDefenceTargets;
    [SerializeField] private float spawnDistanceCheck;

    private bool spawnChargingOrcs = true;


    public void Awake()
    {
        foreach (var obj in onObjectiveCompleteActions)
            obj.Initialize();

        for (var i = 0; i < firstDefenceTargets.Length; i++)
            firstDefenceTargets[i].SetActive(false);
    }

    private void Start()
    {
        if (Vector3.Distance(Player.Active.transform.position, Vector3.zero) > spawnDistanceCheck) spawnChargingOrcs = false;
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
        spawnChargingOrcs = false;
        SpawnEnemiesInFirstDefence();
    }
}
