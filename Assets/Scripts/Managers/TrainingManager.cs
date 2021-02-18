using UnityEngine;

public class TrainingManager : MonoSingleton<TrainingManager>
{
    [SerializeField] private LoadoutData trainingLoadout;

    [Header("OnObjectiveComplete Actions")]
    [SerializeField] private OnObjectiveCompleteWrapper[] onObjectiveCompleteActions;

    [Header("Melee Zone")]
    [SerializeField] private GameObject cyberblade;
    [SerializeField] private GameObject[] MeleeEdgeZones;
    [SerializeField] private GameObject[] MeleeBorderZones;
    [SerializeField] private GameObject[] MeleeTargets;

    [Header("Ranged Zone")]
    [SerializeField] private GameObject scarfaceMk2;
    [SerializeField] private GameObject[] RangedEdgeZones;
    [SerializeField] private GameObject[] RangedBorderZones;
    [SerializeField] private GameObject[] RangedTargets;

    [Header("Projectiles Zone")]
    [SerializeField] private GameObject crossbow;
    [SerializeField] private GameObject[] ProjectilesEdgeZones;
    [SerializeField] private GameObject[] ProjectilesBorderZones;
    [SerializeField] private GameObject[] ProjectilesTargets;
    [SerializeField] private GameObject ProjectilesPowerfulEnemy;

    [Header("Progression Zone")]
    [SerializeField] private GameObject loadoutEditor;
    [SerializeField] private GameObject[] ProgressionEdgeZones;
    [SerializeField] private GameObject[] ProgressionBorderZones;
    [SerializeField] private GameObject[] ProgressionCrates;

    private void Start()
    {
        cyberblade.SetActive(false);
        scarfaceMk2.SetActive(false);
        crossbow.SetActive(false);
        loadoutEditor.SetActive(false);
        ProjectilesPowerfulEnemy.SetActive(false);

        DisableCollidersMeleeZone();
        DisableCollidersProgressionZone();
        DisableCollidersProjectilesZone();
        DisableCollidersRangedZone();

        foreach (var obj in onObjectiveCompleteActions)
            obj.Initialize();

        for (var i = 0; i < MeleeTargets.Length; i++)
            MeleeTargets[i].SetActive(false);

        for (var i = 0; i < RangedTargets.Length; i++)
            RangedTargets[i].SetActive(false);

        for (var i = 0; i < ProjectilesTargets.Length; i++)
            ProjectilesTargets[i].SetActive(false);

        for (var i = 0; i < ProgressionCrates.Length; i++)
            ProgressionCrates[i].SetActive(false);
    }

    public static LoadoutData GetTrainingLoadout()
    {
        return Active.trainingLoadout;
    }

    public void OnReachMeleeZone()
    {
        cyberblade.SetActive(true);
        //EnableColliderMeleeZone();
    }

    public void OnPickUpCyberBlade()
    {
        EnableColliderMeleeZone();
        SpawnEnemiesInMeleeZone();
    }

    public void OnKillMeleeTargets()
    {
        DisableCollidersMeleeZone();
    }

    public void OnReachRangedZone()
    {
        scarfaceMk2.SetActive(true);
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
        crossbow.SetActive(true);
        EnableColliderProjectilesZone();
    }

    public void OnPickUpProjectilesWeapon()
    {
        SpawnEnemiesInProjectilesZone();
    }

    public void OnKillProjectilesTargets()
    {
        // Spawn miniboss.
        ProjectilesPowerfulEnemy.SetActive(true);
    }

    public void OnKillProjectilesPowerfulEnemy()
    {
        DisableCollidersProjectilesZone();
    }
    
    public void OnReachProgressionZone()
    {
        EnableColliderProgressionZone();
        SpawnCratesInProgressionZone();
    }

    public void OnInteractWithXpCube()
    {
        loadoutEditor.SetActive(true);
    }

    public void OnInteractWithLootCube()
    {

    }

    public void OnInteractWithLoadoutEditor()
    {

    }

    public void OnReachedLoadoutEditorZone()
    {

    }

    public void OnFinishedLoadoutTraining()
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
