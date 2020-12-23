using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public static CameraAnimation Instance;
    public readonly int deathAnim = Animator.StringToHash(("Death"));
    private Animator cameraAnimator;

    private void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    private void Start()
    {
        cameraAnimator = GetComponent<Animator>();
        cameraAnimator.enabled = false;
    }
    
    public void DeathAnimation()
    {
        cameraAnimator.enabled = true;
        cameraAnimator.SetBool(deathAnim,true);
    }
}
