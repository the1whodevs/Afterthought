using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalMenu : MonoSingleton<PortalMenu>
{
    public Action onPortalAccept;

    [SerializeField] private GameObject mainPanel;
   

    private void Start()
    {
        mainPanel.SetActive(true);
    }

    public void ShowPortalMenu()
    {
        Player.Active.Controller.EnterUI();

        gameObject.SetActive(true);
    }

    public void YesButton()
    {
        onPortalAccept?.Invoke();

        if (Player.Active) Player.Active.Controller.ExitUI();

        gameObject.SetActive(false);
    }

    public void NoButton()
    {
        Time.timeScale = 1.0f;

        if (Player.Active) Player.Active.Controller.ExitUI();

        gameObject.SetActive(false);
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
