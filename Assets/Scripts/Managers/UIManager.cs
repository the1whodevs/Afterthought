using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject HealthBar => healthBar;

    [SerializeField] private TextMeshProUGUI interactPrompt;
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

    public void ShowInteractPrompt(KeyCode keyPrompt)
    {
        interactPrompt.text = $"Press {keyPrompt} to interact";
        interactPrompt.gameObject.SetActive(true);
    }

    public void HideInteractPrompt()
    {
        interactPrompt.gameObject.SetActive(false);
    }

    public void ToggleCrosshair(bool status) => crosshair.SetActive(status);
    public void ToggleHealthBar(bool status) => healthBar.SetActive(status);
    
    public void SetAmmoUI(int currentAmmo, int maxAmmo) => ammoUI.text = $"{currentAmmo} / {maxAmmo}";
}
