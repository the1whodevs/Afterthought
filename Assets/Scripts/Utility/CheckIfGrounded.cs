using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

public class CheckIfGrounded : MonoBehaviour
{
    [SerializeField] private Collider playerCollider;
    
    private bool isGrounded;
    private bool isOnTerrain;
    private RaycastHit hit;
    public bool IsGrounded => isGrounded; 
    public bool IsOnTerrain => isOnTerrain;
    
    private void Update()
    {
        isGrounded = PlayerGrounded();
        isOnTerrain = CheckOnTerrain();
    }

    bool PlayerGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out hit, playerCollider.bounds.extents.y + 0.5f);
    }

    bool CheckOnTerrain()
    {
        if (hit.collider != null && hit.collider.tag == "Terrain") return true;
        else return false;
    }
}
