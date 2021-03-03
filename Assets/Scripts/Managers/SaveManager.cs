using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoSingleton<SaveManager>
{
    private static SaveSystem.SaveData currentDataLoading;

    private Player player;

    private const int QUICKSAVE_INDEX = -1;

    private void Start()
    {
        Debug.Log("SaveManage Start!");
        DontDestroyOnLoad(this);

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    public void SetPlayerInstance(Player inst)
    {
        player = inst;
        player.Init();
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        player = null;
        Debug.Log("Scene changed!");

        if (currentDataLoading != null) StartCoroutine(LoadData(currentDataLoading));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            QuickSave();

        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
            QuickLoad();
    }

    public void QuickSave()
    {
        SaveAtIndex(QUICKSAVE_INDEX);
    }

    public void SaveAtIndex(int index)
    {
        var p = Player.Active;
        var le = LoadoutEditor.Active; 

        var temp = new List<ILootable>();
        var temp2 = new List<EmeraldAI.EmeraldAISystem>();

        foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects()) 
        {
            temp.AddRange(go.GetComponentsInChildren<ILootable>(true));
            temp2.AddRange(go.GetComponentsInChildren<EmeraldAI.EmeraldAISystem>(true));
        }

        SaveSystem.Save(index, SceneManager.GetActiveScene().buildIndex, p.Experience.Level, p.Experience.CurrentXP, p.Health.CurrentHP, p.transform.position, p.transform.rotation,
            p.Objectives.CurrentObjectiveIndex, le.AllLoadouts, le.AllWeapons, p.Loadout.AllAmmo, le.AllEquipment, le.AllTalents, temp2.ToArray(), temp.ToArray());
    }

    public void NewSave()
    {
        var filePaths = Directory.GetFiles(Application.persistentDataPath + "/", SaveSystem.SAVE_EXT);

        SaveAtIndex(filePaths.Length);
    }

    public void QuickLoad()
    {
        LoadAtIndex(QUICKSAVE_INDEX);
    }

    public void LoadAtIndex(int index)
    {
        currentDataLoading = SaveSystem.Load(index);

        SceneManager.LoadScene("Loading", LoadSceneMode.Single);
    }

    public void LoadLast()
    {
        var filePaths = Directory.GetFiles(Application.persistentDataPath + "/", SaveSystem.SAVE_EXT);

        if (filePaths.Length > 0) LoadAtIndex(filePaths.Length);
        else Debug.LogError("No save files found!");
    }

    private IEnumerator LoadData(SaveSystem.SaveData data)
    {
        currentDataLoading = null;

        Debug.Log("Loading data...");

        var loadedScene = SceneManager.LoadSceneAsync(data.level, LoadSceneMode.Additive);
        loadedScene.allowSceneActivation = true;

        while (!loadedScene.isDone) yield return new WaitForEndOfFrame();

        var scene = SceneManager.GetSceneAt(1);
        SceneManager.SetActiveScene(scene);

        var p = Player.Active;
        p.Init();
        var le = LoadoutEditor.Active;

        var temp = new List<ILootable>();
        var temp2 = new List<EmeraldAI.EmeraldAISystem>();

        foreach (var go in scene.GetRootGameObjects())
        {
            temp.AddRange(go.GetComponentsInChildren<ILootable>(true));
            temp2.AddRange(go.GetComponentsInChildren<EmeraldAI.EmeraldAISystem>(true));
        }

        p.Experience.LoadData(data.level, data.playerXP);
        p.Health.LoadData(data.playerHP);
        p.transform.position = new Vector3(data.playerPosition_X, data.playerPosition_Y, data.playerPosition_Z);
        p.transform.rotation = new Quaternion(data.playerRotation_X, data.playerRotation_Y, data.playerRotation_Z, data.playerRotation_W);

        p.Objectives.LoadData(data.objectiveIndex);

        le.LoadData(data.loadoutsWepAindex, data.loadoutsWepBindex, data.loadoutsEqAindex, data.loadoutsEqBindex, data.loadoutsTalAindex, data.loadoutsTalBindex, data.loadoutsTalCindex, data.weaponsLootStatus, data.weaponsAmmoInMag, data.equipmentAmmo);

        p.Loadout.LoadData(data.ammoTypesCurrentAmmo);

        for (var i = 0; i < temp.Count; i++)
            if (data.lootablesLootStatus[i]) Destroy(temp[i].gameObject);

        for (var i = 0; i < temp2.Count; i++)
        {
            temp2[i].transform.position = new Vector3(data.aiPosition_X[i], data.aiPosition_Y[i], data.aiPosition_Z[i]);

            temp2[i].transform.rotation = new Quaternion(data.aiRotation_X[i], data.aiRotation_Y[i], data.aiRotation_Z[i], data.aiRotation_W[i]);

            temp2[i].CurrentHealth = data.aiHP[i];
        }

        var asyncop = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        while (!asyncop.isDone) yield return new WaitForEndOfFrame();

        Debug.Log("Finished loading data...");

        currentDataLoading = null;
    }
}
