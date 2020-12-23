using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public PlayerAnimator Animator { get; private set; }
    public PlayerEquipment Equipment { get; private set; }
    
    public PlayerController Controller { get; private set; }
    
    public PlayerHealth Health { get; private set; }
    
    public MouseCamera Camera { get; private set; }
    
    public PlayerPostProcessing PostProcessing { get; private set; }

    private void Awake()
    {
        if (instance) Destroy(this);

        Equipment = GetComponent<PlayerEquipment>();
        Animator = GetComponent<PlayerAnimator>();
        Controller = GetComponent<PlayerController>();
        Health = GetComponent<PlayerHealth>();
        Camera = GetComponentInChildren<MouseCamera>();
        PostProcessing = GetComponent<PlayerPostProcessing>();
        
        instance = this;
    }

    private void Start()
    {
        Equipment.Init();
    }
}
