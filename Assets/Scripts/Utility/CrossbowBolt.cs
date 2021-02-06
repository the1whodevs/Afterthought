using EmeraldAI;
using Knife.RealBlood.Decals;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrossbowBolt : MonoBehaviour
{
    [SerializeField] private float hitOffset = 0.1f;
    
    private WeaponData CurrentWeapon;
    private Vector3 startPos;
    private Rigidbody arrowBody;
    
    
    public void Init(WeaponData weaponData, Vector3 startPosition)
    {
        arrowBody = GetComponent<Rigidbody>();
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

        var dmg = CurrentWeapon.weaponDamage;
        var talent = Player.Active.Loadout.HasIncreasedWeaponTypeTalent(CurrentWeapon.weaponType);

        if (talent) dmg *= talent.value;

        talent = Player.Active.Loadout.HasIncreasedDamageWhileCrouching();
        if (talent && Player.Active.Controller.IsCrouching) dmg *= talent.value;

        var damage =(int)Mathf.Lerp(dmg, 0.0f, distance / maxRange);
        
        var emeraldAIsys = other.transform.GetComponent<EmeraldAISystem>();
        
        // If we hit an AI, damage it.
        if (emeraldAIsys && emeraldAIsys.enabled)
        {
            emeraldAIsys.Damage(damage, EmeraldAISystem.TargetType.Player, transform, 5000);
            Stick(other);
        }
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
                        null), PlayerDamage.BULLET_HOLE_LIFETIME);
            
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
                            null), PlayerDamage.BULLET_HOLE_LIFETIME);
            }

            Stick();

            if (hitSurfaceInfo) hitSurfaceInfo.PlayImpactSound();
        }
    }

    private void Stick()
    {
        var trail = GetComponentInChildren<TrailRenderer>().gameObject;
        Destroy(trail);
        arrowBody.isKinematic = true;
        transform.position += transform.forward * hitOffset;
        GetComponent<Collider>().enabled = false;
        Destroy(this);
    }

    private void Stick(Collision other)
    {
        var trail = GetComponentInChildren<TrailRenderer>().gameObject;
        Destroy(trail);
        var colliders = other.transform.GetComponentsInChildren<Collider>();
        var maxDist = 1000.0f;
        var colliderHit = other.collider;
        
        foreach (var c in colliders)
        {
            var dist = Vector3.Distance(c.transform.position, other.GetContact(0).point);
            
            
            if (dist<maxDist)
            {
                maxDist = dist;
                colliderHit = c;
            }
        }
        
        transform.SetParent(colliderHit.transform,true);
        transform.position += transform.forward * hitOffset;
        arrowBody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        Destroy(this);
    }
}
