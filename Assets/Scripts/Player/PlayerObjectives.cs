using System;
using System.Collections;
using UnityEngine;

public class PlayerObjectives : MonoSingleton<PlayerObjectives, ReportMissingInstance>
{
    [SerializeField] private ObjectiveDataPickupEquipment[] pickupEquipmentObjectives;
    [SerializeField] private ObjectiveDataPickupWeapon[] pickupWeaponObjectives;
    [SerializeField] private ObjectiveDataKillTargets[] killTargetsObjectives;
    [SerializeField] private ObjectiveDataGoToArea[] goToAreaObjectives;

    private ObjectiveData[] allObjectives;

    private ObjectiveData currentObjective
    {
        get
        {
            if (currentObjectiveId < 0 || currentObjectiveId >= allObjectives.Length) return null;

            return allObjectives[currentObjectiveId];
        }
    }

    private int currentObjectiveId = -1;

    private void Start()
    {
        Player.Active.Loadout.OnEquipmentEquipped += OnPlayerEquippedEquipment;
        Player.Active.Loadout.OnWeaponEquipped += OnPlayerEquippedWeapon;

        InitializeObjectives();

        StartCoroutine(CheckForCurrentObjective());
    }

    private void InitializeObjectives()
    {
        allObjectives = new ObjectiveData[pickupEquipmentObjectives.Length +
            pickupWeaponObjectives.Length +
            killTargetsObjectives.Length +
            goToAreaObjectives.Length];

        var index = 0;

        for (var i = 0; i < pickupEquipmentObjectives.Length; i++)
        {
            allObjectives[index] = pickupEquipmentObjectives[i];
            allObjectives[index].onObjectiveComplete.AddListener(NextObjective);
            index++;
        }

        for (var i = 0; i < pickupWeaponObjectives.Length; i++)
        {
            allObjectives[index] = pickupWeaponObjectives[i];
            allObjectives[index].onObjectiveComplete.AddListener(NextObjective);
            index++;
        }

        for (var i = 0; i < killTargetsObjectives.Length; i++)
        {
            allObjectives[index] = killTargetsObjectives[i];
            allObjectives[index].onObjectiveComplete.AddListener(NextObjective);
            index++;
        }

        for (var i = 0; i < goToAreaObjectives.Length; i++)
        {
            allObjectives[index] = goToAreaObjectives[i];
            allObjectives[index].onObjectiveComplete.AddListener(NextObjective);
            index++;
        }

        Array.Sort(allObjectives, new ObjectiveDataComparer().Compare);

        NextObjective();
    }

    private void OnPlayerEquippedEquipment(EquipmentData equippedEquipment)
    {
        if (currentObjective is null) return;

        if (currentObjective.objectiveType == ObjectiveData.ObjectiveType.PickupEquipment)
            currentObjective.CheckObjective(equippedEquipment);
    }

    private void OnPlayerEquippedWeapon(WeaponData equippedWeapon)
    {
        if (currentObjective.objectiveType == ObjectiveData.ObjectiveType.PickupWeapon)
            currentObjective.CheckObjective(equippedWeapon);
    }

    public void NextObjective()
    {
        currentObjectiveId++;

        if (!(currentObjective is null))
        {
            UIManager.Active.UpdateObjectiveText(currentObjective.objectiveText);
        }
    }

    private IEnumerator CheckForCurrentObjective()
    {
        while (true)
        {
            while (currentObjective is null) yield return new WaitForEndOfFrame();

            switch (currentObjective.objectiveType)
            {
                case ObjectiveData.ObjectiveType.KillTargets:
                    currentObjective.CheckObjective(null);
                    break;

                case ObjectiveData.ObjectiveType.GoToArea:
                    currentObjective.CheckObjective(Player.Active.transform);
                    break;

                case ObjectiveData.ObjectiveType.PickupEquipment:
                    break;

                case ObjectiveData.ObjectiveType.PickupWeapon:
                    break;

                default:
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
