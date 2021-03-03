﻿using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerPostProcessing : MonoBehaviour
{
    [SerializeField] private GameObject damageVolumeObject;
    [SerializeField] private GameObject adsVolumeObject;
    [SerializeField] private GameObject deathVolumeObject;
    
    [SerializeField] private float effectDuration;
    [SerializeField] private float waitTime;
    [SerializeField] private float adsTransitionTime;

    private float damageTimer = 0.0f;
    private float adsT = 0.0f;
    //private float targetDeath_weight;
    
    private int targetADS_weight;
    private int lastADS_targetWeight;

    private PostProcessVolume damage_ppv;
    private PostProcessVolume ads_ppv;
    private PostProcessVolume death_ppv;
    
    //private ColorGrading deathColorGrading;
    
    private const float TOLERANCE = 0.0000001f;

    public void Init()
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
        if (status && !Player.Active.Loadout.CurrentWeapon.hasScope) status = false;

        targetADS_weight = status ? 1 : 0;

        if (!(Mathf.Abs(lastADS_targetWeight - targetADS_weight) > TOLERANCE)) return;
        
        adsT = 0.0f;
        lastADS_targetWeight = targetADS_weight;
    }

    private void ADSTransition()
    {
        if (adsT > 1.0f) return;
        
        adsT += Time.deltaTime / adsTransitionTime;

        if (adsT >= 1.0f)
        {
            ads_ppv.weight = targetADS_weight;
            return;
        }
        
        ads_ppv.weight = Mathf.Lerp(ads_ppv.weight, targetADS_weight, adsT);
    }

    public void Death()
    {
        death_ppv.weight = 1.0f;
        damageVolumeObject.SetActive(false);
        adsVolumeObject.SetActive(false);
    }
}
