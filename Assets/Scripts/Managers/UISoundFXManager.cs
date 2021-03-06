using UnityEngine;

public class UISoundFXManager : MonoSingleton<UISoundFXManager>
{
    [Header("Buttons SFX")]
    [SerializeField] private AudioSource uiSFXplayer;
    [SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip startGameSFX;

    [Header("Loading SFX")]
    [SerializeField] private AudioSource loadingSFXplayer;
    [SerializeField] private AudioSource loadingMusicPlayer;
    [SerializeField] private AudioClip loadingStartedClip;
    [SerializeField] private AudioClip loadingFinishedClip;

    private void Awake()
    {
        if (Active && Active == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError("Another UISoundFXManager has been found!");
            Destroy(gameObject);
        }
    }

    public void PlayHoverSFX()
    {
        uiSFXplayer.PlayOneShot(hoverSFX);
    }

    public void PlayClickSFX()
    {
        uiSFXplayer.PlayOneShot(clickSFX);
    }

    public void PlayLoadingStarted()
    {
        loadingSFXplayer.PlayOneShot(loadingStartedClip);
    }

    public void PlayLoadingMusic()
    {
        loadingMusicPlayer.Play();
    }

    public void PlayLoadingFinished()
    {
        loadingSFXplayer.PlayOneShot(loadingFinishedClip);
    }

    public void PlayStartGameSFX()
    {
        uiSFXplayer.PlayOneShot(startGameSFX);
    }
}
