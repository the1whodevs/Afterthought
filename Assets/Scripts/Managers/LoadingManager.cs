using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoSingleton<LoadingManager>
{
    [System.Serializable]
    private class LoadingData
    {
        public int buildIndex = -1;
        public string sceneName = "";
        public string description = "";
        public Sprite loadingBackground;
        public LevelObjectiveData levelObjectiveData;
    }

    public bool Loading { get; private set; }

    public Action<int> onLoadingStarted; // sends the build index to be loaded as a parameter
    public Action onLoadingSceneLoaded;
    public Action onTargetLevelLoaded;
    public Action onLoadingFinished;

    [SerializeField] private GameObject loadingOverlay;

    [SerializeField] private Image loadingFillImage;
    [SerializeField] private Image loadingBackground;

    [SerializeField] private TextMeshProUGUI loadingProgressDisplay;
    [SerializeField] private TextMeshProUGUI loadingDescription;
    [SerializeField] private TextMeshProUGUI pressAnyKeyDisplay;

    [SerializeField] private float textFadeSpeed = 5.0f;
    [SerializeField] private float fakeLoadingSpeed = 5.0f;
    [SerializeField] private float loadingDelay = 0.25f;

    [SerializeField] private LoadingData[] loadingData;

    [SerializeField] private GameObject endLoadingButton;

    private const string LOADING_SCENE = "99 - Loading";

    private void Awake()
    {
        DontDestroyOnLoad(this);

        HideLoadingOverlay();
    }

    public void LoadLevel(string levelName)
    {
        LoadLevel(SceneManager.GetSceneByName(levelName).buildIndex);
    }

    public string GetObjectiveDescription(int buildIndex, int objectiveIndex)
    {
        return loadingData[buildIndex].levelObjectiveData.objectiveData[objectiveIndex].objectiveText;
    }

    public string GetSceneName(int buildIndex)
    {
        return loadingData[buildIndex].sceneName;
    }

    public void LoadLevel(int buildIndex)
    {
        if (Loading) return;

        Loading = true;

        loadingBackground.sprite = loadingData[buildIndex].loadingBackground;
        loadingDescription.text = loadingData[buildIndex].description;

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
        onLoadingStarted?.Invoke(buildIndex);

        Time.timeScale = 1.0f;

        var loadingSceneAsyncOp = SceneManager.LoadSceneAsync(LOADING_SCENE, LoadSceneMode.Single);

        while (!loadingSceneAsyncOp.isDone) yield return new WaitForEndOfFrame();

        onLoadingSceneLoaded?.Invoke();

        var t = 0.0f;

        while (t <= 1.0f)
        {
            t += Time.deltaTime * fakeLoadingSpeed;

            loadingFillImage.fillAmount = Mathf.Lerp(0.0f, 0.1f, t);
            loadingProgressDisplay.text = (100.0f * loadingFillImage.fillAmount).ToString("F0") + "%";

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(loadingDelay);

        var asyncOp = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = true;

        while (!asyncOp.isDone)
        {
            loadingFillImage.fillAmount = 0.1f + asyncOp.progress;
            loadingProgressDisplay.text = (100.0f * (0.1f + asyncOp.progress)).ToString("F0") + "%";

            yield return new WaitForEndOfFrame();
        }

        onTargetLevelLoaded?.Invoke();

        while (SaveManager.Active.LoadingData) yield return new WaitForEndOfFrame();

        onLoadingFinished?.Invoke();

        UISoundFXManager.Active.PlayLoadingFinished();

        UIManager.Active.HidePauseMenu();
        DeathMenu.Active.HideDeathMenu();

        // If going to the main menu, or the cinematic scene...
        if (buildIndex == 1 || buildIndex == 2)
        {
            // Just hide the loading overlay.
            HideLoadingOverlay();
            yield break;
        }

        // ... otherwise wait for player interaction.
        endLoadingButton.SetActive(true);
        pressAnyKeyDisplay.color = Color.white;
        pressAnyKeyDisplay.gameObject.SetActive(true);
        loadingProgressDisplay.gameObject.SetActive(false);

        t = 0.0f;

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
