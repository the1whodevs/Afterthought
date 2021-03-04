using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoSingleton<SaveManager>
{
    private static SaveSystem.SaveData currentDataLoading;

    private Player player;

    private const string LOADING_SCENE = "99 - Loading";
    private const int QUICKSAVE_INDEX = -1;

    private void Start()
    {
        DontDestroyOnLoad(this);

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    public void SetPlayerInstance(Player inst)
    {
        player = inst;

        if (currentDataLoading == null) player.Init(true);
    }

    private void SceneManager_activeSceneChanged(Scene current, Scene next)
    {
        player = null;

        if (currentDataLoading != null && next.name == LOADING_SCENE) StartCoroutine(LoadData(currentDataLoading));
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

        SceneManager.LoadScene(LOADING_SCENE, LoadSceneMode.Single);
    }

    public void LoadLast()
    {
        var filePaths = Directory.GetFiles(Application.persistentDataPath + "/", SaveSystem.SAVE_EXT);

        if (filePaths.Length > 0) LoadAtIndex(filePaths.Length);
        else Debug.LogError("No save files found!");
    }

    private IEnumerator LoadData(SaveSystem.SaveData data)
    {
        var loadedScene = SceneManager.LoadSceneAsync(data.level, LoadSceneMode.Additive);
        loadedScene.allowSceneActivation = true;

        while (!loadedScene.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        var scene = SceneManager.GetSceneAt(1);
        SceneManager.SetActiveScene(scene);

        yield return new WaitForSeconds(1.0f);

        while (!player)
        {
            player = Player.Active;
            yield return new WaitForEndOfFrame();
        }

        currentDataLoading = null;

        player.SpawnObjectsToSpawnOnSpawn();
        player.GetReferences();

        var le = LoadoutEditor.Active;

        while (!le)
        {
            le = LoadoutEditor.Active; 
            yield return new WaitForEndOfFrame();
        }

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


        for (var i = 0; i < lootables.Count; i++)
        {
            lootables[i].gameObject.SetActive(true);

            yield return new WaitForEndOfFrame();

            lootables[i].SetLootStatus(data.lootablesLootStatus[i]);
            
            lootables[i].gameObject.SetActive(data.lootablesObjectStatus[i]);
        }


        for (var i = 0; i < ais.Count; i++)
        {
            ais[i].transform.position = new Vector3(data.aiPosition_X[i], data.aiPosition_Y[i], data.aiPosition_Z[i]);

            ais[i].transform.rotation = new Quaternion(data.aiRotation_X[i], data.aiRotation_Y[i], data.aiRotation_Z[i], data.aiRotation_W[i]);

            ais[i].CurrentHealth = data.aiHP[i];

            ais[i].gameObject.SetActive(data.aiObjectStatus[i]);

            if (ais[i].CurrentHealth <= 0) ais[i].CheckDead();
        }

    }
}
