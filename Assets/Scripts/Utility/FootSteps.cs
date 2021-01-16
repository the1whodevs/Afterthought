using UnityEngine;
using Random = UnityEngine.Random;

public class FootSteps : MonoBehaviour
{
    public CheckIfGrounded checkIfGrounded;
    public CheckTerrainTexture checkTerrainTexture;

    [SerializeField] private float modifier = 0.5f;
    
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] grassClips;
    [SerializeField] private AudioClip[] mudClips;
    [SerializeField] private AudioClip[] stoneClips;

    private CharacterController cc;
    
    private AudioClip previousClip;
    
    private float currentSpeed;
    private float distanceCovered;
    private float airTime;
    
    private bool isWalking;
    
    void Start()
    {
        cc = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        currentSpeed = GetPlayerSpeed();
        isWalking = CheckIfWalking();
        PlaySoundFalling();

        if (isWalking)
        {
            distanceCovered += (currentSpeed * Time.deltaTime) * modifier;
            if (distanceCovered > 1)
            {
                TriggerNextClip();
                distanceCovered = 0;
            }
        }
    }

    float GetPlayerSpeed()
    {
        float speed = cc.velocity.magnitude;
        return speed;
    }

    bool CheckIfWalking()
    {
        if (currentSpeed > 0 && checkIfGrounded.IsGrounded) return true;
        else return false;
    }

    AudioClip GetClipFromArray(AudioClip[] clipArray)
    {
        int attempts = 3;
        AudioClip selectedClip = clipArray[Random.Range(0, clipArray.Length - 1)];

        while (selectedClip == previousClip && attempts >0 )
        {
            selectedClip = clipArray[Random.Range(0, clipArray.Length - 1)];
            attempts--;
        }

        previousClip = selectedClip;
        return selectedClip;
    }

    void TriggerNextClip()
    {
        audioSource.pitch = Random.Range(0.9f,1.1f);
        audioSource.volume = Random.Range(0.0f, 1.0f);

        if (checkIfGrounded.IsOnTerrain)
        {
            checkTerrainTexture.GetTerrainTexture();
            if (checkTerrainTexture.textureValues[0]>0)
            {
                audioSource.PlayOneShot(GetClipFromArray(grassClips),checkTerrainTexture.textureValues[0]);
            }
            if (checkTerrainTexture.textureValues[1]>0)
            {
                audioSource.PlayOneShot(GetClipFromArray(mudClips),checkTerrainTexture.textureValues[1]);
            }
            if (checkTerrainTexture.textureValues[2]>0)
            {
                audioSource.PlayOneShot(GetClipFromArray(mudClips),checkTerrainTexture.textureValues[2]);
            }
            if (checkTerrainTexture.textureValues[3]>0)
            {
                audioSource.PlayOneShot(GetClipFromArray(mudClips),checkTerrainTexture.textureValues[3]);
            }
        }
        else
        {
            audioSource.PlayOneShot(GetClipFromArray(stoneClips),1);
        }
    }
    
    void PlaySoundFalling()
    {
        if (!checkIfGrounded.IsGrounded)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            if (airTime > 0.25f)
            {
                TriggerNextClip();
                airTime = 0.0f;
            }
        }
    }
}
