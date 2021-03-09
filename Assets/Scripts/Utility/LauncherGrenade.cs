using System;
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
            if (emeraldAIsys && emeraldAIsys.enabled && !emeraldAIsys.CheckIsFriendlyToPlayer())
            {
                emeraldAIsys.Damage(damage, Player.Active.transform, EmeraldAISystem.TargetType.Player, (int)ragdollForce);
                UIManager.Active.RefreshHitmarker();
            }
            else if (playerHealth)
            {
                playerHealth.DamagePlayer(damage);
            }
            // Otherwise just spawn a bullet hole.
            else
            {
                Player.Active.Damage.SpawnHitEffects(hit, launcherData);
            }         
        }
        
        Destroy(gameObject);
    }
}
