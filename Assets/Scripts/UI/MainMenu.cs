using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;

    // The panel containing all main menu buttons.
    [SerializeField] private GameObject mainPanel;

    // The settings window.
    [SerializeField] private GameObject settingsPanel;

    public const string SAVE_FILE_PREFIX = "AFTERTHOUGHT_SAVE_";
    public const string HORIZONTAL_SENS_KEY = "MOUSE_X_SENSITIVITY";
    public const string VERTICAL_SENS_KEY = "MOUSE_Y_SENSITIVITY";

    private void Awake()
    {
        continueButton.SetActive(PlayerPrefs.HasKey($"{SAVE_FILE_PREFIX}0"));
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ContinueButton()
    {

    }

    public void NewGameButton()
    {

    }

    public void LoadGameButton()
    {

    }

    public void SettingsToggle()
    {
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
        mainPanel.SetActive(!mainPanel.activeInHierarchy);
    }
}
