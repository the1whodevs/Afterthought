using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance
    {
        get
        {
            if (_instance) return _instance;

            _instance = FindObjectOfType<PauseMenu>();

            if (!_instance) throw new System.Exception("PauseMenu is missing!");

            return _instance;
        }
    }

    private static PauseMenu _instance;

    // The panel containing all main menu buttons.
    [SerializeField] private GameObject mainPanel;

    // The settings window.
    [SerializeField] private GameObject settingsPanel;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingImage;
    [SerializeField] private TextMeshProUGUI loadingProgress;

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
        if (_instance && _instance != this) Destroy(gameObject);

        if (!_instance) _instance = this;

        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        loadingPanel.SetActive(false);

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

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowPauseMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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

        if (int.TryParse(value, out var intVal)) return;

        horizontalSensitivityIF.SetTextWithoutNotify("5");
    }

    public void SetMouseXSensitivityInputField(string s)
    {
        s = horizontalSensitivityIF.text;

        if (int.TryParse(s, out var intVal))
        {
            var sens = intVal / 10.0f;
            PlayerPrefs.SetFloat(MainMenu.HORIZONTAL_SENS_KEY, sens);
            horizontalSensitivitySlider.SetValueWithoutNotify(intVal);
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

        if (int.TryParse(value, out var intVal)) return;

        verticalSensitivityIF.SetTextWithoutNotify("5");
    }

    public void SetMouseYSensitivityInputField(string s)
    {
        s = verticalSensitivityIF.text;

        if (int.TryParse(s, out var intVal))
        {
            var sens = intVal / 10.0f;
            PlayerPrefs.SetFloat(MainMenu.VERTICAL_SENS_KEY, sens);
            verticalSensitivitySlider.SetValueWithoutNotify(intVal);
        }
    }

    public void BackToMainMenu()
    {
        loadingPanel.SetActive(true);
        mainPanel.SetActive(false);

        Time.timeScale = 1.0f;

        StartCoroutine(LoadScene(1));
    }

    public void ExitGame()
    {
        Time.timeScale = 1.0f;

        Application.Quit();
    }


    private IEnumerator LoadScene(int buildIndex)
    {
        var asyncOp = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = true;

        while (!asyncOp.isDone)
        {
            loadingImage.fillAmount = 0.1f + asyncOp.progress;
            loadingProgress.text = (100.0f * (0.1f + asyncOp.progress)).ToString("F1") + "%";
            yield return new WaitForEndOfFrame();
        }
    }
}
