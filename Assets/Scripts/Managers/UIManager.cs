using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Active
    {
        get
        {
            if (_instance) return _instance;

            _instance = FindObjectOfType<UIManager>();

            return _instance;
        }
    }

    private static UIManager _instance;

    public GameObject Crosshair => crosshair;
    public GameObject HealthBar => healthBar;
    public TextMeshProUGUI ScannedHPdisplay => scannedHPdisplay;

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

    public void Init(PlayerLoadout loadout)
    {
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
