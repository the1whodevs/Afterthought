using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupLength = 2.0f;
    [SerializeField] private LayerMask pickupLayers;

    private PlayerController pc;

    private Transform playerCam;

    private GameObject lastHitObject;
    private PickableObj lastHitPickable;

    public void Init()
    {
        pc = Player.Active.Controller;

        playerCam = Player.Active.Camera.transform;
    }

    private void Update()
    {
        if (pc.IsInUI) return;

        Debug.DrawRay(playerCam.position, playerCam.forward * pickupLength, Color.blue);

        var itemHit = Physics.Raycast(playerCam.position, playerCam.forward, out var hit, pickupLength, pickupLayers);

        if (!itemHit)
        {
            lastHitObject = null;
            lastHitPickable = null;
        }
        else
        {
            lastHitPickable = hit.transform.GetComponent<PickableObj>();

            if (lastHitPickable) lastHitObject = hit.rigidbody.gameObject;
        }
    }

}
