using System.Collections;
using UnityEngine;

public class PlayerShotgun : MonoBehaviour
{
    // FOR TESTING ONLY!
    [SerializeField] private float attackSpeed = 0.5f;
    [SerializeField] private float reloadSpeed = 1.0f;

    [SerializeField] private int startingBullets = 8;

    private int currentBullets; 

    private float adsSpeed = 5.0f;
    private float weaponSwitchSpeed = 1.0f;

    private Animator anim;

    private readonly int attack = Animator.StringToHash("attack");
    private readonly int isSprinting = Animator.StringToHash("isSprinting");
    private readonly int isMoving = Animator.StringToHash("isMoving");
    private readonly int switch_weapon = Animator.StringToHash("switch_weapon");
    private readonly int switch_speed = Animator.StringToHash("switch_speed");
    private readonly int attack_speed = Animator.StringToHash("attack_speed");
    private readonly int reload_speed = Animator.StringToHash("reload_speed");
    private readonly int isReloading = Animator.StringToHash("isReloading");
    private readonly int lastReload = Animator.StringToHash("last_reload");

    private float targetLayerWeight = 0;
    private float startingLayerWeight = 0;

    private bool isAds;

    private void Start()
    {
        anim = GetComponent<Animator>();
        SetAnimParameters();

        currentBullets = startingBullets;

        StartCoroutine(AdjustLayerWeight());
    }

    private void SetAnimParameters()
    {
        anim.SetFloat(switch_speed, weaponSwitchSpeed);
        anim.SetFloat(attack_speed, attackSpeed);
        anim.SetFloat(reload_speed, reloadSpeed);
    }

    // Called through animation event. TODO: Move to a proper place.
    public void ReloadBullet()
    {
        currentBullets++;

        if (currentBullets == startingBullets - 1)
        {
            anim.SetBool(isReloading, false);

            anim.ResetTrigger(lastReload);
            anim.SetTrigger(lastReload);
        }
    }

    // Called through animation event. TODO: Do this properly and move to a proper place.
    public void UseBullet()
    {
        // Debug.Log("Use Bullet: " + Time.time);
    }

    private void UseAmmo()
    {
        if (currentBullets <= 0)
        {
            Debug.LogError("UseBullet called but currentBullets <= 0!");
            return;
        }

        currentBullets--;
    }

    // Update is called once per frame
    private void Update()
    {
        SetAnimParameters();

        anim.SetBool(isSprinting, Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W));
        anim.SetBool(isMoving, !Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W));

        isAds = Input.GetMouseButton(1);

        if (isAds && !anim.GetBool(isSprinting))
        {
            startingLayerWeight = anim.GetLayerWeight(1);
            targetLayerWeight = 1;
        }
        else
        {
            startingLayerWeight = anim.GetLayerWeight(1);
            targetLayerWeight = 0;
        }

        var currentlyReloading = anim.GetBool(isReloading);

        if (Input.GetKeyDown(KeyCode.R) && !currentlyReloading)
        {
            anim.SetBool(isReloading, true);
        }

        if (Input.GetMouseButton(0))
        {
            anim.ResetTrigger(attack);
            anim.SetTrigger(attack);
            UseAmmo();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.ResetTrigger(switch_weapon);
            anim.SetTrigger(switch_weapon);
        }
    }

    private IEnumerator AdjustLayerWeight()
    {
        const float tolerance = 0.001f;

        //while (!pe.CurrentAnimator) yield return new WaitForEndOfFrame();

        while (true)
        {
            //if (pe.UsingEquipment || Player.Instance.Controller.IsInUI || !pe.CurrentAnimator)
            //    yield return new WaitForEndOfFrame();
            if (Mathf.Abs(anim.GetLayerWeight(1) - targetLayerWeight) <= tolerance) yield return new WaitForEndOfFrame();
            else
            {
                var weight = Mathf.Lerp(startingLayerWeight, targetLayerWeight, Time.deltaTime * adsSpeed);
                anim.SetLayerWeight(1, weight);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
