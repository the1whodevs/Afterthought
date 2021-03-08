using System.IO;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    [SerializeField] private KeyCode screenshotKey;

    [SerializeField, Header("SCREENSHOT FILE NAME ONLY!")] 
    private string screenshotPath = "SCREENSHOT";

    private int screenshotIndex = 0;

#if UNITY_EDITOR

    private void Start()
    {
        screenshotPath = SaveSystem.SAVES_DIR + screenshotPath;

        while (File.Exists($"{screenshotPath}_{screenshotIndex}{SaveSystem.SCREENSHOT_EXT}"))
        {
            screenshotIndex++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            SaveSystem.TakeScreenshot($"{screenshotPath}_{screenshotIndex}{SaveSystem.SCREENSHOT_EXT}");
            screenshotIndex++;
        }
    }

#endif
}
