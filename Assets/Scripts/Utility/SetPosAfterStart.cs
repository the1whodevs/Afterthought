using UnityEngine;

public class SetPosAfterStart : MonoBehaviour
{
    [SerializeField] private Transform targetPose;

    private void LateUpdate()
    {
        transform.position = targetPose.position;
        transform.rotation = targetPose.rotation;
    }
}
