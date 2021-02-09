using EmeraldAI;
using Knife.RealBlood.Decals;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private int ragdollForce_Shotgun = 1500;
    [SerializeField] private int ragdollForce_NonShotgun = 750;
    [SerializeField] private int ragdollForce_Melee = 250;

    [SerializeField] private LayerMask hitScanLayerMask;

    private WeaponData CurrentWeapon => Player.Active.Loadout.CurrentWeapon;

    private Camera playerCamera;

    private Transform firePoint => Player.Active.Loadout.FirePoint;

    private UIManager uiManager;

    public const float BULLET_HOLE_LIFETIME = 10.0f;
    public const float SHOTGUN_PELLET_VARIANCE = 5.0f;
    public const float PROJECTILE_LIFETIME = 60.0f;
    public const float MUZZLE_LIFETIME = 2.0f;

    public const int SHOTGUN_PELLETS = 15;
    public const int DELAY_BETWEEN_BURSTS = 50;
    public const int BURST_FIRE_COUNT = 3;

    public void Init()
    {
        uiManager = UIManager.Active;
        playerCamera = Camera.main;
    }

    private void SetAmmoUI()
    {
        uiManager.UpdateWeaponAmmoUI(CurrentWeapon);
    }

    private void MeleeDamage()
    {
        HitscanDamage(playerCamera.transform.forward, ragdollForce_Melee);
    }

    private void ShotgunFirearmDamage()
    {
        SetAmmoUI();

        var distance = CurrentWeapon.minRange;

        var cam = Player.Active.Camera.transform;

        for (var i = 0; i < SHOTGUN_PELLETS; i++)
        {
            var v3Offset = cam.up * Random.Range(0.0f, SHOTGUN_PELLET_VARIANCE);
            v3Offset = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), cam.forward) * v3Offset;

            // Dir
            var v3Hit = cam.forward * distance + v3Offset;

            HitscanDamage(v3Hit, ragdollForce_Shotgun);
        }

        Player.Active.Camera.ApplyRecoil(CurrentWeapon.recoil_horizontal, CurrentWeapon.recoil_vertical);
    }

    private void NonShotgunFirearmDamage()
    {
        SetAmmoUI();

        HitscanDamage(playerCamera.transform.forward, ragdollForce_NonShotgun);

        Player.Active.Camera.ApplyRecoil(CurrentWeapon.recoil_horizontal, CurrentWeapon.recoil_vertical);

        return;
    }

    private void FireProjectile()
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

        Player.Active.Camera.ApplyRecoil(CurrentWeapon.recoil_horizontal, CurrentWeapon.recoil_vertical);

        Destroy(bullet, PROJECTILE_LIFETIME);
    }

    private IEnumerator BurstFire(WeaponData CurrentWeapon ,int numOfBursts)
    {
        if (CurrentWeapon.isShotgun)
        {
            for (var i = 0; i < numOfBursts; i++)
            {
                ShotgunFirearmDamage();
                yield return new WaitForSeconds((1f/1000f) * DELAY_BETWEEN_BURSTS);
            }
        }
        else
        {
            for (var i = 0; i < numOfBursts; i++)
            {
                NonShotgunFirearmDamage();
                yield return new WaitForSeconds((1f / 1000f) * DELAY_BETWEEN_BURSTS);
            }
        }
    }

    private IEnumerator ExplodeThrowable(EquipmentData relatedEquipment, GameObject toExplode)
    {
        yield return new WaitForSeconds(relatedEquipment.fuseTime);

        Player.Active.Loadout.ThrowableExploded(toExplode);

        var pos = toExplode.transform.position;

        Destroy(toExplode);
        Destroy(Instantiate(relatedEquipment.explosionPrefab, pos, Quaternion.identity, null), relatedEquipment.explosionLifetime);

        switch (relatedEquipment.Type)
        {
            case EquipmentData.EquipmentType.DmgExplosion:
                DmgExplosion(pos, relatedEquipment);
                break;

            case EquipmentData.EquipmentType.OccludeExplosion:
                // TODO: Blind all AI & Player for explosionLifetime.
                break;

            case EquipmentData.EquipmentType.BlockExplosion:
                // TODO: Smoke logic?
                break;

        }
    }

    private void DmgExplosion(Vector3 pos, EquipmentData relatedEquipment)
    {
        var hits = Physics.SphereCastAll(pos, relatedEquipment.maxRange, Vector3.down, hitScanLayerMask);

        if (hits.Length == 0) return;

        foreach (var hit in hits)
        {
            var distance = Vector3.Distance(hit.transform.position, pos);
            distance -= relatedEquipment.minRange;

            if (distance < 0.0f) distance = 0.0f;

            var dmg = relatedEquipment.damage;
            var talent = Player.Active.Loadout.HasIncreasedEquipmentDamageTalent();
            if (talent) dmg *= talent.value;

            talent = Player.Active.Loadout.HasIncreasedDamageWhileCrouching();
            if (talent && Player.Active.Controller.IsCrouching) dmg *= talent.value;

            var damage = (int)Mathf.Lerp(dmg, 0.0f, distance / relatedEquipment.maxRange);

            var emeraldAIsys = hit.transform.GetComponent<EmeraldAISystem>();
            var playerHealth = hit.transform.GetComponent<PlayerHealth>();

            // If we hit an AI, damage it.
            if (emeraldAIsys && emeraldAIsys.enabled)
            {
                emeraldAIsys.Damage(damage, EmeraldAISystem.TargetType.Player, transform, (int)relatedEquipment.ragdollForce);
            }
            else if (playerHealth)
            {
                playerHealth.DamagePlayer(damage);
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

    private void HitscanDamage(Vector3 scanDir, int ragdollForce)
    {
        var ray = new Ray(playerCamera.transform.position, scanDir);

        Debug.DrawLine(ray.origin, ray.direction * CurrentWeapon.maxRange, Color.blue, 1.0f, true);

        if (!Physics.Raycast(ray, out var hit, CurrentWeapon.maxRange, hitScanLayerMask)) return;

        var emeraldAIsys = hit.transform.GetComponent<EmeraldAISystem>();

        var dmg = CurrentWeapon.weaponDamage;
        var talent = Player.Active.Loadout.HasIncreasedWeaponTypeTalent(CurrentWeapon.weaponType);

        if (talent) dmg *= talent.value;

        talent = Player.Active.Loadout.HasIncreasedDamageWhileCrouching();
        if (talent && Player.Active.Controller.IsCrouching) dmg *= talent.value;

        // If we hit an AI, damage it.
        if (emeraldAIsys && emeraldAIsys.enabled)
            emeraldAIsys.Damage((int)dmg, EmeraldAISystem.TargetType.Player, transform, ragdollForce);

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
                        null), BULLET_HOLE_LIFETIME);

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
                            null), BULLET_HOLE_LIFETIME);
            }

            if (hitSurfaceInfo) hitSurfaceInfo.PlayImpactSound();
        }
    }

    /// <summary>
    /// Called through animation event from each weapon.
    /// </summary>
    public void TryDealDamage()
    {
        // Play shot effect.
        Player.Active.Audio.PlayGunshot(CurrentWeapon);

        if (CurrentWeapon.fireType == WeaponData.FireType.Burst) CurrentWeapon.ammoInMagazine -= BURST_FIRE_COUNT;
        else CurrentWeapon.ammoInMagazine--;

        var fireType = CurrentWeapon.fireType;

        switch (CurrentWeapon.weaponType)
        {
            case WeaponData.WeaponType.Firearm:
                if (fireType == WeaponData.FireType.Burst) StartCoroutine(BurstFire(CurrentWeapon, BURST_FIRE_COUNT));
                else
                {
                    if (CurrentWeapon.isShotgun) ShotgunFirearmDamage();
                    else NonShotgunFirearmDamage();
                }
                break;

            case WeaponData.WeaponType.Projectile:
                FireProjectile();
                break;

            case WeaponData.WeaponType.Melee:
                MeleeDamage();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void StartThrowableFuse(EquipmentData equipment, GameObject thrownPrefab)
    {
        StartCoroutine(ExplodeThrowable(equipment, thrownPrefab));
    }

    public void Muzzle()
    {    
        Destroy(Instantiate(CurrentWeapon.muzzleEffect, firePoint.position, firePoint.rotation, null), MUZZLE_LIFETIME);
    }
}