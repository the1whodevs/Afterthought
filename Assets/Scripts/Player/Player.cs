using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Active;

    public PlayerAnimator Animator { get; private set; }
    public PlayerLoadout Loadout { get; private set; }
    public PlayerAudio Audio { get; private set; }
    public PlayerDamage Damage { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerPickup Pickup { get; private set; }
    public PlayerHealth Health { get; private set; }
    public PlayerExperience Experience { get; private set; }
    public PlayerPostProcessing PostProcessing { get; private set; }
    public PlayerShotgun Shotgun { get; private set; }
    public PlayerPistol Pistol { get; private set; }
    public PlayerMelee Melee { get; private set; }
    public PlayerEquipment Equipment { get; private set; }
    public PlayerCamera Camera { get; private set; }
    public PlayerObjectives Objectives { get; private set; }
    public PlayerVisor Visor { get; private set; }

    [SerializeField] private GameObject[] objectsToSpawnOnSpawn;

    private void Start()
    {
        Debug.Log("PlayerStart!");

        Active = this;

        SaveManager.Active.SetPlayerInstance(this);
    }

    public void Init()
    {
        Debug.Log("Player Init...");

        foreach (var toSpawn in objectsToSpawnOnSpawn)
        {
            var g = Instantiate(toSpawn, transform.parent);
            SceneManager.MoveGameObjectToScene(g, SceneManager.GetActiveScene());
        }

        GetReferences();

        InitComponents();

        Debug.Log("Player init completed!");
    }

    public void InitComponents()
    {
        Camera.Init();
        Audio.Init();
        Animator.Init();
        Loadout.Init();
        UIManager.Active.Init(Loadout);
        Objectives.Init();
        Damage.Init();
        Controller.Init(Audio, Animator, Loadout);
        Pickup.Init(Controller, Loadout);
        Visor.Init(Controller);
        Health.Init();
        PostProcessing.Init();
    }

    public void GetReferences()
    {
        Loadout = GetComponent<PlayerLoadout>();
        Audio = GetComponent<PlayerAudio>();
        Damage = GetComponent<PlayerDamage>();
        Animator = GetComponent<PlayerAnimator>();
        Controller = GetComponent<PlayerController>();
        Health = GetComponent<PlayerHealth>();
        Camera = GetComponentInChildren<PlayerCamera>();
        PostProcessing = GetComponent<PlayerPostProcessing>();
        Shotgun = GetComponent<PlayerShotgun>();
        Pistol = GetComponent<PlayerPistol>();
        Melee = GetComponent<PlayerMelee>();
        Equipment = GetComponent<PlayerEquipment>();
        Experience = GetComponent<PlayerExperience>();
        Pickup = GetComponent<PlayerPickup>();
        Objectives = GetComponent<PlayerObjectives>();
        Visor = GetComponent<PlayerVisor>();
    }
}