using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisor : MonoBehaviour
{
    [SerializeField] private LayerMask scannableItemLayers;

    private PlayerController pc;

    private TextMeshProUGUI scannedHPpercentage;
    private Image crosshairImage;

    private Camera mainCam;

    public void Init(PlayerController pc)
    {
        this.pc = pc;

        crosshairImage = UIManager.Active.Crosshair.GetComponent<Image>();
        crosshairImage.color = Color.white;
        scannedHPpercentage = UIManager.Active.ScannedHPdisplay;
        scannedHPpercentage.color = Color.white;
        scannedHPpercentage.text = "";
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (pc.IsInUI) return;

        var ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (!Physics.Raycast(ray, out var hit, mainCam.farClipPlane, scannableItemLayers)) return;

        var scannable = hit.transform.GetComponent<IScannable>();
        if (scannable != null) 
        { 
            crosshairImage.color = scannable.GetCrosshairColor();
            scannedHPpercentage.color = crosshairImage.color;
            scannedHPpercentage.text = $"HP: {100*scannable.GetHPPercentage():F2} %";
        }
        else
        {
            crosshairImage.color = Color.white;
            scannedHPpercentage.color = Color.white;
            scannedHPpercentage.text = "";
        }
    }
}
