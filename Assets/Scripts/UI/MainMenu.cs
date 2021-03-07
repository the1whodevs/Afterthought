using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;

    // The panel containing all main menu buttons.
    [SerializeField] private GameObject mainPanel;

    //// The settings window.
    //[SerializeField] private GameObject settingsPanel;

    [Header("Load Settings")]
    [SerializeField, Min(0)] private int sceneIndexToLoadOnNew = 3;

    //// The panels within the settings window.
    //[Header("Controls Panel")]
    //[SerializeField] private GameObject controlsPanel;
    //[SerializeField] private Slider horizontalSensitivitySlider;
    //[SerializeField] private TMP_InputField horizontalSensitivityIF;
    //[SerializeField] private Slider verticalSensitivitySlider;
    //[SerializeField] private TMP_InputField verticalSensitivityIF;

    //[SerializeField] private GameObject audioPanel;
    //[SerializeField] private GameObject graphicsPanel;

    public const string SAVE_FILE_PREFIX = "AFTERTHOUGHT_SAVE_";
    public const string HORIZONTAL_SENS_KEY = "MOUSE_X_SENSITIVITY";
    public const string VERTICAL_SENS_KEY = "MOUSE_Y_SENSITIVITY";

    private void Awake()
    {
        continueButton.SetActive(PlayerPrefs.HasKey($"{SAVE_FILE_PREFIX}0"));
        mainPanel.SetActive(true);
        //settingsPanel.SetActive(false);

        //controlsPanel.SetActive(true);
        //audioPanel.SetActive(false);
        //graphicsPanel.SetActive(false);

        var mouseX = PlayerPrefs.GetFloat(HORIZONTAL_SENS_KEY, 1.0f);

        //horizontalSensitivityIF.SetTextWithoutNotify((10 *mouseX).ToString("F0"));
        //horizontalSensitivitySlider.SetValueWithoutNotify(10.0f * mouseX);

        var mouseY = PlayerPrefs.GetFloat(VERTICAL_SENS_KEY, 1.0f);

        //verticalSensitivityIF.SetTextWithoutNotify((10 * mouseY).ToString("F0"));
        //verticalSensitivitySlider.SetValueWithoutNotify(10.0f * mouseY);
    }

    public void ContinueButton()
    {
        SaveManager.Active.LoadLast();
    }

    public void NewGameButton()
    {
        if (!mainPanel.activeInHierarchy) return;

        mainPanel.SetActive(false);

        LoadingManager.Active.LoadLevel(sceneIndexToLoadOnNew);
    }

    public void LoadGameButton()
    {
        UIManager.Active.ShowLoadGamePanel();
    }

    //public void SettingsToggle()
    //{
    //    settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
    //    mainPanel.SetActive(!mainPanel.activeInHierarchy);
    //}

    public void ShowControlsPanel()
    {
        //controlsPanel.SetActive(true);
        //audioPanel.SetActive(false);
        //graphicsPanel.SetActive(false);
    }

    public void ShowAudioPanel()
    {
        //controlsPanel.SetActive(false);
        //audioPanel.SetActive(true);
        //graphicsPanel.SetActive(false);
    }

    public void ShowGraphicsPanel()
    {
        //controlsPanel.SetActive(false);
        //audioPanel.SetActive(false);
        //graphicsPanel.SetActive(true);
    }

    public void SetMouseXSensitivity()
    {
        //var value = horizontalSensitivitySlider.value;
        //var sens = (float)value / 10.0f;
        //PlayerPrefs.SetFloat(HORIZONTAL_SENS_KEY, sens);
        //horizontalSensitivityIF.SetTextWithoutNotify(value.ToString("F0"));
    }

    public void EnsureIntegerHorizontal(string value)
    {
        //value = horizontalSensitivityIF.text;

        //if (int.TryParse(value, out var intVal))
        //{
        //    if (intVal / 10.0f > horizontalSensitivitySlider.maxValue)
        //        horizontalSensitivityIF.SetTextWithoutNotify($"{horizontalSensitivitySlider.maxValue}");
        //    return;
        //}

        //horizontalSensitivityIF.SetTextWithoutNotify("5");
    }

    public void SetMouseXSensitivityInputField(string s)
    {
        //s = horizontalSensitivityIF.text;

        //if (int.TryParse(s, out var intVal))
        //{
        //    var sens = intVal / 10.0f;

        //    if (sens > horizontalSensitivitySlider.maxValue)
        //        sens = horizontalSensitivitySlider.maxValue;

        //    PlayerPrefs.SetFloat(HORIZONTAL_SENS_KEY, sens);
        //    horizontalSensitivitySlider.SetValueWithoutNotify(sens * 10.0f);
        //}
    }

    public void SetMouseYSensitivity()
    {
        //var value = verticalSensitivitySlider.value;
        //var sens = (float)value / 10.0f;
        //PlayerPrefs.SetFloat(VERTICAL_SENS_KEY, sens);
        //verticalSensitivityIF.SetTextWithoutNotify(value.ToString("F0"));
    }

    public void EnsureIntegerVertical(string value)
    {
        //value = verticalSensitivityIF.text;

        //if (int.TryParse(value, out var intVal))
        //{
        //    if (intVal / 10.0f > verticalSensitivitySlider.maxValue)
        //        verticalSensitivityIF.SetTextWithoutNotify($"{verticalSensitivitySlider.maxValue}");
        //    return;
        //}

        //verticalSensitivityIF.SetTextWithoutNotify("5");
    }

    public void SetMouseYSensitivityInputField(string s)
    {
        //s = verticalSensitivityIF.text;

        //if (int.TryParse(s, out var intVal))
        //{
        //    var sens = intVal / 10.0f;


        //    if (sens > horizontalSensitivitySlider.maxValue)
        //        sens = horizontalSensitivitySlider.maxValue;


        //    PlayerPrefs.SetFloat(VERTICAL_SENS_KEY, sens);
        //    verticalSensitivitySlider.SetValueWithoutNotify(sens * 10.0f);
        //}
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayHoverSFX()
    {
        UISoundFXManager.Active.PlayHoverSFX();
    }

    public void PlayClickSFX()
    {
        UISoundFXManager.Active.PlayClickSFX();
    }

    public void PlayStartGameSFX()
    {
        UISoundFXManager.Active.PlayStartGameSFX();
    }
}