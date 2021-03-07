using System;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public static Action<InteractableObject> OnInteract;

    [Header("Trigger Interact")]
    [SerializeField, TagSelector] private string loadoutChangerTag;

    [Header("Pickup Settings")]
    [SerializeField] private float pickupLength = 2.0f;
    [SerializeField] private LayerMask pickupLayers;

    private bool fireResetRequired = false;
    private bool hasInitialized = false;

    private PlayerController pc;
    private PlayerLoadout pl;

    private Transform playerCam;

    public void Init(PlayerController pc, PlayerLoadout pl)
    {
        this.pc = pc;
        this.pl = pl;

        playerCam = Player.Active.Camera.transform;

        hasInitialized = true;
    }

    private void Update()
    {
        if (!hasInitialized) return;

        if (pc.IsInUI) return;

        Debug.DrawRay(playerCam.position, playerCam.forward * pickupLength, Color.blue);

        var itemHit = Physics.Raycast(playerCam.position, playerCam.forward, out var hit, pickupLength, pickupLayers, QueryTriggerInteraction.Collide);

        if (!itemHit || pl.SwitchingWeapons)
        {
            UIManager.Active.HideInteractPrompt();
            return;
        }

        var pickableHit = hit.transform.GetComponent<InteractableObject>();

        if (!pickableHit || pickableHit.IsLooted || (pickableHit.GetComponent<EmeraldAI.EmeraldAISystem>() && pickableHit.GetComponent<EmeraldAI.EmeraldAISystem>().CurrentHealth > 0)) UIManager.Active.HideInteractPrompt();
        else
        {
            var interact = Input.GetAxisRaw("Interact") > 0.0f;

            if (!interact) fireResetRequired = false;

            UIManager.Active.ShowInteractPrompt(KeyCode.F, 
                $"{pickableHit.GetActionVerb()} " +
                $"{pickableHit.GetActionPronoun()} " +
                $"{pickableHit.name}");

            if (interact && !fireResetRequired)
            {
                fireResetRequired = true;
                OnInteract?.Invoke(pickableHit);
                pickableHit.Interact();
            }
        }
    }
}
