using EmeraldAI;
using Knife.RealBlood.Decals;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrossbowBolt : MonoBehaviour
{
    private WeaponData CurrentWeapon;
    private Vector3 startPos;
    
    
    public void Init(WeaponData weaponData, Vector3 startPosition)
    {
        CurrentWeapon = weaponData;
        this.startPos = startPosition;
    }
    private void OnCollisionEnter(Collision other)
    {
        var distance = Vector3.Distance(other.transform.position, startPos);
        distance -= CurrentWeapon.minRange;
        var maxRange = CurrentWeapon.maxRange - CurrentWeapon.minRange;
        
        if (distance < 0.0f)
        {
            distance = 0.0f;
        }

        var damage =(int)Mathf.Lerp(CurrentWeapon.weaponDamage, 0.0f, distance / maxRange);
        var emeraldAIsys = other.transform.GetComponent<EmeraldAISystem>();

        // If we hit an AI, damage it.
        if (emeraldAIsys && emeraldAIsys.enabled) emeraldAIsys.Damage(damage, EmeraldAISystem.TargetType.Player, transform, 1000);
        // Otherwise just spawn a bullet hole.
        else
        {
            var cdp =other.transform.GetComponent<CharacterDamagePainter>();
            var hitSurfaceInfo = other.transform.GetComponent<HitSurfaceInfo>();
            var hit = other.contacts[0];
            var hitRb = other.rigidbody;
            
            if (hitRb) hitRb.AddForce(-hit.normal * 100.0f, ForceMode.Impulse);
            
            if (!hitSurfaceInfo) hitSurfaceInfo = other.transform.GetComponentInParent<HitSurfaceInfo>();
            if (!cdp) cdp = other.transform.GetComponentInParent<CharacterDamagePainter>();
            
            Destroy(
                hitSurfaceInfo
                    ? Instantiate(hitSurfaceInfo.hitEffect, hit.point, Quaternion.LookRotation(hit.normal),
                        other.transform)
                    : Instantiate(CurrentWeapon.hitImpact, hit.point, Quaternion.LookRotation(hit.normal),
                        null), PlayerEquipment.BULLET_HOLE_LIFETIME);
            
            if (cdp)
            {
                cdp.Paint(hit.point,hit.normal);
            }
            else
            {
                Destroy(
                    hitSurfaceInfo
                        ? Instantiate(hitSurfaceInfo.RandomHitDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f), Quaternion.LookRotation(hit.normal),
                            other.transform)
                        : Instantiate(CurrentWeapon.RandomHitDecal, hit.point + hit.normal * Random.Range(0.001f, 0.002f), Quaternion.LookRotation(hit.normal),
                            null), PlayerEquipment.BULLET_HOLE_LIFETIME);
            }
            
            if (hitSurfaceInfo) hitSurfaceInfo.PlayImpactSound();
        }
    }
}
