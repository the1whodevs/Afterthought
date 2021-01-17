using UnityEngine;
using UnityEngine.SceneManagement;

public class GAInit : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();
        SceneManager.LoadScene(1);
    }
}
