using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

public class PlayerPostProcessing : MonoBehaviour
{
    [SerializeField] private GameObject damageVolumeObject;
    [SerializeField] private GameObject adsVolumeObject;
    [SerializeField] private GameObject deathVolumeObject;
    
    [SerializeField] private float effectDuration;
    [SerializeField] private float waitTime;
    [SerializeField] private float adsTransitionTime;

    private float damageTimer = 0.0f;
    private float adsTimer = 0.0f;
    private float targetADSweight;
    private float targetDeathweight;
    
    private PostProcessVolume damage_ppv;
    private PostProcessVolume ads_ppv;
    private PostProcessVolume death_ppv;
    
    private ColorGrading deathColorGrading;
    
    private void Start()
    {
        damageTimer = effectDuration + waitTime;
        
        damage_ppv = damageVolumeObject.GetComponent<PostProcessVolume>();
        ads_ppv = adsVolumeObject.GetComponent<PostProcessVolume>();
        death_ppv = deathVolumeObject.GetComponent<PostProcessVolume>();
        
        damage_ppv.weight = 0.0f;
        ads_ppv.weight = 0.0f;
        death_ppv.weight = 0.0f;
    }

    private void Update()
    {
        DamageTimer();
        ADSTransition();
    }

    private void DamageTimer()
    {
        damageTimer += Time.deltaTime;
        if (damageTimer <= waitTime) return;

        var t = Mathf.Clamp01((damageTimer - waitTime) / effectDuration);
        damage_ppv.weight = Mathf.Lerp(1.0f, 0.0f, t);
    }

    public void Damage()
    {
        damageTimer = 0.0f;
        damage_ppv.weight = 1.0f;
    }

    public void ADS(bool status)
    {
        targetADSweight = status ? 1.0f : 0.0f;
    }

    private void ADSTransition()
    {
        var t = Mathf.Clamp01(Time.deltaTime / adsTransitionTime);
        
        ads_ppv.weight = Mathf.Lerp(ads_ppv.weight, targetADSweight, t);
    }

    public void Death()
    {
        death_ppv.weight = 1.0f;
        damageVolumeObject.SetActive(false);
        adsVolumeObject.SetActive(false);
    }
}
