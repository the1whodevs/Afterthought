using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance => FindObjectOfType<MainMenu>();

    [SerializeField] private GameObject continueButton;

    // The panel containing all main menu buttons.
    [SerializeField] private GameObject mainPanel;

    [Header("Load Settings")]
    [SerializeField, Min(0)] private int sceneIndexToLoadOnNew = 3;


    private void Awake()
    {
        mainPanel.SetActive(true);
    }

    public void UpdateContinueButton()
    {
        continueButton.SetActive(SaveManager.Active.NumOfSaves > 0);
    }

    public void ContinueButton()
    {
        SaveManager.Active.LoadLast();
    }

    public void NewGameButton()
    {
        if (!mainPanel.activeInHierarchy) return;

        mainPanel.SetActive(false);

        LoadoutEditor.Active.SetAllStartingAmmo();
        LoadingManager.Active.LoadLevel(sceneIndexToLoadOnNew);
    }

    public void LoadGameButton()
    {
        UIManager.Active.ShowLoadGamePanel();
    }

    public void ShowSettingsWindow()
    {
        SettingsManager.Active.ShowSettingsWindow();
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