using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI ammoUI;
    [SerializeField] private GameObject crosshair;
    
    private void Awake()
    {
        if (Instance) Destroy(gameObject);

        Instance = this;
    }

    private void Start()
    {
        crosshair.SetActive(true);
    }

    public void ToggleCrosshair(bool status) => crosshair.SetActive(status);
    
    public void SetAmmoUI(int currentAmmo, int maxAmmo) => ammoUI.text = $"{currentAmmo} / {maxAmmo}";
}
