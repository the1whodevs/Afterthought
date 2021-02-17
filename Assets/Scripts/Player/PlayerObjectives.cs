using System;
using System.Collections;
using UnityEngine;

public class PlayerObjectives : MonoSingleton<PlayerObjectives>
{
    [SerializeField] private Objective[] levelObjectiveData;

    [SerializeField] private ObjectivePickupEquipment[] pickupEquipmentObjectives;
    [SerializeField] private ObjectivePickupWeapon[] pickupWeaponObjectives;
    [SerializeField] private ObjectiveKillTargets[] killTargetsObjectives;
    [SerializeField] private ObjectiveGoToArea[] goToAreaObjectives;
    [SerializeField] private ObjectiveInteractWithInteractable[] interactWithObjectives;

    private Objective_Old[] allObjectives;

    private Objective_Old currentObjective
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
        PlayerPickup.OnInteract += OnPlayerInteract;

        InitializeObjectives();

        StartCoroutine(CheckForCurrentObjective());
    }

    private void InitializeObjectives()
    {
        allObjectives = new Objective_Old[pickupEquipmentObjectives.Length +
            pickupWeaponObjectives.Length +
            killTargetsObjectives.Length +
            goToAreaObjectives.Length +
            interactWithObjectives.Length];

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

        for (var i = 0; i < interactWithObjectives.Length; i++)
        {
            allObjectives[index] = interactWithObjectives[i];
            allObjectives[index].onObjectiveComplete.AddListener(NextObjective);
            index++;
        }

        Array.Sort(allObjectives, new ObjectiveComparer().Compare);

        NextObjective();
    }

    private void OnPlayerInteract(InteractableObject obj)
    {
        if (currentObjective == null) return;

        if (currentObjective.objectiveType == Objective_Old.ObjectiveType.Interact)
            currentObjective.CheckObjective(obj);
    }

    private void OnPlayerEquippedEquipment(EquipmentData equippedEquipment)
    {
        if (currentObjective == null) return;

        if (currentObjective.objectiveType == Objective_Old.ObjectiveType.PickupEquipment)
            currentObjective.CheckObjective(equippedEquipment);
    }

    private void OnPlayerEquippedWeapon(WeaponData equippedWeapon)
    {
        if (currentObjective == null) return;

        if (currentObjective.objectiveType == Objective_Old.ObjectiveType.PickupWeapon)
            currentObjective.CheckObjective(equippedWeapon);
    }

    public void NextObjective()
    {
        currentObjectiveId++;

        if (!(currentObjective is null))
        {
            UIManager.Active.UpdateObjectiveText(currentObjective.objectiveText);
        }
        else
        {
            UIManager.Active.UpdateObjectiveText("");
        }
    }

    private IEnumerator CheckForCurrentObjective()
    {
        while (true)
        {
            while (currentObjective is null) yield return new WaitForEndOfFrame();

            switch (currentObjective.objectiveType)
            {
                case Objective_Old.ObjectiveType.KillTargets:
                    currentObjective.CheckObjective(null);
                    break;

                case Objective_Old.ObjectiveType.GoToArea:
                    currentObjective.CheckObjective(Player.Active.transform);
                    break;

                case Objective_Old.ObjectiveType.PickupEquipment:
                    break;

                case Objective_Old.ObjectiveType.PickupWeapon:
                    break;

                default:
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
