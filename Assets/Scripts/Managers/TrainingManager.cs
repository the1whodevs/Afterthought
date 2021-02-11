using System;
using System.Collections;
using UnityEngine;

public class TrainingManager : MonoSingleton<TrainingManager, ReportMissingInstanceError>
{
    [SerializeField] private LoadoutData trainingLoadout;

    [SerializeField] private GameObject[] EdgeZones;
    [SerializeField] private GameObject[] BorderZones;
    [SerializeField] private GameObject[] MeleeTargets;

    public static LoadoutData GetTrainingLoadout()
    {
        return Active.trainingLoadout;
    }

    public void OnReachMeleeZone()
    {
        EnableColliderMeleeZone();
    }
    public void OnPickUpCyberBlade()
    {
        SpawnEnemiesInMeleeZone();
    }
    public void OnKillMeleeTargets()
    {
        DisableCollidersMeleeZone();
        Debug.Log("DisableColliders");
    }

    private void EnableColliderMeleeZone()
    {
        for (int i = 0; i < EdgeZones.Length; i++)
        {
            EdgeZones[i].SetActive(false);
        }
        for (int i = 0; i < BorderZones.Length; i++)
        {
            BorderZones[i].SetActive(true);
        }
    }

    private void SpawnEnemiesInMeleeZone()
    {
        for (int i = 0; i < MeleeTargets.Length; i++)
        {
            MeleeTargets[i].SetActive(true);
        }
    }

    private void DisableCollidersMeleeZone()
    {
        for (int i = 0; i < EdgeZones.Length; i++)
        {
            EdgeZones[i].SetActive(true);
        }
        for (int i = 0; i < BorderZones.Length; i++)
        {
            BorderZones[i].SetActive(false);
        }
    }

}
