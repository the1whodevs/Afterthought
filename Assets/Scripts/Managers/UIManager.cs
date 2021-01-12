using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI ammoUI;
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
    }

    public void ToggleCrosshair(bool status) => crosshair.SetActive(status);
    public void ToggleHealthBar(bool status) => healthBar.SetActive(status);
    
    public void SetAmmoUI(int currentAmmo, int maxAmmo) => ammoUI.text = $"{currentAmmo} / {maxAmmo}";
}
