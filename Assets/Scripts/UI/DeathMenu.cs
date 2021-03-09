using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoSingleton<DeathMenu>
{
    // The panel containing all main menu buttons.
    [SerializeField] private GameObject mainPanel;

    public const string SAVE_FILE_PREFIX = "AFTERTHOUGHT_SAVE_";

    private void Awake()
    {
        HideDeathMenu();
    }

    public void HideDeathMenu()
    {
        mainPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void RestartLevelButton()
    {
        HideDeathMenu();

        LoadingManager.Active.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowDeathMenu()
    {
        Player.Active.Controller.EnterUI();

        gameObject.SetActive(true);
    }

    public void BackToMainMenu()
    {
        HideDeathMenu();

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
