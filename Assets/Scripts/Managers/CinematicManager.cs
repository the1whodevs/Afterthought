using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoSingleton<CinematicManager>
{
    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private float delayBefore = 1.0f;
    [SerializeField] private float delayAfter = 1.0f;

    [SerializeField] private int buildIndexToLoadAfterCinematic = 3;

    private double videoDuration;

    public void StartVideo()
    {
        videoDuration = videoPlayer.clip.length;

        Invoke("StartVideoAfterDelay", delayBefore);
    }

    private void StartVideoAfterDelay()
    {
        videoPlayer.Play();

        Invoke("LoadNextSceneAndForceSave", (float)videoDuration + delayAfter);
    }

    private void LoadNextSceneAndForceSave()
    {
        LoadingManager.Active.ForceSaveAfterFullLoad(buildIndexToLoadAfterCinematic);
    }
}
