using UnityEngine;

public class HitSurfaceInfo : MonoBehaviour
{
    public GameObject hitEffect;
    public GameObject[] hitDecal;
        
    public GameObject RandomHitDecal => hitDecal[Random.Range(0, hitDecal.Length)];
}
