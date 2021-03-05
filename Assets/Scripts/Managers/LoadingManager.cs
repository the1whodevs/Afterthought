using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoSingleton<LoadingManager>
{

    public Action<int> onLoadingStarted; // sends the build index to be loaded as a parameter
    public Action onLoadingFinished;

    [SerializeField] private GameObject loadingOverlay;
    [SerializeField] private Image loadingFillImage;
    [SerializeField] private TextMeshProUGUI loadingProgressDisplay;

    [SerializeField] private AudioSource loadingSFXplayer;
    [SerializeField] private AudioClip loadingStartedClip;
    [SerializeField] private AudioClip loadingFinishedClip;


    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadLevel(int buildIndex)
    {
        ShowLoadingOverlay();
        loadingSFXplayer.PlayOneShot(loadingStartedClip);
        StartCoroutine(LoadScene(buildIndex));  
    }

    public void ShowLoadingOverlay()
    {
        loadingOverlay.SetActive(true);
    }

    public void HideLoadingOverlay()
    {
        loadingOverlay.SetActive(false);
    }

    private IEnumerator LoadScene(int buildIndex)
    {
        var asyncOp = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = true;

        while (!asyncOp.isDone)
        {
            loadingFillImage.fillAmount = 0.1f + asyncOp.progress;
            loadingProgressDisplay.text = (100.0f * (0.1f + asyncOp.progress)).ToString("F1") + "%";

            yield return new WaitForEndOfFrame();
        }
        loadingSFXplayer.PlayOneShot(loadingFinishedClip);
        HideLoadingOverlay();
    }

}
