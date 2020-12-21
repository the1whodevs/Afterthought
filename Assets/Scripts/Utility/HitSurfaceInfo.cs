using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class HitSurfaceInfo : MonoBehaviour
{
    public AudioClip[] impactSoundFx;
    
    public GameObject hitEffect;
    public GameObject[] hitDecal;
        
    public GameObject RandomHitDecal => hitDecal[Random.Range(0, hitDecal.Length)];

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayImpactSound() => audioSource.PlayOneShot(impactSoundFx[Random.Range(0, impactSoundFx.Length)]);
}
