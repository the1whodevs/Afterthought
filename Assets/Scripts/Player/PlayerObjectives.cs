using System.Collections;
using UnityEngine;

public class PlayerObjectives : MonoSingleton<PlayerObjectives>
{
    [SerializeField] private LevelObjectiveData levelObjectiveData;

    private ObjectiveData[] allObjectives => levelObjectiveData.objectiveData;

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
        PlayerPickup.OnInteract += OnPlayerInteract;
        LoadoutEditor.Active.OnWeaponSwitched += OnLoadoutSlotSwitched;
        LoadoutEditor.Active.OnEquipmentSwitched += OnLoadoutSlotSwitched;
        LoadoutEditor.Active.OnTalentSwitched += OnLoadoutSlotSwitched;

        levelObjectiveData.Initialize();
        NextObjective();

        StartCoroutine(CheckForCurrentObjective());
    }

    private void OnPlayerInteract(InteractableObject obj)
    {
        if (currentObjective == null) return;

        if (currentObjective.objectiveType == ObjectiveData.ObjectiveType.Interact)
            currentObjective.CheckObjective(obj);
    }

    public void NextObjective()
    {
        currentObjectiveId++;

        if (currentObjective != null)
            UIManager.Active.UpdateObjectiveText(currentObjective.objectiveText);
        else
            UIManager.Active.UpdateObjectiveText("");
    }

    private void OnLoadoutSlotSwitched(int slotSwitched)
    {
        if (currentObjective == null) return;

        if (currentObjective.objectiveType == ObjectiveData.ObjectiveType.EquipEquipmentOnSlot ||
            currentObjective.objectiveType == ObjectiveData.ObjectiveType.EquipTalentOnSlot ||
            currentObjective.objectiveType == ObjectiveData.ObjectiveType.EquipWeaponOnSlot)
            currentObjective.CheckObjective(slotSwitched);
    }

    private IEnumerator CheckForCurrentObjective()
    {
        while (true)
        {
            while (currentObjective == null) yield return new WaitForEndOfFrame();

            switch (currentObjective.objectiveType)
            {
                case ObjectiveData.ObjectiveType.KillTargets:
                    currentObjective.CheckObjective();
                    break;

                case ObjectiveData.ObjectiveType.GoToArea:
                    currentObjective.CheckObjective();
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
