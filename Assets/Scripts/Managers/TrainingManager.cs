using System;
using System.Collections;
using UnityEngine;

public class TrainingManager : MonoSingleton<TrainingManager, ReportMissingInstanceError>
{
    [SerializeField] private LoadoutData trainingLoadout;

    [Header("Melee Zone")] 
    [SerializeField] private GameObject[] MeleeEdgeZones;
    [SerializeField] private GameObject[] MeleeBorderZones;
    [SerializeField] private GameObject[] MeleeTargets;

    [Header("Ranged Zone")]
    [SerializeField] private GameObject[] RangedEdgeZones;
    [SerializeField] private GameObject[] RangedBorderZones;
    [SerializeField] private GameObject[] RangedTargets;

    [Header("Projectiles Zone")]
    [SerializeField] private GameObject[] ProjectilesEdgeZones;
    [SerializeField] private GameObject[] ProjectilesBorderZones;
    [SerializeField] private GameObject[] ProjectilesTargets;

    [Header("Progression Zone")]
    [SerializeField] private GameObject[] ProgressionEdgeZones;
    [SerializeField] private GameObject[] ProgressionBorderZones;
    [SerializeField] private GameObject[] ProgressionCrates;

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
    }

    public void OnReachRangedZone()
    {
        EnableColliderRangedZone();
    }

    public void OnPickUpRangedWeapon()
    {
        SpawnEnemiesInRangedZone();
    }

    public void OnKillRangedTargets()
    {
        DisableCollidersRangedZone();
    }

    public void OnReachProjectilesZone()
    {
        EnableColliderProjectilesZone();
    }

    public void OnPickUpProjectilesWeapon()
    {
        SpawnEnemiesInProjectilesZone();
    }

    public void OnKillProjectilesTargets()
    {
        DisableCollidersProjectilesZone();
    }
    
    public void OnReachProgressionZone()
    {
        EnableColliderProgressionZone();
        SpawnCratesInProgressionZone();
    }

    public void OnInteractinWithXpCrate()
    {
        DisableCollidersProgressionZone();
    }

    private void EnableColliderMeleeZone()
    {
        for (int i = 0; i < MeleeEdgeZones.Length; i++)
        {
            MeleeEdgeZones[i].SetActive(false);
        }
        for (int i = 0; i < MeleeBorderZones.Length; i++)
        {
            MeleeBorderZones[i].SetActive(true);
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
        for (int i = 0; i < MeleeEdgeZones.Length; i++)
        {
            MeleeEdgeZones[i].SetActive(true);
        }
        for (int i = 0; i < MeleeBorderZones.Length; i++)
        {
            MeleeBorderZones[i].SetActive(false);
        }
    }

    private void EnableColliderRangedZone()
    {
        for (int i = 0; i < RangedEdgeZones.Length; i++)
        {
            RangedEdgeZones[i].SetActive(false);
        }
        for (int i = 0; i < RangedBorderZones.Length; i++)
        {
            RangedBorderZones[i].SetActive(true);
        }
    }

    private void DisableCollidersRangedZone()
    {
        for (int i = 0; i < RangedEdgeZones.Length; i++)
        {
            RangedEdgeZones[i].SetActive(true);
        }
        for (int i = 0; i < RangedBorderZones.Length; i++)
        {
            RangedBorderZones[i].SetActive(false);
        }
    }

    private void SpawnEnemiesInRangedZone()
    {
        for (int i = 0; i < RangedTargets.Length; i++)
        {
            RangedTargets[i].SetActive(true);
        }
    }

    private void EnableColliderProjectilesZone()
    {
        for (int i = 0; i < ProjectilesEdgeZones.Length; i++)
        {
            ProjectilesEdgeZones[i].SetActive(false);
        }
        for (int i = 0; i < ProjectilesEdgeZones.Length; i++)
        {
            ProjectilesBorderZones[i].SetActive(true);
        }
    }

    private void DisableCollidersProjectilesZone()
    {
        for (int i = 0; i < ProjectilesEdgeZones.Length; i++)
        {
            ProjectilesEdgeZones[i].SetActive(true);
        }
        for (int i = 0; i < ProjectilesBorderZones.Length; i++)
        {
            ProjectilesBorderZones[i].SetActive(false);
        }
    }

    private void SpawnEnemiesInProjectilesZone()
    {
        for (int i = 0; i < ProjectilesTargets.Length; i++)
        {
            ProjectilesTargets[i].SetActive(true);
        }
    }

    private void EnableColliderProgressionZone()
    {
        for (int i = 0; i < ProgressionEdgeZones.Length; i++)
        {
            ProgressionEdgeZones[i].SetActive(false);
        }
        for (int i = 0; i < ProgressionBorderZones.Length; i++)
        {
            ProgressionBorderZones[i].SetActive(true);
        }
    }

    private void DisableCollidersProgressionZone()
    {
        for (int i = 0; i < ProgressionEdgeZones.Length; i++)
        {
            ProgressionEdgeZones[i].SetActive(true);
        }
        for (int i = 0; i < ProgressionBorderZones.Length; i++)
        {
            ProgressionBorderZones[i].SetActive(false);
        }
    }

    private void SpawnCratesInProgressionZone()
    {
        for (int i = 0; i < ProgressionCrates.Length; i++)
        {
            ProgressionCrates[i].SetActive(true);
        }
    }
}
