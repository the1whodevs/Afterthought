using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private float switchSpeed = 1.0f;

    private Animator anim;

    private readonly int throwHash = Animator.StringToHash("throw");
    private readonly int switch_speed = Animator.StringToHash("switch_speed");

    private bool thrown = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        anim.SetFloat(switch_speed, switchSpeed);
    }

    private void Update()
    {
        if (!thrown && Input.GetMouseButton(0))
        {
            thrown = true;
            ThrowGrenade();
        }
    }

    private void ThrowGrenade()
    {
        anim.ResetTrigger(throwHash);
        anim.SetTrigger(throwHash);
    }
}
