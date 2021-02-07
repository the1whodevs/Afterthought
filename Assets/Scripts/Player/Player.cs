using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Active;

    public PlayerAnimator Animator { get; private set; }
    public PlayerLoadout Loadout { get; private set; }
    public PlayerAudio Audio { get; private set; }
    public PlayerDamage Damage { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerHealth Health { get; private set; }
    public PlayerExperience Experience { get; private set; }
    public PlayerPostProcessing PostProcessing { get; private set; }
    public PlayerShotgun Shotgun { get; private set; }
    public PlayerPistol Pistol { get; private set; }
    public PlayerMelee Melee { get; private set; }
    public PlayerEquipment Equipment { get; private set; }
    public PlayerCamera Camera { get; private set; }

    [SerializeField] private GameObject[] objectsToSpawnOnSpawn;

    private void Awake()
    {
        if (Active) Destroy(this);

        foreach (var toSpawn in objectsToSpawnOnSpawn)
        {
            Instantiate(toSpawn);
        }

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

        Active = this;

        Audio.Init();
        Animator.Init();
        Loadout.Init();
        Damage.Init();
    }
}