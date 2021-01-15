using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmeraldAI;
using JetBrains.Annotations;
using Knife.RealBlood.Decals;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerEquipment : MonoBehaviour
{
    /// <summary>
    /// The current weapon equipped.
    /// </summary>
    public WeaponData CurrentWeapon { get; private set; }
    public GameObject CurrentWeaponObject { get; private set; }

    public bool HasScope => CurrentWeapon.hasScope;
    public bool UsingEquipment => isUsingEquipment;

    public GameObject ScopeGameObject { get; private set; }
    
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

    [SerializeField] private AudioSource gunFireAudioSource;
    [SerializeField] private AudioSource gunEmptyClipAudioSource;

    [SerializeField, TagSelector] private string scopeTag;

    private EquipmentData currentEquipment;

    private GameObject currentEquipmentObject;
    private GameObject currentEquipmentThrowable;

    // This is the weapon we are going to equip or eventually have equipped.
    private WeaponData futureWeapon;
    private WeaponData lastWeapon;

    private Camera playerCamera;
    
    private PlayerAnimator pa;

    private UIManager uiManager;

    private Transform firePoint;

    private const int SHOTGUN_PELLETS = 15;
    
    private const float SHOTGUN_PELLET_VARIANCE = 5.0f;
    
    public const float BulletHoleLifetime = 10.0f;

    private bool isReloading;
    private bool isUsingEquipment;

    private const int DELAY_BETWEEN_BURSTS = 50;
    
    public void Init()
    {
        uiManager = UIManager.Instance;
        playerCamera = Camera.main;
        pa = Player.Instance.Animator;
        
        foreach (var weaponData in allWeaponData)
        {
            weaponData.LoadData();
        }

        foreach (var equipmentData in allEquipmentData)
        {
            equipmentData.LoadData();
        }

        EquipPrimaryWeapon();
    }

    public bool UseEquipmentA()
    {
        return UseEquipment(loadout.Equipment1);
    }
    
    public bool UseEquipmentB()
    {
        return UseEquipment(loadout.Equipment2);
    }

    public void EquipPrimaryWeapon()
    {
        Equip(loadout.PrimaryWeapon);
    }

    public void EquipSecondaryWeapon()
    {
        Equip(loadout.SecondaryWeapon);
    }

    public async void TryDealDamage()
    {
        const float projectileLifetime = 60.0f;

        gunFireAudioSource.pitch = CurrentWeapon.shootAudioPitch;
        gunFireAudioSource.PlayOneShot(CurrentWeapon.RandomShotAudioFX);

        var fireType = CurrentWeapon.fireType;
        
        switch (CurrentWeapon.weaponType)
        {
            case WeaponData.WeaponType.Firearm:
                
                var repeat = fireType == WeaponData.FireType.Burst ? Player.Instance.Controller.BurstFireCount : 1;
                var isShotgun = CurrentWeapon.isShotgun;
                
                for (var i = 0; i < repeat; i++)
                {
                    MouseCamera.Instance.ApplyRecoil(CurrentWeapon.recoil_horizontal, CurrentWeapon.recoil_vertical);

                    if (isShotgun)
                    {
                        ShotgunFirearmDamage();
                    }
                    else
                    {
                        // This is false when raycast does not hit.
                        if (!NonShotgunFirearmDamage()) return;
                    }
                    
                    await Task.Delay(DELAY_BETWEEN_BURSTS);
                }
                
                break;
            
            case WeaponData.WeaponType.Projectile:

                MouseCamera.Instance.ApplyRecoil(CurrentWeapon.recoil_horizontal, CurrentWeapon.recoil_vertical);

                ProjectileDamage(projectileLifetime);
                break;
            
            case WeaponData.WeaponType.Melee:
                 MeleeDamage();
                 break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MeleeDamage()
    {
        var meleeRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        Debug.DrawLine(meleeRay.origin, meleeRay.direction * CurrentWeapon.maxRange, Color.blue, 1.0f, true);

        if (!Physics.Raycast(meleeRay, out var meleeHit, CurrentWeapon.maxRange, hitScanLayerMask)) return;

        var emeraldAISystem = meleeHit.transform.GetComponent<EmeraldAISystem>();

        // If we hit an AI, damage it.
        if (emeraldAISystem && emeraldAISystem.enabled)
            emeraldAISystem.Damage((int) CurrentWeapon.weaponDamage, EmeraldAISystem.TargetType.Player, transform, 1000);
        // Otherwise just spawn a bullet hole.
        else
        {
            var cdp = meleeHit.transform.GetComponent<CharacterDamagePainter>();
            var hitSurfaceInfo = meleeHit.transform.GetComponent<HitSurfaceInfo>();

            var hitRb = meleeHit.rigidbody;

            if (hitRb) hitRb.AddForce(-meleeHit.normal * 100.0f, ForceMode.Impulse);

            if (!hitSurfaceInfo) hitSurfaceInfo = meleeHit.transform.GetComponentInParent<HitSurfaceInfo>();
            if (!cdp) cdp = meleeHit.transform.GetComponentInParent<CharacterDamagePainter>();

            Destroy(
                hitSurfaceInfo
                    ? Instantiate(hitSurfaceInfo.hitEffect, meleeHit.point,
                        Quaternion.LookRotation(meleeHit.normal),
                        meleeHit.transform)
                    : Instantiate(CurrentWeapon.hitImpact, meleeHit.point,
                        Quaternion.LookRotation(meleeHit.normal),
                        null), BulletHoleLifetime);

            if (cdp)
            {
                cdp.Paint(meleeHit.point, meleeHit.normal);
            }
            else
            {
                Destroy(
                    hitSurfaceInfo
                        ? Instantiate(hitSurfaceInfo.RandomHitDecal,
                            meleeHit.point + meleeHit.normal * Random.Range(0.001f, 0.002f),
                            Quaternion.LookRotation(meleeHit.normal),
                            meleeHit.transform)
                        : Instantiate(CurrentWeapon.RandomHitDecal,
                            meleeHit.point + meleeHit.normal * Random.Range(0.001f, 0.002f),
                            Quaternion.LookRotation(meleeHit.normal),
                            null), BulletHoleLifetime);
            }

            if (hitSurfaceInfo) hitSurfaceInfo.PlayImpactSound();
        }
    }

    private void ProjectileDamage(float projectileLifetime)
    {
        SetAmmoUI();
        var lookRot = Quaternion.LookRotation(firePoint.forward);
        var bullet = Instantiate(CurrentWeapon.projectilePrefab, firePoint.position, lookRot, null);
        bullet.GetComponent<Rigidbody>()
            .AddForce(bullet.transform.forward * CurrentWeapon.projectileSpeed, ForceMode.Impulse);

        var crossbowBolt = bullet.GetComponent<CrossbowBolt>();
        if (crossbowBolt) crossbowBolt.Init(CurrentWeapon, transform.position);

        var launcherGrenade = bullet.GetComponent<LauncherGrenade>();
        if (launcherGrenade) launcherGrenade.Init(CurrentWeapon);

        Destroy(bullet, projectileLifetime);
    }

    private void ShotgunFirearmDamage()
    {
        SetAmmoUI();

        var distance = CurrentWeapon.minRange;

        var cam = Player.Instance.Camera.transform;
        
        for (var i = 0; i < SHOTGUN_PELLETS; i++) 
        {
            var v3Offset = cam.up * Random.Range(0.0f, SHOTGUN_PELLET_VARIANCE);
            v3Offset = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), cam.forward) * v3Offset;
            
            // Dir
            var v3Hit = cam.forward * distance + v3Offset;
            
            var ray = new Ray(cam.position, v3Hit);
            
            Debug.DrawLine(ray.origin, ray.direction * CurrentWeapon.maxRange, Color.blue, 1.0f, true);
            
            if (!Physics.Raycast(ray, out var hit, CurrentWeapon.maxRange, hitScanLayerMask)) continue;
   
            var emeraldAIsys = hit.transform.GetComponent<EmeraldAISystem>();

            // If we hit an AI, damage it.
            if (emeraldAIsys && emeraldAIsys.enabled)
                emeraldAIsys.Damage((int) CurrentWeapon.weaponDamage, EmeraldAISystem.TargetType.Player, transform, 1000);
            // Otherwise just spawn a bullet hole.
            else
            {
                var cdp = hit.transform.GetComponent<CharacterDamagePainter>();
                var hitSurfaceInfo = hit.transform.GetComponent<HitSurfaceInfo>();

                var hitRb = hit.rigidbody;

                if (hitRb) hitRb.AddForce(-hit.normal * 100.0f, ForceMode.Impulse);

                if (!hitSurfaceInfo) hitSurfaceInfo = hit.transform.GetComponentInParent<HitSurfaceInfo>();
                if (!cdp) cdp = hit.transform.GetComponentInParent<CharacterDamagePainter>();

                Destroy(
                    hitSurfaceInfo
                        ? Instantiate(hitSurfaceInfo.hitEffect, hit.point, Quaternion.LookRotation(hit.normal),
                            hit.transform)
                        : Instantiate(CurrentWeapon.hitImpact, hit.point, Quaternion.LookRotation(hit.normal),
                            null), BulletHoleLifetime);

                if (cdp)
                {
                    cdp.Paint(hit.point, hit.normal);
                }
                else
                {
                    Destroy(
                        hitSurfaceInfo
                            ? Instantiate(hitSurfaceInfo.RandomHitDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f),
                                Quaternion.LookRotation(hit.normal),
                                hit.transform)
                            : Instantiate(CurrentWeapon.RandomHitDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f),
                                Quaternion.LookRotation(hit.normal),
                                null), BulletHoleLifetime);
                }

                if (hitSurfaceInfo) hitSurfaceInfo.PlayImpactSound();
            }
        }
    }
    
    private bool NonShotgunFirearmDamage()
    {
        SetAmmoUI();

        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        Debug.DrawLine(ray.origin, ray.direction * CurrentWeapon.maxRange, Color.blue, 1.0f, true);

        if (!Physics.Raycast(ray, out var hit, CurrentWeapon.maxRange, hitScanLayerMask)) return false;

        var emeraldAIsys = hit.transform.GetComponent<EmeraldAISystem>();

        // If we hit an AI, damage it.
        if (emeraldAIsys && emeraldAIsys.enabled)
            emeraldAIsys.Damage((int) CurrentWeapon.weaponDamage, EmeraldAISystem.TargetType.Player, transform, 1000);

        // Otherwise just spawn a bullet hole.
        else
        {
            var cdp = hit.transform.GetComponent<CharacterDamagePainter>();
            var hitSurfaceInfo = hit.transform.GetComponent<HitSurfaceInfo>();

            var hitRb = hit.rigidbody;

            if (hitRb) hitRb.AddForce(-hit.normal * 100.0f, ForceMode.Impulse);

            if (!hitSurfaceInfo) hitSurfaceInfo = hit.transform.GetComponentInParent<HitSurfaceInfo>();
            if (!cdp) cdp = hit.transform.GetComponentInParent<CharacterDamagePainter>();

            Destroy(
                hitSurfaceInfo
                    ? Instantiate(hitSurfaceInfo.hitEffect, hit.point, Quaternion.LookRotation(hit.normal),
                        hit.transform)
                    : Instantiate(CurrentWeapon.hitImpact, hit.point, Quaternion.LookRotation(hit.normal),
                        null), BulletHoleLifetime);

            if (cdp)
            {
                cdp.Paint(hit.point, hit.normal);
            }
            else
            {
                Destroy(
                    hitSurfaceInfo
                        ? Instantiate(hitSurfaceInfo.RandomHitDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f),
                            Quaternion.LookRotation(hit.normal),
                            hit.transform)
                        : Instantiate(CurrentWeapon.RandomHitDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f),
                            Quaternion.LookRotation(hit.normal),
                            null), BulletHoleLifetime);
            }

            if (hitSurfaceInfo) hitSurfaceInfo.PlayImpactSound();
        }

        return true;
    }

    public void SetAmmoUI()
    {
        uiManager.SetAmmoUI(CurrentWeapon.currentAmmo, ammoAvailable);
    }

    public void PlayEmptyClipSound()
    {
        if (gunEmptyClipAudioSource.isPlaying) return;

        gunEmptyClipAudioSource.clip = CurrentWeapon.emptyClipAudioFx;
        gunEmptyClipAudioSource.pitch = 1.0f;
        gunEmptyClipAudioSource.Play();
    }

    public void Reload()
    {
        gunFireAudioSource.pitch = 1.0f;
        gunFireAudioSource.PlayOneShot(CurrentWeapon.reloadAudioFx);

        isReloading = true;
        pa.Reload();
    }

    public void ResetMagazine()
    {
        CurrentWeapon.ReloadMag(ref ammoAvailable);
        isReloading = false;
        SetAmmoUI();
    }

    [UsedImplicitly]
    // Used through an AnimationEvent to disable the melee weapons colliders.
    public void ResetAttackCollider()
    {
        CurrentWeaponObject.GetComponentInChildren<Collider>().enabled = false;
    }

    public void Muzzle()
    {
        const float muzzleLifetime = 2.0f;
        Destroy(Instantiate(CurrentWeapon.muzzleEffect, firePoint.position, firePoint.rotation, null), muzzleLifetime);
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

        StartCoroutine(ExplodeThrowable(currentEquipment, currentEquipmentThrowable));

        currentEquipment = null;
        currentEquipmentObject = null;
        currentEquipmentThrowable = null;

        EquipWeapon(lastWeapon);
    }

    private IEnumerator ExplodeThrowable(EquipmentData relatedEquipment, GameObject toExplode)
    {
        yield return new WaitForSeconds(relatedEquipment.fuseTime);

        var pos = toExplode.transform.position;
        Destroy(toExplode);
        Destroy(Instantiate(relatedEquipment.explosionPrefab, pos, Quaternion.identity, null), 3.0f);

        var hits = Physics.SphereCastAll(pos, relatedEquipment.maxRange, Vector3.down, hitScanLayerMask);

        if (hits.Length == 0) yield break;

        foreach (var hit in hits)
        {
            var distance = Vector3.Distance(hit.transform.position, pos);
            distance -= relatedEquipment.minRange;

            if (distance < 0.0f) distance = 0.0f;

            var damage = (int)Mathf.Lerp(relatedEquipment.damage, 0.0f, 1.0f - distance / relatedEquipment.maxRange);

            var emeraldAIsys = hit.transform.GetComponent<EmeraldAISystem>();

            // If we hit an AI, damage it.
            if (emeraldAIsys && emeraldAIsys.enabled)
            {
                emeraldAIsys.Damage(damage, EmeraldAISystem.TargetType.Player, transform, (int)relatedEquipment.ragdollForce);
            }
            // Otherwise just spawn a bullet hole.
            else
            {
                var cdp = hit.transform.GetComponent<CharacterDamagePainter>();
                var hitSurfaceInfo = hit.transform.GetComponent<HitSurfaceInfo>();
                var hitRb = hit.rigidbody;

                if (hitRb) hitRb.AddForce(-hit.normal * relatedEquipment.ragdollForce, ForceMode.Impulse);

                if (!hitSurfaceInfo) hitSurfaceInfo = hit.transform.GetComponentInParent<HitSurfaceInfo>();

                if (!cdp) cdp = hit.transform.GetComponentInParent<CharacterDamagePainter>();

                if (cdp)
                {
                    cdp.Paint(hit.point, hit.normal);
                }
                //else
                //{
                //    Destroy(
                //        hitSurfaceInfo
                //            ? Instantiate(hitSurfaceInfo.RandomExplosionDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f), Quaternion.LookRotation(hit.normal),
                //                hit.transform)
                //            : Instantiate(relatedEquipment.explosionDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f), Quaternion.LookRotation(hit.normal),
                //                null), BulletHoleLifetime);
                //}

                if (hitSurfaceInfo) hitSurfaceInfo.PlayImpactSound();
            }
        } 
    }

    private bool UseEquipment(EquipmentData toUse)
    {
        if (toUse.currentAmmo == 0 || isUsingEquipment) return false;

        toUse.currentAmmo--;
        isUsingEquipment = true;
        StartCoroutine(SwitchToEquipment(toUse));

        return true;
    }

    private IEnumerator SwitchToEquipment(EquipmentData toEquip)
    {
        const float delay = 1.0f;

        var hash = Player.Instance.Animator.unequipAnimHash;

        CurrentAnimator.ResetTrigger(hash);
        CurrentAnimator.SetTrigger(hash);

        yield return new WaitForSeconds(delay);

        Unequip();
        EquipEquipment(toEquip);
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

        var hash = Player.Instance.Animator.unequipAnimHash;
        
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

        GetFirePoint(CurrentWeaponObject);

        CurrentWeapon = weaponToEquip;
        lastWeapon = CurrentWeapon;
        ScopeGameObject = GameObject.FindGameObjectWithTag(scopeTag);

        isUsingEquipment = false;
        isReloading = false;
        SetAmmoUI();
        CurrentAnimator = CurrentWeaponObject.GetComponent<Animator>();
        Player.Instance.Camera.currentZoom = CurrentWeapon.scopeZoom;
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