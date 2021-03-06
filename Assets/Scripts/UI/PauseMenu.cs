using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoSingleton<PauseMenu>
{
    // The panel containing all main menu buttons.
    [SerializeField] private GameObject mainPanel;

    // The settings window.
    [SerializeField] private GameObject settingsPanel;

    // The panels within the settings window.
    [Header("Controls Panel")]
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private Slider horizontalSensitivitySlider;
    [SerializeField] private TMP_InputField horizontalSensitivityIF;
    [SerializeField] private Slider verticalSensitivitySlider;
    [SerializeField] private TMP_InputField verticalSensitivityIF;

    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject graphicsPanel;

    private void Awake()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);

        controlsPanel.SetActive(true);
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(false);

        var mouseX = PlayerPrefs.GetFloat(MainMenu.HORIZONTAL_SENS_KEY, 1.0f);

        horizontalSensitivityIF.SetTextWithoutNotify((10 *mouseX).ToString("F0"));
        horizontalSensitivitySlider.SetValueWithoutNotify(10.0f * mouseX);

        var mouseY = PlayerPrefs.GetFloat(MainMenu.VERTICAL_SENS_KEY, 1.0f);

        verticalSensitivityIF.SetTextWithoutNotify((10 * mouseY).ToString("F0"));
        verticalSensitivitySlider.SetValueWithoutNotify(10.0f * mouseY);

        gameObject.SetActive(false);
    }

    public void ResumeButton()
    {
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);

        if (Player.Active) Player.Active.Controller.ExitUI();
    }

    public void ShowPauseMenu()
    {
        Player.Active.Controller.EnterUI();

        gameObject.SetActive(true);
    }

    public void LoadGameButton()
    {

    }

    public void SaveGameButton()
    {

    }

    public void SettingsToggle()
    {
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
        mainPanel.SetActive(!mainPanel.activeInHierarchy);
    }

    public void ShowControlsPanel()
    {
        controlsPanel.SetActive(true);
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(false);
    }

    public void ShowAudioPanel()
    {
        controlsPanel.SetActive(false);
        audioPanel.SetActive(true);
        graphicsPanel.SetActive(false);
    }

    public void ShowGraphicsPanel()
    {
        controlsPanel.SetActive(false);
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(true);
    }

    public void SetMouseXSensitivity()
    {
        var value = horizontalSensitivitySlider.value;
        var sens = (float)value / 10.0f;
        PlayerPrefs.SetFloat(MainMenu.HORIZONTAL_SENS_KEY, sens);
        horizontalSensitivityIF.SetTextWithoutNotify(value.ToString("F0"));
    }

    public void EnsureIntegerHorizontal(string value)
    {
        value = horizontalSensitivityIF.text;

        if (int.TryParse(value, out var intVal))
        {
            if (intVal / 10.0f > horizontalSensitivitySlider.maxValue)
                horizontalSensitivityIF.SetTextWithoutNotify($"{horizontalSensitivitySlider.maxValue}");
            return;
        }

        horizontalSensitivityIF.SetTextWithoutNotify("5");
    }

    public void SetMouseXSensitivityInputField(string s)
    {
        s = horizontalSensitivityIF.text;

        if (int.TryParse(s, out var intVal))
        {
            var sens = intVal / 10.0f;

            if (sens > horizontalSensitivitySlider.maxValue)
                sens = horizontalSensitivitySlider.maxValue;

            PlayerPrefs.SetFloat(MainMenu.HORIZONTAL_SENS_KEY, sens);
            horizontalSensitivitySlider.SetValueWithoutNotify(sens * 10.0f);
        }
    }

    public void SetMouseYSensitivity()
    {
        var value = verticalSensitivitySlider.value;
        var sens = (float)value / 10.0f;
        PlayerPrefs.SetFloat(MainMenu.VERTICAL_SENS_KEY, sens);
        verticalSensitivityIF.SetTextWithoutNotify(value.ToString("F0"));
    }

    public void EnsureIntegerVertical(string value)
    {
        value = verticalSensitivityIF.text;

        if (int.TryParse(value, out var intVal))
        {
            if (intVal / 10.0f > verticalSensitivitySlider.maxValue)
                verticalSensitivityIF.SetTextWithoutNotify($"{verticalSensitivitySlider.maxValue}");
            return;
        }

        verticalSensitivityIF.SetTextWithoutNotify("5");
    }

    public void SetMouseYSensitivityInputField(string s)
    {
        s = verticalSensitivityIF.text;

        if (int.TryParse(s, out var intVal))
        {
            var sens = intVal / 10.0f;


            if (sens > horizontalSensitivitySlider.maxValue)
                sens = horizontalSensitivitySlider.maxValue;


            PlayerPrefs.SetFloat(MainMenu.VERTICAL_SENS_KEY, sens);
            verticalSensitivitySlider.SetValueWithoutNotify(sens * 10.0f);
        }
    }

    public void BackToMainMenu()
    {
        mainPanel.SetActive(false);

        Time.timeScale = 1.0f;

        LoadingManager.Active.LoadLevel(1);
    }

    public void ExitGame()
    {
        Time.timeScale = 1.0f;

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
}
