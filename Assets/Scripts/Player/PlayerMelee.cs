using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    // FOR TESTING ONLY!
    private float attackSpeed = 1.0f;
    private float weaponSwitchSpeed = 1.0f;
    
    private Animator anim;

    private readonly int attack = Animator.StringToHash("attack");
    private readonly int attackNum = Animator.StringToHash("attackNum");
    private readonly int isSprinting = Animator.StringToHash("isSprinting");
    private readonly int isMoving = Animator.StringToHash("isMoving");
    private readonly int switch_weapon = Animator.StringToHash("switch_weapon");
    private readonly int switch_speed = Animator.StringToHash("switch_speed");
    private readonly int attack_speed = Animator.StringToHash("attack_speed");

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat(switch_speed, weaponSwitchSpeed);
        anim.SetFloat(attack_speed, attackSpeed);
    }

    // Update is called once per frame
    private void Update()
    {
        anim.SetFloat(switch_speed, weaponSwitchSpeed);
        anim.SetFloat(attack_speed, attackSpeed);

        anim.SetBool(isSprinting, Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W));
        anim.SetBool(isMoving, !Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W));

        if (Input.GetMouseButton(0))
        {
            anim.SetInteger(attackNum, 0);
            anim.ResetTrigger(attack);
            anim.SetTrigger(attack);
        }
        else if (Input.GetMouseButton(1))
        { 
            anim.SetInteger(attackNum, 1);
            anim.ResetTrigger(attack);
            anim.SetTrigger(attack);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            anim.ResetTrigger(switch_weapon);
            anim.SetTrigger(switch_weapon);
        }
    }
}
