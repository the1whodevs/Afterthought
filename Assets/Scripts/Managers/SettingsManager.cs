using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoSingleton<SettingsManager>
{
    [SerializeField] private TMP_InputField horizontalSensInputField;
    [SerializeField] private TMP_InputField verticalSensInputField;

    [SerializeField] private Slider horizontalSensSlider;
    [SerializeField] private Slider verticalSensSlider;

    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject audioPanel;

    [SerializeField] private TextMeshProUGUI quicksaveBindingDisplay;
    [SerializeField] private TextMeshProUGUI quickloadBindingDisplay;

    private bool recordingKey = false;

    public const string HORIZONTAL_SENS_KEY = "MOUSE_X_SENSITIVITY";
    public const string VERTICAL_SENS_KEY = "MOUSE_Y_SENSITIVITY";

    private void Awake()
    {
        if (Active && Active == this) DontDestroyOnLoad(this);
        else
        {
            Debug.LogError("Another SettingsManager instance has been found!");
            Destroy(this);
        }

        var mouseX = PlayerPrefs.GetFloat(HORIZONTAL_SENS_KEY, 1.0f);
        horizontalSensInputField.SetTextWithoutNotify((10 * mouseX).ToString("F1"));
        horizontalSensSlider.SetValueWithoutNotify(10.0f * mouseX);

        var mouseY = PlayerPrefs.GetFloat(VERTICAL_SENS_KEY, 1.0f);
        verticalSensInputField.SetTextWithoutNotify((10 * mouseY).ToString("F1"));
        verticalSensSlider.SetValueWithoutNotify(10.0f * mouseY);
    }

    public void ShowSettingsWindow()
    {
        controlsPanel.SetActive(true);
        audioPanel.SetActive(false);
        settingsWindow.SetActive(true);

        UpdateBindingTexts();
    }

    private void UpdateBindingTexts()
    {
        quicksaveBindingDisplay.text = SaveManager.Active.GetQuicksaveBinding.ToString();
        quickloadBindingDisplay.text = SaveManager.Active.GetQuickloadBinding.ToString();
    }

    public void HideSettingsWindow()
    {
        settingsWindow.SetActive(false);
    }

    public void SetSensitivityX(float value)
    {
        var sens = value / 10.0f;
        PlayerPrefs.SetFloat(HORIZONTAL_SENS_KEY, sens);
        horizontalSensInputField.SetTextWithoutNotify(value.ToString("F1"));
        horizontalSensSlider.SetValueWithoutNotify(value);
    }

    public void SetSensitivityX(string value)
    {
        if (float.TryParse(value, out var floatVal))
        {
            if (floatVal > horizontalSensSlider.maxValue) floatVal = horizontalSensSlider.maxValue;

            horizontalSensInputField.SetTextWithoutNotify(floatVal.ToString("F1"));
            horizontalSensSlider.SetValueWithoutNotify(floatVal);

            return;
        }

        horizontalSensInputField.SetTextWithoutNotify("10");
    }

    public void SetSensitivityY(float value)
    {
        var sens = value / 10.0f;
        PlayerPrefs.SetFloat(VERTICAL_SENS_KEY, sens);
        verticalSensInputField.SetTextWithoutNotify(value.ToString("F1"));
        verticalSensSlider.SetValueWithoutNotify(value);
    }

    public void SetSensitivityY(string value)
    {
        if (float.TryParse(value, out var floatVal))
        {
            if (floatVal > verticalSensSlider.maxValue) floatVal = verticalSensSlider.maxValue;

            verticalSensInputField.SetTextWithoutNotify(floatVal.ToString("F1"));
            verticalSensSlider.SetValueWithoutNotify(floatVal);

            return;
        }

        verticalSensInputField.SetTextWithoutNotify("10");
    }

    public void StartRecordKeyQuicksave()
    {
        if (recordingKey) return;

        recordingKey = true;

        StartCoroutine(RecordKey(SaveManager.Active.SetQuickSaveBinding));
    }

    public void StartRecordKeyQuickload()
    {
        if (recordingKey) return;

        recordingKey = true;

        StartCoroutine(RecordKey(SaveManager.Active.SetQuickLoadBinding));
    }

    private IEnumerator RecordKey(System.Action<KeyCode> setter)
    {
        while (recordingKey)
        {
            Debug.Log("Recording key...");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                recordingKey = false;
                break;
            }

            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (vKey.CompareTo(KeyCode.Escape) == 0) continue;

                if (Input.GetKey(vKey))
                {
                    Debug.Log($"Caught {vKey} key...");

                    setter(vKey);
                    recordingKey = false;
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        UpdateBindingTexts();
    }
}