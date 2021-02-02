using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject HealthBar => healthBar;

    [SerializeField] private TextMeshProUGUI interactPrompt;

    [SerializeField] private TextMeshProUGUI weaponAmmoCount;
    [SerializeField] private TextMeshProUGUI equipmentAammoCount;
    [SerializeField] private TextMeshProUGUI equipmentBammoCount;

    [SerializeField] private Image weapon_AmmoIcon;
    [SerializeField] private Image equipmentA_AmmoIcon;
    [SerializeField] private Image equipmentB_AmmoIcon;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject healthBar;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);

        Instance = this;
    }

    private void Start()
    {
        crosshair.SetActive(true);
        healthBar.SetActive(true);

        UpdateAmmoIcons(Player.Instance.Equipment.Loadout, Player.Instance.Equipment.CurrentWeapon);
    }

    public void ShowInteractPrompt(KeyCode keyPrompt)
    {
        interactPrompt.text = $"Press {keyPrompt} to interact";
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
        weapon_AmmoIcon.sprite = weaponEquipped.ammoType.icon;
        weaponAmmoCount.text = $"{weaponEquipped.ammoInMagazine} / {weaponEquipped.ammoType.currentAmmo}";
    }

    public void UpdateEquipmentAmmoCountEquipmentA(EquipmentData eqEquipped)
    {
        equipmentAammoCount.text = $"{eqEquipped.currentAmmo}";
    }

    public void UpdateEquipmentAmmoCountEquipmentb(EquipmentData eqEquipped)
    {
        equipmentBammoCount.text = $"{eqEquipped.currentAmmo}";
    }

    public void ToggleCrosshair(bool status) => crosshair.SetActive(status);
    public void ToggleHealthBar(bool status) => healthBar.SetActive(status);
}
