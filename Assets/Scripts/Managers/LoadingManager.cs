using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoSingleton<LoadingManager>
{
    public bool Loading { get; private set; }

    public Action<int> onLoadingStarted; // sends the build index to be loaded as a parameter
    public Action onLoadingFinished;

    [SerializeField] private GameObject loadingOverlay;

    [SerializeField] private Image loadingFillImage;

    [SerializeField] private TextMeshProUGUI loadingProgressDisplay;
    [SerializeField] private TextMeshProUGUI pressAnyKeyDisplay;

    [SerializeField] private float textFadeSpeed = 5.0f;

    [SerializeField] private GameObject endLoadingButton;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        HideLoadingOverlay();
    }

    public void LoadLevel(int buildIndex)
    {
        Loading = true;

        ShowLoadingOverlay();

        if (buildIndex == 1) UIManager.Active.HideIngamePanel();

        UISoundFXManager.Active.PlayLoadingStarted();
        UISoundFXManager.Active.PlayLoadingMusic();

        StartCoroutine(LoadScene(buildIndex));  
    }

    public void ShowLoadingOverlay()
    {
        pressAnyKeyDisplay.gameObject.SetActive(false); 
        endLoadingButton.SetActive(false);
        loadingProgressDisplay.gameObject.SetActive(true);
        loadingOverlay.SetActive(true);
    }

    public void HideLoadingOverlay()
    {
        loadingOverlay.SetActive(false);

        if (Player.Active) Player.Active.Controller.ExitUI();

        Loading = false;
    }

    private IEnumerator LoadScene(int buildIndex)
    {
        var asyncOp = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = true;

        Time.timeScale = 1.0f;

        while (!asyncOp.isDone)
        {
            loadingFillImage.fillAmount = 0.1f + asyncOp.progress;
            loadingProgressDisplay.text = (100.0f * (0.1f + asyncOp.progress)).ToString("F1") + "%";

            yield return new WaitForEndOfFrame();
        }

        UISoundFXManager.Active.PlayLoadingFinished();

        endLoadingButton.SetActive(true);
        pressAnyKeyDisplay.color = Color.white;
        pressAnyKeyDisplay.gameObject.SetActive(true);
        loadingProgressDisplay.gameObject.SetActive(false);

        var t = 0.0f;

        var alphaStart = 1.0f;
        var alphaEnd = 0.0f;

        while (pressAnyKeyDisplay.gameObject.activeInHierarchy)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            t += textFadeSpeed * Time.deltaTime;

            if (Input.anyKeyDown)
            {
                HideLoadingOverlay();
                break;
            }

            pressAnyKeyDisplay.color = new Color(1, 1, 1, Mathf.Lerp(alphaStart, alphaEnd, t));

            if (t >= 1.0f)
            {
                t = 0.0f;

                var temp = alphaEnd;

                alphaEnd = alphaStart;
                alphaStart = temp;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
