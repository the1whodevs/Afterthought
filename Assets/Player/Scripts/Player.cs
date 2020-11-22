using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public enum PlayerRace { Cyborg = 0, Human = 1 }
    
    public PlayerRace Race { get; private set; }
    
    public PlayerAnimator Animator { get; private set; }
    public PlayerEquipment Equipment { get; private set; }
    
    /// <summary>
    /// If the saved value is 0, player is cyborg.
    /// If the saved value is 1, player is human.
    /// </summary>
    public const string PLAYER_SKIN_KEY = "PLAYER_SELECTED_SKIN";
    
    private void Awake()
    {
        if (instance) Destroy(this);

        Race = (PlayerRace) PlayerPrefs.GetInt(PLAYER_SKIN_KEY, 0);

        instance = this;
    }

    private void Start()
    {
        Animator = GetComponent<PlayerAnimator>();
        Animator.Init(Race);
        
        Equipment = GetComponent<PlayerEquipment>();
        Equipment.Init();
    }
}
