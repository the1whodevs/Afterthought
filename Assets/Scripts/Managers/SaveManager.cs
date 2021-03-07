using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoSingleton<SaveManager>
{
    public System.Action onDataLoadingCompleted;

    public bool LoadingData { get; private set; } = false;
    public bool HasQuicksave { get; private set; }

    public int NumOfSaves { get; private set; }

    private static SaveSystem.SaveData currentDataLoading;

    private Player player;
    private LoadoutEditor le;

    private int selectedSaveIndex = -100;

    private const int QUICKSAVE_INDEX = -1;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        NumOfSaves = SaveSystem.GetNumOfSaves();
        HasQuicksave = SaveSystem.CheckForQuicksave();

        le = LoadoutEditor.Active;
    }

    private void Start()
    {
        LoadingManager.Active.onTargetLevelLoaded += OnTargetLevelLoaded;
    }

    public void SetPlayerInstance(Player inst)
    {
        player = inst;

        if (currentDataLoading == null) player.Init(true);
    }

    private void OnTargetLevelLoaded()
    {
        player = null;

        if (currentDataLoading != null) StartCoroutine(LoadData(currentDataLoading));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            QuickSave();

        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
            QuickLoad();
    }

    public void SelectSave(int index)
    {
        selectedSaveIndex = index;
    }

    public void DeleteSelectedSave()
    {
        if (selectedSaveIndex >= -1)
        {
            NumOfSaves--;

            var i = selectedSaveIndex;

            selectedSaveIndex = -100;

            SaveSystem.DeleteAt(i);

            UIManager.Active.RefreshSaveLoadPanels();
        }
    }

    public void LoadSelectedFile()
    {
        if (selectedSaveIndex >= -1)
        {
            var i = selectedSaveIndex;
            selectedSaveIndex = -100;
            LoadAtIndex(i);
        }
    }

    public void SaveSelectedIndex()
    {
        if (selectedSaveIndex >= -1)
        {
            var i = selectedSaveIndex;
            selectedSaveIndex = -100;
            SaveAtIndex(i);
        }
    }

    public void QuickSave()
    {
        SaveAtIndex(QUICKSAVE_INDEX);
    }

    public void SaveAtIndex(int index)
    {
        if (index == -1) HasQuicksave = true;
        else if (index == NumOfSaves)
        {
            NumOfSaves++;
        }

        var p = Player.Active;
        var le = LoadoutEditor.Active; 

        var lootables = new List<ILootable>();
        var ais = new List<EmeraldAI.EmeraldAISystem>();
        var scene = SceneManager.GetActiveScene();

        foreach (var go in scene.GetRootGameObjects()) 
        {
            lootables.AddRange(go.GetComponentsInChildren<ILootable>(true));
            ais.AddRange(go.GetComponentsInChildren<EmeraldAI.EmeraldAISystem>(true));
        }

        var indexesToDisable = new List<int>();

        for (var i = 0; i < lootables.Count; i++)
        {
            if (lootables[i].gameObject.activeInHierarchy) continue;

            lootables[i].gameObject.SetActive(true);
            indexesToDisable.Add(i);
        }

        foreach (var go in scene.GetRootGameObjects())
        {
            var lootablesChildren = go.GetComponentsInChildren<ILootable>(true);

            foreach (var lootable in lootablesChildren)
            {
                if (lootables.Contains(lootable)) continue;

                lootables.Add(lootable);
                Debug.Log("Added " + lootable.gameObject.name);
            }
        }

        for (var i = 0; i < indexesToDisable.Count; i++)
        {
            lootables[indexesToDisable[i]].gameObject.SetActive(false);
        }

        p.Camera.RecalculateOffset();

        SaveSystem.Save(index, scene.buildIndex, p.Experience.Level, p.Experience.CurrentXP, p.Health.CurrentHP, p.transform.position, p.transform.rotation, p.Camera.CamRotationY, p.Camera.BodyRotationX, p.Camera.OffsetFromPlayer, p.Objectives.CurrentObjectiveIndex, le.AllLoadouts, le.AllWeapons, p.Loadout.AllAmmo, le.AllEquipment, le.AllTalents, ais.ToArray(), lootables.ToArray());
    }

    public void QuickLoad()
    {
        LoadAtIndex(QUICKSAVE_INDEX);
    }

    public void LoadAtIndex(int index)
    {
        currentDataLoading = SaveSystem.Load(index);

        LoadingData = true;

        LoadingManager.Active.LoadLevel(currentDataLoading.level);
    }

    public void LoadLast()
    {
        var filePaths = Directory.GetFiles(Application.persistentDataPath + "/", SaveSystem.SAVE_EXT);

        if (filePaths.Length > 0) LoadAtIndex(filePaths.Length);
        else Debug.LogError("No save files found!");
    }

    private IEnumerator LoadData(SaveSystem.SaveData data)
    {
        var scene = SceneManager.GetActiveScene();

        while (!player)
        {
            player = Player.Active;
            yield return new WaitForEndOfFrame();
        }

        currentDataLoading = null;

        player.GetReferences();

        var lootables = new List<ILootable>();
        var ais = new List<EmeraldAI.EmeraldAISystem>();

        foreach (var go in scene.GetRootGameObjects())
        {
            lootables.AddRange(go.GetComponentsInChildren<ILootable>(true));
            ais.AddRange(go.GetComponentsInChildren<EmeraldAI.EmeraldAISystem>(true));
        }

        for (var i = 0; i < lootables.Count; i++)
        {
            lootables[i].gameObject.SetActive(true);
        }

        foreach (var go in scene.GetRootGameObjects())
        {
            var lootablesChildren = go.GetComponentsInChildren<ILootable>(true);

            foreach (var lootable in lootablesChildren)
            {
                if (lootables.Contains(lootable)) continue;

                lootables.Add(lootable);
            }
        }

        player.Controller.enabled = false;
        player.Camera.enabled = false;

        player.Experience.LoadData(data.level, data.playerXP);
        player.Health.LoadData(data.playerHP);

        player.transform.position = new Vector3(data.playerPosition_X, data.playerPosition_Y, data.playerPosition_Z);

        player.Camera.LoadData(new Vector3(data.cameraPlayerOffset_X, data.cameraPlayerOffset_Y, data.cameraPlayerOffset_Z), data.bodyRotationX, data.cameraRotationY);

        player.Camera.enabled = true;
        player.Camera.RecalculateOffset();
        player.Controller.enabled = true;

        player.Objectives.LoadData(data.objectiveIndex-1);

        le.LoadData(data.loadoutsWepAindex, data.loadoutsWepBindex, data.loadoutsEqAindex, data.loadoutsEqBindex, data.loadoutsTalAindex, data.loadoutsTalBindex, data.loadoutsTalCindex, data.weaponsLootStatus, data.weaponsAmmoInMag, data.equipmentAmmo);

        player.Loadout.LoadData(data.ammoTypesCurrentAmmo);
        player.InitComponents(false);

        for (var i = 0; i < ais.Count; i++)
        {
            ais[i].transform.position = new Vector3(data.aiPosition_X[i], data.aiPosition_Y[i], data.aiPosition_Z[i]);

            ais[i].transform.rotation = new Quaternion(data.aiRotation_X[i], data.aiRotation_Y[i], data.aiRotation_Z[i], data.aiRotation_W[i]);

            ais[i].CurrentHealth = data.aiHP[i];

            ais[i].gameObject.SetActive(data.aiObjectStatus[i]);

            if (ais[i].CurrentHealth <= 0) ais[i].CheckDead();
        }

        for (var i = 0; i < lootables.Count; i++)
        {
            lootables[i].gameObject.SetActive(true);

            yield return new WaitForEndOfFrame();

            lootables[i].SetLootStatus(data.lootablesLootStatus[i]);
            
            lootables[i].gameObject.SetActive(data.lootablesObjectStatus[i]);
        }

        onDataLoadingCompleted?.Invoke();

        LoadingData = false;
    }
}
