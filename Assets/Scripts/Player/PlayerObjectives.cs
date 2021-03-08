using System.Collections;
using UnityEngine;

public class PlayerObjectives : MonoSingleton<PlayerObjectives>
{
    public int CurrentObjectiveIndex => currentObjectiveId;

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

    public void LoadoutEditorInit()
    {
        StartCoroutine(SubscribeToLoadoutEditorEvents());

        PlayerPickup.OnInteract += OnPlayerInteract;

        levelObjectiveData.Initialize();

        foreach (var obj in levelObjectiveData.objectiveData)
            obj.onObjectiveComplete += NextObjective;

        NextObjective();
    }

    public void Init(bool cleanInit)
    {
        if (!cleanInit)
        {
            var temp = currentObjectiveId;

            currentObjectiveId = 0;

            for (var i = 0; i < temp; i++)
                allObjectives[i].onObjectiveComplete?.Invoke();

        }

        StartCoroutine(CheckForCurrentObjective());
    }

    public void LoadData(int currentObjectiveIndex)
    {
        currentObjectiveId = currentObjectiveIndex;
    }

    private IEnumerator SubscribeToLoadoutEditorEvents()
    {
        while (!LoadoutEditor.Active) yield return new WaitForEndOfFrame();

        LoadoutEditor.Active.OnWeaponSwitched += OnLoadoutSlotSwitched;
        LoadoutEditor.Active.OnEquipmentSwitched += OnLoadoutSlotSwitched;
        LoadoutEditor.Active.OnTalentSwitched += OnLoadoutSlotSwitched;
    }

    private void OnPlayerInteract(InteractableObject obj)
    {
        if (currentObjective == null) return;

        if (currentObjective.objectiveType == ObjectiveData.ObjectiveType.Interact)
            currentObjective.CheckObjective(obj);
    }

    public void NextObjective()
    {

        if (!SaveManager.Active.LoadingData) UISoundFXManager.Active.PlayObjectiveUpdatedClip();

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
