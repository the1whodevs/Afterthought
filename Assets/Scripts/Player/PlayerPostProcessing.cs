using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

public class PlayerPostProcessing : MonoBehaviour
{
    [SerializeField] private GameObject damageVolumeObject;
    [SerializeField] private float effectDuration;
    [SerializeField] private float waitTime;

    private float timer = 0.0f;
    private PostProcessVolume damage_ppv;
    private void Start()
    {
        timer = effectDuration + waitTime;
        damage_ppv = damageVolumeObject.GetComponent<PostProcessVolume>();
        damage_ppv.weight = 0.0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer <= waitTime) return;

        var t = Mathf.Clamp01((timer-waitTime) / effectDuration);
        damage_ppv.weight = Mathf.Lerp(1.0f, 0.0f, t);
    }

    public void PlayerDamage()
    {
        timer = 0.0f;
        damage_ppv.weight = 1.0f;
    }

}
