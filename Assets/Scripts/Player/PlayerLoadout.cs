using System;
using System.Collections;
using System.Collections.Generic;
using EmeraldAI;
using JetBrains.Annotations;
using Knife.RealBlood.Decals;
using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    /// <summary>
    /// The current weapon equipped.
    /// </summary>
    public WeaponData CurrentWeapon { get; private set; }
    public GameObject CurrentWeaponObject { get; private set; }

    public Transform FirePoint => firePoint;

    public bool HasScope => CurrentWeapon.hasScope;
    public bool UsingEquipment => isUsingEquipment;

    public GameObject ScopeGameObject { get; private set; }
    
    /// <summary>
    /// The player's loadout.
    /// </summary>
    public LoadoutData Loadout => loadout;

    public Animator CurrentAnimator { get; private set; }

    public bool IsReloading => isReloading;

    public Action<WeaponData> OnWeaponEquipped;

    [SerializeField] private LoadoutData loadout;
    
    [SerializeField] private List<WeaponData> allWeaponData = new List<WeaponData>();
    [SerializeField] private List<EquipmentData> allEquipmentData = new List<EquipmentData>();
    [SerializeField] private List<AmmoData> allAmmoData = new List<AmmoData>();

    [SerializeField] private Transform WeaponR;
    
    [SerializeField, TagSelector] private string scopeTag;

    private EquipmentData currentEquipment;

    private GameObject currentEquipmentObject;
    private GameObject currentEquipmentThrowable;

    // This is the weapon we are going to equip or eventually have equipped.
    private WeaponData futureWeapon;
    private WeaponData lastWeapon;

    private PlayerAnimator pa;

    private UIManager uiManager;

    private Transform firePoint;

    private bool isReloading;
    private bool isUsingEquipment;

    public void Init()
    {
        uiManager = UIManager.Active;
        
        pa = Player.Active.Animator;
        
        foreach (var weaponData in allWeaponData)
        {
            weaponData.LoadData();
        }

        foreach (var equipmentData in allEquipmentData)
        {
            equipmentData.LoadData();
        }

        foreach (var ammoData in allAmmoData)
        {
            ammoData.LoadData();
        }

        EquipPrimaryWeapon();
    }

    public void ChangeActiveLoadout(LoadoutData newLoadout)
    {
        newLoadout.CopyTo(loadout);

        Unequip();
        UnequipEquipment();

        EquipPrimaryWeapon();
    }

    public bool UseEquipmentA()
    {
        return UseEquipment(loadout.Equipment[0]);
    }
    
    public bool UseEquipmentB()
    {
        return UseEquipment(loadout.Equipment[1]);
    }

    public void EquipPrimaryWeapon()
    {
        Equip(loadout.Weapons[0]);
    }

    public void EquipSecondaryWeapon()
    {
        Equip(loadout.Weapons[1]);
    }

    public bool HasTalent(TalentData toCheck)
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Equals(toCheck)) return true;
        }

        return false;
    }

    public TalentData HasIncreasedWeaponTypeTalent(WeaponData.WeaponType typeToCheck)
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.IncreaseWeaponTypeDmg &&
                talent.WeaponTypeAffected == typeToCheck) return talent;
        }

        return null;
    }

    public TalentData HasIncreasedEquipmentDamageTalent()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.IncreaseEquipmentDamage) return talent;
        }

        return null;
    }

    public TalentData HasIncreasedDamageWhileCrouching()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.IncreaseDamageWhileCrouching) return talent;
        }

        return null;
    }

    public TalentData HasIncreasedMaxHealth()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.IncreaseMaxHealth) return talent;
        }

        return null;
    }

    public TalentData HasIncreasedMaxStamina()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.IncreaseMaxStamina) return talent;
        }

        return null;
    }

    public TalentData HasNoMobilityPenalty()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.NoMobilityPenalty) return talent;
        }

        return null;
    }

    public TalentData HasMinimalVerticalRecoil()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.MinimalVerticalRecoil) return talent;
        }

        return null;
    }

    public TalentData HasMinimalHorizontalRecoil()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.MinimalHorizontalRecoil) return talent;
        }

        return null;
    }

    public TalentData HasFasterReloading()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.FasterReloading) return talent;
        }

        return null;
    }

    public TalentData HasFasterCrouchSpeed()
    {
        foreach (var talent in loadout.Talents)
        {
            if (talent.Talent == TalentData.TalentType.FasterCrouchSpeed) return talent;
        }

        return null;
    }

    public void SetAmmoUI()
    {
        uiManager.UpdateWeaponAmmoUI(CurrentWeapon);
    }

    public void Reload()
    {
        Player.Active.Audio.PlayReload(CurrentWeapon);

        isReloading = true;

        pa.Reload();
    }

    public void ResetMagazine()
    {
        isReloading = false;
        CurrentWeapon.ReloadMag(ref CurrentWeapon.ammoType.currentAmmo);
        SetAmmoUI();
    }

    public void SpawnEquipmentThrowable()
    {
        currentEquipmentThrowable = Instantiate(currentEquipment.prefabToThrow, firePoint.position, firePoint.rotation, firePoint);
    }

    public void ThrowEquipmentThrowable()
    {
        currentEquipmentThrowable.transform.parent = null;
        var rb = currentEquipmentThrowable.GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(firePoint.forward * currentEquipment.throwForce, ForceMode.Impulse);

        Player.Active.Damage.ExplodeThrownEquipment(currentEquipment, currentEquipmentThrowable);

        currentEquipment = null;
        currentEquipmentObject = null;
        currentEquipmentThrowable = null;

        EquipWeapon(lastWeapon);
    }

    private bool UseEquipment(EquipmentData toUse)
    {
        if (toUse.currentAmmo == 0 || isUsingEquipment) return false;

        toUse.currentAmmo--;
        
        if (toUse == loadout.Equipment[0]) uiManager.UpdateEquipmentAmmoCountEquipmentA(toUse);
        else uiManager.UpdateEquipmentAmmoCountEquipmentb(toUse);

        isUsingEquipment = true;
        StartCoroutine(SwitchToEquipment(toUse));

        return true;
    }

    private IEnumerator SwitchToEquipment(EquipmentData toEquip)
    {
        const float delay = 1.0f;

        pa.Unequip();

        yield return new WaitForSeconds(delay);

        Unequip();
        EquipEquipment(toEquip);
    }

    private void UnequipEquipment()
    {
        currentEquipment = null;
        if (currentEquipmentObject) Destroy(currentEquipmentObject);
    }

    private void EquipEquipment(EquipmentData toEquip)
    {
        CurrentAnimator = CurrentWeaponObject.GetComponent<Animator>();
        currentEquipment = toEquip;
        currentEquipmentObject = Instantiate(currentEquipment.prefab, WeaponR.position, WeaponR.rotation, WeaponR);

        GetFirePoint(currentEquipmentObject);
    }

    private void Equip(WeaponData weaponToEquip)
    {
        if (weaponToEquip == CurrentWeapon || futureWeapon == weaponToEquip) return;

        futureWeapon = weaponToEquip;
        
        if (CurrentWeaponObject)
        {
            StartCoroutine(SwitchWeapon(weaponToEquip));
        }
        else EquipWeapon(weaponToEquip);
    }


    private IEnumerator SwitchWeapon(WeaponData weaponToEquip)
    {
        const float delay = 1.0f;
        
        if (CurrentAnimator)
        {
            pa.Unequip();

            yield return new WaitForSeconds(delay);
        }
        
        Unequip();
        EquipWeapon(weaponToEquip);
    }

    private void Unequip()
    {
        if (CurrentWeapon) CurrentWeapon = null;
        if (CurrentWeaponObject) Destroy(CurrentWeaponObject);

        CurrentAnimator = null;
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

        GetFirePoint(CurrentWeaponObject);

        CurrentWeapon = weaponToEquip;
        lastWeapon = CurrentWeapon;
        ScopeGameObject = GameObject.FindGameObjectWithTag(scopeTag);

        isUsingEquipment = false;
        isReloading = false;
        SetAmmoUI();
        CurrentAnimator = CurrentWeaponObject.GetComponent<Animator>();
        Player.Active.Camera.currentZoom = CurrentWeapon.scopeZoom;

        OnWeaponEquipped?.Invoke(CurrentWeapon);
    }

    private void GetFirePoint(GameObject objectToLookIn)
    {
        firePoint = objectToLookIn.transform.Find("FirePoint");

        if (!firePoint)
        {
            for (var i = 0; i < objectToLookIn.transform.childCount; i++)
            {
                firePoint = objectToLookIn.transform.GetChild(i).Find("FirePoint");
                if (firePoint) break;
            }

            if (!firePoint)
            {
                for (var i = 0; i < objectToLookIn.transform.childCount; i++)
                {
                    for (var j = 0; j < objectToLookIn.transform.GetChild(i).childCount; j++)
                    {
                        firePoint = objectToLookIn.transform.GetChild(i).GetChild(j).Find("FirePoint");
                        if (firePoint) break;
                    }
                }
            }
        }
    }
}