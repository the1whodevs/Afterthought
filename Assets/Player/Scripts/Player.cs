using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public PlayerAnimator Animator { get; private set; }
    public PlayerEquipment Equipment { get; private set; }
    
    private void Awake()
    {
        if (instance) Destroy(this);

        instance = this;
    }

    private void Start()
    {
        Animator = GetComponent<PlayerAnimator>();
        Animator.Init();
        
        Equipment = GetComponent<PlayerEquipment>();
        Equipment.Init();
    }
}
