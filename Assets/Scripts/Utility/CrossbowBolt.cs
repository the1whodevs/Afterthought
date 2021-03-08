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
        if (emeraldAIsys && emeraldAIsys.enabled && !emeraldAIsys.CheckIsFriendlyToPlayer())
        {
            emeraldAIsys.Damage(damage, EmeraldAISystem.TargetType.Player, transform, 5000);
            Stick(other);
            UIManager.Active.RefreshHitmarker();
        }
        // Otherwise just spawn a bullet hole.
        else
        {
            Player.Active.Damage.SpawnHitEffects(other.contacts[0], CurrentWeapon);

            Stick();
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
