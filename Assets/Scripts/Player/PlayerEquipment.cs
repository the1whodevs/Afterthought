using System;
using System.Collections;
using System.Collections.Generic;
using EmeraldAI;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    /// <summary>
    /// The current weapon equipped.
    /// </summary>
    public WeaponData CurrentWeapon { get; private set; }
    public GameObject CurrentWeaponObject { get; private set; }

    /// <summary>
    /// The player's loadout.
    /// </summary>
    public LoadoutData Loadout => loadout;

    public Animator CurrentAnimator { get; private set; }

    public bool IsReloading => isReloading;

    public int ammoAvailable = 270;
    
    [SerializeField] private LoadoutData loadout;
    
    [SerializeField] private List<WeaponData> allWeaponData = new List<WeaponData>();
    [SerializeField] private List<EquipmentData> allEquipmentData = new List<EquipmentData>();
    [SerializeField] private List<TalentData> allTalentData = new List<TalentData>();

    [SerializeField] private Transform WeaponR;
    
    [SerializeField] private LayerMask hitScanLayerMask;

    private Camera playerCamera;
    
    private PlayerAnimator pa;

    private UIManager uiManager;

    private Transform firePoint;

    private const float MAX_FIRE_DIST = 1000.0f;

    private bool isReloading;
    
    public void Init()
    {
        uiManager = UIManager.Instance;
        playerCamera = Camera.main;
        pa = Player.instance.Animator;
        
        foreach (var weaponData in allWeaponData)
        {
            weaponData.LoadData();
        }
        
        EquipPrimaryWeapon();
    }

    public void EquipPrimaryWeapon()
    {
        Equip(loadout.PrimaryWeapon);
    }

    public void EquipSecondaryWeapon()
    {
        Equip(loadout.SecondaryWeapon);
    }

    public void TryDealDamage()
    {
        const float bulletHoleLifetime = 10.0f;
        const float projectileLifetime = 60.0f;
        
        switch (CurrentWeapon.weaponType)
        {
            case WeaponData.WeaponType.Firearm:
                uiManager.SetAmmoUI(CurrentWeapon.currentAmmo, ammoAvailable);
                var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                
                if (!Physics.Raycast(ray, out var hit, MAX_FIRE_DIST, hitScanLayerMask)) return;
                
                //Debug.DrawLine(ray.origin, hit.point, Color.blue, 1.0f, true);
                var emeraldAIsys = hit.transform.GetComponent<EmeraldAISystem>();

                // If we hit an AI, damage it.
                if (emeraldAIsys) emeraldAIsys.Damage((int)CurrentWeapon.weaponDamage, EmeraldAISystem.TargetType.Player, transform, 1000);
                // Otherwise just spawn a bullet hole.
                else Destroy(Instantiate(CurrentWeapon.bulletHole, hit.point, Quaternion.LookRotation(hit.normal), null), bulletHoleLifetime);
                break;
            
            case WeaponData.WeaponType.Projectile:
                uiManager.SetAmmoUI(CurrentWeapon.currentAmmo, ammoAvailable);
                var lookRot = Quaternion.LookRotation(firePoint.forward);
                var bullet = Instantiate(CurrentWeapon.projectilePrefab, firePoint.position, lookRot, null);
                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * CurrentWeapon.projectileSpeed, ForceMode.Impulse);
                Destroy(bullet, projectileLifetime);
                break;
            
            case WeaponData.WeaponType.Melee:
                CurrentWeaponObject.GetComponentInChildren<Collider>().enabled = true;
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Reload()
    {
        isReloading = true;
        pa.Reload();
    }

    public void ResetMagazine()
    {
        CurrentWeapon.ReloadMag(ref ammoAvailable);
        isReloading = false;
        uiManager.SetAmmoUI(CurrentWeapon.currentAmmo, ammoAvailable);
    }

    [UsedImplicitly]
    // Used through an AnimationEvent to disable the melee weapons colliders.
    public void ResetAttackCollider()
    {
        CurrentWeaponObject.GetComponentInChildren<Collider>().enabled = false;
    }
    
    private void Equip(WeaponData weaponToEquip)
    {
        if (weaponToEquip == CurrentWeapon) return;

        if (CurrentWeaponObject)
        {
            StartCoroutine(SwitchWeapon(weaponToEquip));
        }
        else EquipWeapon(weaponToEquip);
    }

    private IEnumerator SwitchWeapon(WeaponData weaponToEquip)
    {
        const float delay = 1.0f;

        var hash = Player.instance.Animator.unequipAnimHash;
        
        CurrentAnimator.ResetTrigger(hash);
        CurrentAnimator.SetTrigger(hash);
        
        yield return new WaitForSeconds(delay);
        
        Unequip();
        EquipWeapon(weaponToEquip);
    }

    private void Unequip()
    {
        if (CurrentWeapon) CurrentWeapon = null;
        if (CurrentWeaponObject) Destroy(CurrentWeaponObject);
    }

    /// <summary>
    /// This should be called AFTER the specified hand has unequipped any weapons,
    /// and checking that the player CAN equip on that hand.
    /// </summary>
    /// <param name="handToEquipOn"></param>
    /// <param name="weaponToEquip"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void EquipWeapon(WeaponData weaponToEquip)
    {
        CurrentWeaponObject = Instantiate(weaponToEquip.wepPrefab, WeaponR.position, WeaponR.rotation, WeaponR);
        firePoint = CurrentWeaponObject.transform.Find("FirePoint");
        CurrentWeapon = weaponToEquip;
        uiManager.SetAmmoUI(CurrentWeapon.currentAmmo, ammoAvailable);
        CurrentAnimator = CurrentWeaponObject.GetComponent<Animator>();
    }
}