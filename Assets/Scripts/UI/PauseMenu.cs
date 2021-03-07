using UnityEngine;

public class PauseMenu : MonoSingleton<PauseMenu>
{
    // The panel containing all main menu buttons.
    [SerializeField] private GameObject mainPanel;

    private void Awake()
    {
        mainPanel.SetActive(true);

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

        mainPanel.SetActive(true);
        

        gameObject.SetActive(true);
    }

    public void BackToMainMenu()
    {
        mainPanel.SetActive(false);

        Time.timeScale = 1.0f;

        gameObject.SetActive(false);

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
