using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public PlayerAnimator Animator { get; private set; }
    public PlayerEquipment Equipment { get; private set; }
    
    public PlayerController Controller { get; private set; }

    private void Awake()
    {
        if (instance) Destroy(this);

        Equipment = GetComponent<PlayerEquipment>();
        Animator = GetComponent<PlayerAnimator>();
        Controller = GetComponent<PlayerController>();
        
        instance = this;
    }

    private void Start()
    {
        Equipment.Init();
    }
}
