using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public static DeathMenu Instance
    {
        get
        {
            if (_instance) return _instance;

            _instance = FindObjectOfType<DeathMenu>();

            if (!_instance) throw new System.Exception("PauseMenu is missing!");

            return _instance;
        }
    }

    private static DeathMenu _instance;

    // The panel containing all main menu buttons.
    [SerializeField] private GameObject mainPanel;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingImage;
    [SerializeField] private TextMeshProUGUI loadingProgress;

    public const string SAVE_FILE_PREFIX = "AFTERTHOUGHT_SAVE_";

    private void Awake()
    {
        if (_instance && _instance != this) Destroy(gameObject);

        if (!_instance) _instance = this;

        mainPanel.SetActive(true);
        loadingPanel.SetActive(false);

        gameObject.SetActive(false);
    }

    public void RestartButton()
    {
        // TODO: Do a load-last-save restart.
        mainPanel.SetActive(false);
        loadingPanel.SetActive(true);

        StartCoroutine(LoadScene(2));
    }

    public void ShowDeathMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(true);
    }

    public void LoadGameButton()
    {

    }

    public void BackToMainMenu()
    {
        loadingPanel.SetActive(true);
        mainPanel.SetActive(false);

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
