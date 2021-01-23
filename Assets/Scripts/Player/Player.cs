using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public PlayerAnimator Animator { get; private set; }
    public PlayerEquipment Equipment { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerHealth Health { get; private set; }
    public PlayerExperience Experience { get; private set; }
    public PlayerPostProcessing PostProcessing { get; private set; }
    public MouseCamera Camera { get; private set; }

    [SerializeField] private GameObject[] objectsToSpawnOnSpawn;

    private void Awake()
    {
        if (Instance) Destroy(this);

        foreach (var toSpawn in objectsToSpawnOnSpawn)
        {
            Instantiate(toSpawn);
        }

        Equipment = GetComponent<PlayerEquipment>();
        Animator = GetComponent<PlayerAnimator>();
        Controller = GetComponent<PlayerController>();
        Health = GetComponent<PlayerHealth>();
        Camera = GetComponentInChildren<MouseCamera>();
        PostProcessing = GetComponent<PlayerPostProcessing>();
        Experience = GetComponent<PlayerExperience>();

        Instance = this;
    }

    private void Start()
    {
        Equipment.Init();
    }
}