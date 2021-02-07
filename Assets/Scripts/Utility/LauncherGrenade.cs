﻿using System;
using EmeraldAI;
using Knife.RealBlood.Decals;
using UnityEngine;
using Random = UnityEngine.Random;

public class LauncherGrenade : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 10.0f;
    [SerializeField] private float ragdollForce = 5000.0f;
    
    private WeaponData launcherData;

    public void Init(WeaponData weaponData)
    {
        launcherData = weaponData;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        var contact = other.GetContact(0);

        // Spawn explosion.
        Destroy(Instantiate(launcherData.hitImpact, contact.point, Quaternion.LookRotation(-contact.normal), null), PlayerDamage.BULLET_HOLE_LIFETIME);

        var hits = Physics.SphereCastAll(contact.point, explosionRadius, -contact.normal, explosionRadius);

        if (hits.Length <= 0) return;
        
        var maxRange = launcherData.maxRange - launcherData.minRange;

        var startPos = contact.point;
        
        foreach (var hit in hits)
        {
            var distance = Vector3.Distance(hit.transform.position, startPos);
            distance -= launcherData.minRange;

            if (distance < 0.0f) distance = 0.0f;

            var dmg = launcherData.weaponDamage;
            var talent = Player.Active.Loadout.HasIncreasedWeaponTypeTalent(launcherData.weaponType);

            if (talent) dmg *= talent.value;

            talent = Player.Active.Loadout.HasIncreasedDamageWhileCrouching();
            if (talent && Player.Active.Controller.IsCrouching) dmg *= talent.value;

            var damage = (int) Mathf.Lerp(dmg, 0.0f, distance / maxRange);
                
            var emeraldAIsys = hit.transform.GetComponent<EmeraldAISystem>();
            var playerHealth = hit.transform.GetComponent<PlayerHealth>();

            // If we hit an AI, damage it.
            if (emeraldAIsys && emeraldAIsys.enabled)
            {
                emeraldAIsys.Damage(damage, EmeraldAISystem.TargetType.Player, transform, (int)ragdollForce);
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
                    
                if (hitRb) hitRb.AddForce(-hit.normal * ragdollForce, ForceMode.Impulse);

                if (!hitSurfaceInfo) hitSurfaceInfo = hit.transform.GetComponentInParent<HitSurfaceInfo>();
                    
                if (!cdp) cdp = hit.transform.GetComponentInParent<CharacterDamagePainter>();

                if (cdp)
                {
                    cdp.Paint(hit.point,hit.normal);
                }
                else
                {
                    Destroy(
                        hitSurfaceInfo
                            ? Instantiate(hitSurfaceInfo.RandomHitDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f), Quaternion.LookRotation(hit.normal),
                                hit.transform)
                            : Instantiate(launcherData.RandomHitDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f), Quaternion.LookRotation(hit.normal),
                                null), PlayerDamage.BULLET_HOLE_LIFETIME);
                }
                    
                if (hitSurfaceInfo) hitSurfaceInfo.PlayImpactSound(); 
            }         
        }
        
        Destroy(gameObject);
    }
}
