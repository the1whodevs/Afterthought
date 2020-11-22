using UnityEngine;

public class WeaponIKPointers : MonoBehaviour
{
    public Transform LeftHandTargetPos => leftHandTargetPos;
    public Transform RightHandTargetPos => rightHandTargetPos;
    
    [SerializeField] private Transform leftHandTargetPos;
    [SerializeField] private Transform rightHandTargetPos;
}
