using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GAInit : MonoBehaviour
{
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(1);
    }
}
