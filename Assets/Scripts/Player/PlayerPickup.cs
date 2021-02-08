﻿using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Trigger Interact")]
    [SerializeField, TagSelector] private string loadoutChangerTag;

    [Header("Pickup Settings")]
    [SerializeField] private float pickupLength = 2.0f;
    [SerializeField] private LayerMask pickupLayers;

    private PlayerController pc;

    private Transform playerCam;

    public void Init()
    {
        pc = Player.Active.Controller;

        playerCam = Player.Active.Camera.transform;
    }

    private void Update()
    {
        if (pc.IsInUI) return;

        Debug.DrawRay(playerCam.position, playerCam.forward * pickupLength, Color.blue);

        var itemHit = Physics.Raycast(playerCam.position, playerCam.forward, out var hit, pickupLength, pickupLayers, QueryTriggerInteraction.Collide);

        if (!itemHit)
        {
            UIManager.Active.HideInteractPrompt();
        }
        else
        {
            var interact = Input.GetAxisRaw("Interact") > 0.0f;

            // Prioritize loadout selector over other interactables.
            if (hit.transform.CompareTag(loadoutChangerTag))
            {
                UIManager.Active.ShowInteractPrompt(KeyCode.F, "open loadout editor");

                if (interact)
                {
                    pc.GetInUI();
                    Time.timeScale = 0.0f;
                    LoadoutEditor.Instance.ShowWindow();
                }

                return;
            }

            var pickableHit = hit.transform.GetComponent<InteractableObject>();

            if (!pickableHit) return;

            UIManager.Active.ShowInteractPrompt(KeyCode.F, $"pickup {pickableHit.name}");

            if (interact) pickableHit.Interact();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(loadoutChangerTag))
        {
            if (!pc.IsInUI)
            {
                
            }
        }
    }
}
