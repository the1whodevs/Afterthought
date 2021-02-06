using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource gunAudioSource;
    [SerializeField] private AudioSource footstepAudioSource;

    [Header("Footsteps")]

    [SerializeField] private float footstepSpeed = 0.5f;

    [SerializeField] private AudioClip[] grassClips;
    [SerializeField] private AudioClip[] mudClips;
    [SerializeField] private AudioClip[] stoneClips;

    private float distanceCovered;
    private float airTime;

    private bool isWalking;

    private AudioClip previousClip;

    private PlayerController pc;

    public void Init()
    {
        pc = Player.Active.Controller;
    }

    private void Update()
    {
        var isGrounded = pc.IsGrounded;
        var rbSpeed = pc.MoveSpeed;

        isWalking = isGrounded && rbSpeed > 0.0f;

        CheckFalling(isGrounded);

        if (isWalking)
        {
            distanceCovered += (rbSpeed * Time.deltaTime) * footstepSpeed;

            if (distanceCovered > 1)
            {
                TriggerNextClip();
                distanceCovered = 0;
            }
        }
    }

    public void PlayGunshot(WeaponData weapon)
    {
        gunAudioSource.pitch = weapon.shootAudioPitch;
        gunAudioSource.PlayOneShot(weapon.RandomShotAudioFX);
    }

    public void PlayReload(WeaponData weapon)
    {
        gunAudioSource.pitch = 1.0f;
        gunAudioSource.PlayOneShot(weapon.reloadAudioFx);
    }

    public void PlayEmptyClip(WeaponData weapon)
    {
        if (gunAudioSource.isPlaying) return;

        gunAudioSource.clip = weapon.emptyClipAudioFx;
        gunAudioSource.pitch = 1.0f;
        gunAudioSource.Play();
    }

    private void CheckFalling(bool isGrounded)
    {
        if (!isGrounded) airTime += Time.deltaTime;
        else if (airTime > 0.25f)
        {
            TriggerNextClip();
            airTime = 0.0f;
        }
    }

    private AudioClip GetClipFromArray(AudioClip[] clipArray)
    {
        var clips = new List<AudioClip>();

        for (var i = 0; i < clipArray.Length; i++) clips.Add(clipArray[i]);

        clips.Remove(previousClip);

        previousClip = clips[Random.Range(0, clips.Count)];
        return previousClip;
    }

    private void TriggerNextClip()
    {
        footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);
        footstepAudioSource.volume = Random.Range(0.0f, 1.0f);

        if (pc.IsOnTerrain)
        {
            var values = pc.GetTerrainTexture();

            if (values[0] > 0)
                footstepAudioSource.PlayOneShot(GetClipFromArray(grassClips), values[0]);
            
            if (values[1] > 0)
                footstepAudioSource.PlayOneShot(GetClipFromArray(mudClips), values[1]);

            if (values[2] > 0)
                footstepAudioSource.PlayOneShot(GetClipFromArray(mudClips), values[2]);

            if (values[3] > 0)
                footstepAudioSource.PlayOneShot(GetClipFromArray(mudClips), values[3]);
        }
        else
        {
            footstepAudioSource.PlayOneShot(GetClipFromArray(stoneClips), 1);
        }
    }
}
