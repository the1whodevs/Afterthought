using System;
using UnityEngine;

public class PortalMenu : MonoSingleton<PortalMenu>
{
    public Action<int> onPortalAccept;

    [SerializeField] private GameObject mainPanel;

    private int lastTargetBuildIndex;

    private void Start()
    {
        mainPanel.SetActive(true);
    }

    public void ShowPortalMenu(int targetBuildIndex)
    {
        lastTargetBuildIndex = targetBuildIndex;

        Player.Active.Controller.EnterUI();

        gameObject.SetActive(true);
    }

    public void YesButton()
    {
        onPortalAccept?.Invoke(lastTargetBuildIndex);

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
