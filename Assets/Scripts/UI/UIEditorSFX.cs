using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEditorSFX : MonoBehaviour
{
    [Header("Buttons SFX")]
    [SerializeField] private AudioSource UIAudioSource;
    [SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip clickSFX;

    private void Start()
    {
        UIAudioSource.GetComponent<AudioSource>();
    }
    public void PlayHoverSFX()
    {
        UIAudioSource.PlayOneShot(hoverSFX);
    }

    public void PlayClickSFX()
    {
        UIAudioSource.PlayOneShot(clickSFX);
    }
}
