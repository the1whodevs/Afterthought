using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject Crosshair => crosshair;
    public GameObject HealthBar => healthBar;
    public TextMeshProUGUI ScannedHPdisplay => scannedHPdisplay;

    [Header("Load & Save UI")]
    [SerializeField] private GameObject saveGameDisplayPrefab;
    [SerializeField] private float heightPerSaveGameDisplay;

    [Header("Load Game UI")]
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private RectTransform loadGameDisplayContent;
    [SerializeField] private TextMeshProUGUI saveToLoadInfo;
    [SerializeField] private Image saveToLoadScreenshot;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button deleteSaveToLoadButton;

    private List<GameObject> spawnedLoadDisplays = new List<GameObject>();

    [Header("Save Game UI")]
    [SerializeField] private GameObject saveGamePanel;
    [SerializeField] private RectTransform saveGameDisplayContent;
    [SerializeField] private TextMeshProUGUI selectedSaveSlotInfo;
    [SerializeField] private Image selectedSaveSlotScreenshot;
    [SerializeField] private Button saveGameButton;
    [SerializeField] private Button deleteSelectedSaveButton;
    private List<GameObject> spawnedSaveDisplays = new List<GameObject>();

    [Header("Settings UI")]
    [SerializeField] private GameObject settingsPanel;

    [Header("In-game UI")]
    [SerializeField] private GameObject ingamePanel;

    [SerializeField] private TextMeshProUGUI scannedHPdisplay;
    [SerializeField] private TextMeshProUGUI interactPrompt;
    [SerializeField] private TextMeshProUGUI objectivePrompt;

    [SerializeField] private TextMeshProUGUI weaponAmmoCount;
    [SerializeField] private TextMeshProUGUI equipmentAammoCount;
    [SerializeField] private TextMeshProUGUI equipmentBammoCount;

    [SerializeField] private Image weapon_AmmoIcon;
    [SerializeField] private Image equipmentA_AmmoIcon;
    [SerializeField] private Image equipmentB_AmmoIcon;

    [SerializeField] private RectTransform hitmarkerSpawn;
    [SerializeField] private GameObject hitmarker;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject healthBar;

    [SerializeField] private float objectiveTextSpeed = 1.0f;
    [SerializeField] private float recoilModifier = 1.0f;
    [SerializeField] private float recoilDecay = 1.0f;
    [SerializeField] private float hitmarkerUptime = 1.0f;

    // Used to scale crosshair and visualize recoil.
    private float crosshairRecoilProgress = 0.0f;

    private PlayerLoadout pl;

    private PauseMenu pauseMenu;
    private DeathMenu deathMenu;
    private PortalMenu portalMenu;

    private void Awake()
    {
        if (Active && Active == this) DontDestroyOnLoad(gameObject);
        else
        {
            Debug.LogError("A second instance of UIManager has been found!");
            Destroy(gameObject);
        }

        pauseMenu = PauseMenu.Active;
        deathMenu = DeathMenu.Active;
        portalMenu = PortalMenu.Active;

        portalMenu.gameObject.SetActive(false);
        settingsPanel.SetActive(false);

        HideLoadGamePanel();
        HideSaveGamePanel();
        HideIngamePanel();
    }

    public void HideIngamePanel()
    {
        ingamePanel.SetActive(false);
    }

    public void ShowLoadGamePanel()
    {
        if (SaveManager.Active.HasQuicksave)
        {
            var quickSaveDisplay = Instantiate(saveGameDisplayPrefab, loadGameDisplayContent);
            spawnedLoadDisplays.Add(quickSaveDisplay);
            quickSaveDisplay.GetComponent<SaveDisplay>().Init(-1);
        }

        for (var i = SaveManager.Active.NumOfSaves - 1; i >= 0; i--)
        {
            var display = Instantiate(saveGameDisplayPrefab, loadGameDisplayContent);
            spawnedLoadDisplays.Add(display);
            display.GetComponent<SaveDisplay>().Init(i);
        }

        loadGameDisplayContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightPerSaveGameDisplay * SaveManager.Active.NumOfSaves);

        loadGamePanel.SetActive(true);
    }

    public void ShowSaveGamePanel()
    {
        if (SaveManager.Active.HasQuicksave)
        {
            var quickSaveDisplay = Instantiate(saveGameDisplayPrefab, saveGameDisplayContent);
            spawnedSaveDisplays.Add(quickSaveDisplay);
            quickSaveDisplay.GetComponent<SaveDisplay>().Init(-1);
        }

        for (var i = SaveManager.Active.NumOfSaves - 1; i >= 0; i--)
        {
            var display = Instantiate(saveGameDisplayPrefab, saveGameDisplayContent);
            spawnedSaveDisplays.Add(display);
            display.GetComponent<SaveDisplay>().Init(i);
        }

        saveGameDisplayContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightPerSaveGameDisplay * SaveManager.Active.NumOfSaves);

        saveGamePanel.SetActive(true);
    }

    public void SelectSave(string infoText, Sprite screenshot)
    {
        if (loadGamePanel.activeInHierarchy)
        {
            saveToLoadInfo.text = infoText;
            saveToLoadScreenshot.sprite = screenshot;
            loadGameButton.interactable = true;
            deleteSaveToLoadButton.interactable = true;
        }
        else if (saveGamePanel.activeInHierarchy)
        {
            selectedSaveSlotInfo.text = infoText;
            selectedSaveSlotScreenshot.sprite = screenshot;
            saveGameButton.interactable = true;
            deleteSelectedSaveButton.interactable = true;
        }
    }

    public void RefreshSaveLoadPanels()
    {
        var status = loadGamePanel.activeInHierarchy;

        if (status)
        {
            HideLoadGamePanel();
            ShowLoadGamePanel();
        }

        status = saveGamePanel.activeInHierarchy;

        if (status)
        {
            HideSaveGamePanel();
            ShowLoadGamePanel();
        }
    }

    public void DeleteSelectedSave()
    {
        SaveManager.Active.DeleteSelectedSave();
    }

    public void HideLoadGamePanel()
    {
        loadGamePanel.SetActive(false);
        loadGameButton.interactable = false;
        deleteSaveToLoadButton.interactable = false;

        while (spawnedLoadDisplays.Count > 0)
        {
            var display = spawnedLoadDisplays[0];
            spawnedLoadDisplays.RemoveAt(0);
            Destroy(display);
        }
    }

    public void HideSaveGamePanel()
    {
        saveGamePanel.SetActive(false);
        saveGameButton.interactable = false;
        deleteSelectedSaveButton.interactable = false;

        while (spawnedSaveDisplays.Count > 0)
        {
            var display = spawnedSaveDisplays[0];
            spawnedSaveDisplays.RemoveAt(0);
            Destroy(display);
        }
    }

    public void LoadSelectedSaveFile()
    {
        SaveManager.Active.LoadSelectedFile();

        HideLoadGamePanel();
    }

    public void SaveToSelectedSlot()
    {
        // TODO: Check & display overwrite confirmation!
        SaveManager.Active.SaveSelectedIndex();

        HideSaveGamePanel();
    }

    public void ShowPauseMenu()
    {
        pauseMenu.ShowPauseMenu();
    }

    public void ShowDeathMenu()
    {
        deathMenu.ShowDeathMenu();
    }

    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void ShowPortalMenu(int targetBuildIndex)
    {
        portalMenu.ShowPortalMenu(targetBuildIndex);
    }

    public void Init(PlayerLoadout loadout)
    {
        ingamePanel.SetActive(true);

        pl = loadout;

        crosshair.SetActive(true);
        healthBar.SetActive(true);

        UpdateAmmoIcons(pl.Loadout, pl.CurrentWeapon);

        StartCoroutine(CrosshairRecoil());

        LoadoutEditor.Active.Init();
    }

    public void RefreshHitmarker()
    {
        Destroy(Instantiate(hitmarker, hitmarkerSpawn), hitmarkerUptime);
    }

    private IEnumerator CrosshairRecoil()
    {
        var crosshairRect = crosshair.GetComponent<RectTransform>();
        var loadout = Player.Active.Loadout;

        while (!loadout)
        {
            loadout = Player.Active.Loadout;
            yield return new WaitForEndOfFrame();
        }

        while (Application.isPlaying)
        {
            if (loadout.CurrentWeapon)
            {
                var data = loadout.CurrentWeapon.weaponTypeData;
                crosshairRect.sizeDelta = Vector2.Lerp(data.crosshairDefaultXY, data.maxRecoilCrosshairXY, crosshairRecoilProgress);

                crosshairRecoilProgress = Mathf.Clamp01(crosshairRecoilProgress - Time.deltaTime * recoilDecay);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void UpdateObjectiveText(string newObjective)
    { 
        StartCoroutine(UpdateObjectiveTextSmoothly(newObjective));
    }

    private IEnumerator UpdateObjectiveTextSmoothly(string newText)
    {
        var t = 0.0f;

        objectivePrompt.text = "<sprite=0>";

        string textToDisplay;

        while (t <= 1.0f)
        {
            t += Time.deltaTime * objectiveTextSpeed;

            var targetLength = Mathf.CeilToInt(Mathf.Clamp(newText.Length * t, 0, newText.Length));
            textToDisplay = newText.Substring(0, targetLength);
            objectivePrompt.text = "<sprite=0> " + textToDisplay;

            yield return new WaitForEndOfFrame();
        }
    }

    public void AddRecoil(float recoilX, float recoilY)
    {
        crosshairRecoilProgress = Mathf.Clamp01(crosshairRecoilProgress + Time.deltaTime * recoilModifier * (recoilX * recoilY));
    }

    public void ShowInteractPrompt(KeyCode keyPrompt, string actionName)
    {
        interactPrompt.text = $"Press {keyPrompt} to {actionName}.";
        interactPrompt.gameObject.SetActive(true);
    }

    public void HideInteractPrompt()
    {
        interactPrompt.gameObject.SetActive(false);
    }

    public void UpdateAmmoIcons(LoadoutData equippedLoadout, WeaponData currentWeapon)
    {
        equipmentAammoCount.text = equippedLoadout.Equipment[0].currentAmmo.ToString();
        equipmentBammoCount.text = equippedLoadout.Equipment[1].currentAmmo.ToString();

        equipmentA_AmmoIcon.sprite = equippedLoadout.Equipment[0].icon;
        equipmentB_AmmoIcon.sprite = equippedLoadout.Equipment[1].icon;

        UpdateWeaponAmmoUI(currentWeapon);
    }

    public void UpdateWeaponAmmoUI(WeaponData weaponEquipped)
    {
        crosshair.GetComponent<Image>().sprite = weaponEquipped.weaponTypeData.crosshair;
        crosshairRecoilProgress = 0.0f;

        weapon_AmmoIcon.sprite = weaponEquipped.weaponTypeData.ammoType.icon;

        if (weaponEquipped.weaponType == WeaponData.WeaponType.Melee) weaponAmmoCount.text = "";
        else weaponAmmoCount.text = $"{weaponEquipped.ammoInMagazine} / {weaponEquipped.weaponTypeData.ammoType.currentAmmo}";
    }

    public void UpdateEquipmentAmmoCountEquipmentA(EquipmentData eqEquipped)
    {
        equipmentAammoCount.text = $"{eqEquipped.currentAmmo}";
    }

    public void UpdateEquipmentAmmoCountEquipmentB(EquipmentData eqEquipped)
    {
        equipmentBammoCount.text = $"{eqEquipped.currentAmmo}";
    }

    public void ToggleCrosshair(bool status) => crosshair.SetActive(status);
    public void ToggleHealthBar(bool status) => healthBar.SetActive(status);
}
