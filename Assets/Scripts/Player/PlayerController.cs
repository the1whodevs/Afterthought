﻿using EmeraldAI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMoveState { Idle, CrouchIdle, Run, CrouchRun, Sprint, Jumping }

    public System.Action OnReloadCancel;

    public PlayerMoveState CurrentMoveState = PlayerMoveState.Idle;

    public bool IsInUI { get; private set; }
    public bool IsADS { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsCrouching => CurrentMoveState == PlayerMoveState.CrouchIdle || 
        CurrentMoveState == PlayerMoveState.CrouchRun;
    public bool TryingToFire { get; private set; }

    public bool IsOnTerrain => (groundedCheckHit.collider != null && groundedCheckHit.collider.tag == "Terrain");
    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, out groundedCheckHit, cc.bounds.extents.y + 0.5f);

    public float MoveSpeed => cc.velocity.magnitude;

    [Header("Move Settings")]
    [SerializeField] private float runSpeed = 10.0f;
    [SerializeField] private float crouchSpeed = 7.0f;
    [SerializeField] private float sprintSpeed = 20.0f;
    [SerializeField] private float jumpHeight = 15.0f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float stickToGround = 2.0f;
    [SerializeField] private float stateChangeSpeed = 1.0f;
    [SerializeField] private PlayerStandState standSetting;
    [SerializeField] private PlayerStandState crouchSetting;
    [SerializeField] private AnimationCurve stateChangeCurve;

    private float[] textureValues = new float[4];

    private Terrain currentTerrain;

    private float attackInterval => 1.0f / pl.CurrentWeapon.fireRate;

    private RaycastHit groundedCheckHit;

    private Vector3 playerVelocity = Vector3.zero;
    
    private float attackTimer = 0.0f;
    private float standStateBlend;
     
    // If this is true, the player needs to let go of the fire button, and press it again to fire.
    // e.g. Bolt-action, semi-auto, burst
    private bool fireResetRequired = false;
    private bool CanAttack => attackTimer >= attackInterval;
    private bool hasInitialized = false;

    // Used to determine if the player has been cooking a grenade!
    private bool lastEquipmentAUseState = false;
    private bool lastEquipmentBUseState = false;

    private Transform myCamera;

    private CharacterController cc;
    
    private PlayerAudio pad;
    private PlayerAnimator pa;
    private PlayerLoadout pl;

    [System.Serializable]
    public class PlayerStandState
    {
        public float controlCameraHeight;
        public float colliderHeight;
        public float colliderCenterHeight;
    }
     
    public void Init(PlayerAudio pa, PlayerAnimator pan, PlayerLoadout pl, Transform playerCamera)
    {
        cc = GetComponent<CharacterController>();
        pad = pa;
        this.pa = pan;
        this.pl = pl;

        myCamera = playerCamera;

        currentTerrain = Terrain.activeTerrain;

        hasInitialized = true;
    }
    
    private void Update()
    {
        if (!hasInitialized) return;

        var d = Time.deltaTime;

        if (IsInUI) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsInUI = true;
            Time.timeScale = 0.0f;
            PauseMenu.Instance.ShowPauseMenu();
        }

        UpdatePlayer();
        WeaponControls(d);
    }

    private void UpdatePlayer()
    {
        MoveControls(out var isCrouching);

        // Moved here so we can jump & be crouched!
        StanceStateChange(isCrouching);

        if (!cc.isGrounded) ApplyGravity();
    }
    
    private void WeaponControls(float dTime)
    {
        attackTimer += dTime;

        var fire = Input.GetAxisRaw("Fire1") > 0.0f;
        var aim = Input.GetAxisRaw("Fire2") > 0.0f;
        var reload = Input.GetAxisRaw("Reload") > 0.0f;
        var tryEquipPrimary = Input.GetAxisRaw("EquipPrimary") < 0.0f;
        var tryEquipSecondary = Input.GetAxisRaw("EquipSecondary") > 0.0f;
        var tryThrowEquipment1 = Input.GetAxisRaw("Equipment 1") > 0.0f;
        var tryThrowEquipment2 = Input.GetAxisRaw("Equipment 2") > 0.0f;
        if (!tryThrowEquipment2) tryThrowEquipment2 = Input.GetAxisRaw("Equipment 1") < 0.0f;

        TryingToFire = fire;
        IsADS = aim;

        if (pl.UsingEquipment)
        {
            // This is true while switching to the equipment to be used!
            if (!pl.CurrentEquipment) return;

            if (pl.CurrentEquipment.Equals(pl.Loadout.Equipment[0])) EquipmentFireLogic(tryThrowEquipment1, ref lastEquipmentAUseState);
            else EquipmentFireLogic(tryThrowEquipment2, ref lastEquipmentBUseState);
            return;
        }

        if (tryThrowEquipment1 && pl.Loadout.Equipment[0].currentAmmo > 0 && !pl.UsingEquipment) 
        {
            pl.UseEquipmentA();
            return;
        } 
        else if (tryThrowEquipment2 && pl.Loadout.Equipment[1].currentAmmo > 0 && !pl.UsingEquipment) 
        {
            pl.UseEquipmentB();
            return;
        } 

        if (tryEquipPrimary) pl.EquipPrimaryWeapon();
        else if (tryEquipSecondary) pl.EquipSecondaryWeapon();

        WeaponFireLogic(fire, aim);

        if (!pl.IsReloading && reload && pl.CurrentWeapon.weaponTypeData.ammoType.currentAmmo > 0 && pl.CurrentWeapon.ammoInMagazine < pl.CurrentWeapon.magazineCapacity) pl.Reload();
         
        var ads = !pl.IsReloading && aim;
         
        pa.AimDownSights(ads);
        Player.Active.PostProcessing.ADS(ads);
    }

    private void EquipmentFireLogic(bool equipmentUse, ref bool lastEquipState)
    {
        if (equipmentUse)
        {
            pa.Cook();
        }
        // The player was cooking the grenade, and should now throw it!
        else if (!equipmentUse)
        {
            pa.Fire();
        }

        lastEquipState = equipmentUse;
    }

    private void WeaponFireLogic(bool fire, bool aim)
    {
        if (pl.CurrentWeapon.weaponAnimType.Equals(PlayerWeaponAnimator.WeaponAnimatorType.Melee) && !fire && aim)
        {
            fire = aim;
        }

        if (fire && !fireResetRequired && CanAttack && CurrentMoveState != PlayerMoveState.Sprint)
        {
            if (pl.CurrentWeapon.weaponType == WeaponData.WeaponType.Melee)
            {
                attackTimer = 0.0f;

                if (aim) pa.AltFire();
                else pa.Fire();
            }
            else
            {
                if (pl.CurrentWeapon.ammoInMagazine > 0)
                {
                    attackTimer = 0.0f;

                    fireResetRequired = pl.CurrentWeapon.fireType != WeaponData.FireType.FullAuto;

                    // cs.ShakeCamera(0.05f, 0.01f);
                    pa.Fire();
                }
                else
                {
                    pad.PlayEmptyClip(pl.CurrentWeapon);
                }
            }
        }
        else if (!fire)
        {
            fireResetRequired = false;
        }
    }
    
    private void MoveControls(out bool isCrouching)
    { 
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        var isSprinting = Input.GetAxisRaw("Sprint") >= 1.0f;
        isCrouching = Input.GetAxisRaw("Crouch") >= 1.0f;
        var isJumping = Input.GetAxisRaw("Jump") >= 1.0f;
        
        var isMoving = !Mathf.Approximately(v, 0.0f) ||
                        !Mathf.Approximately(h, 0.0f);

        if (!cc.isGrounded) return;

        if (isSprinting && pl.IsReloading) OnReloadCancel?.Invoke();

        IsMoving = isMoving;

        if (isJumping) CurrentMoveState = PlayerMoveState.Jumping;
        else if (isMoving)
        {
            if (isSprinting && v > 0.0f)
            {
                CurrentMoveState = PlayerMoveState.Sprint;
                isCrouching = false;
            }
            else
            {
                CurrentMoveState = isCrouching ? PlayerMoveState.CrouchRun : PlayerMoveState.Run;
            }
        }
        else
        {
            CurrentMoveState = PlayerMoveState.Idle;
        }
        
        var forward = transform.forward;

        forward.y = 0;
        forward.Normalize();

        var moveVector = forward * v + transform.right * h;

        moveVector.Normalize();

        var speed = runSpeed;
        if (isSprinting && !isCrouching && v > 0.0f)
        {
            speed = sprintSpeed;
        }
        else if (isCrouching)
        {
            var crouchTalent = pl.HasFasterCrouchSpeed();
            speed = crouchSpeed * (crouchTalent ? crouchTalent.value : 1.0f);
        }
        
        var r = new Ray(transform.position, Vector3.down);

        Physics.SphereCast(r, cc.radius, out var hitInfo, cc.height / 2f, groundLayer);

        var talent = pl.HasNoMobilityPenalty();
        if (pl.CurrentWeapon && !talent)
        {
            speed = Mathf.Clamp(speed - pl.CurrentWeapon.mobilityPenalty, 0.0f, speed);
        }

        var desiredVelocity = Vector3.ProjectOnPlane(moveVector, hitInfo.normal) * speed;
        playerVelocity.x = desiredVelocity.x;
        playerVelocity.z = desiredVelocity.z;
        playerVelocity.y = -stickToGround;

        if (!cc.isGrounded) return;
        
        if (isJumping && !isCrouching)
        {
            playerVelocity.y = jumpHeight;
        }
        
        cc.Move(playerVelocity * Time.deltaTime);
    }
     
    private void StanceStateChange(bool isCrouching)
    {
        standStateBlend = Mathf.MoveTowards(standStateBlend, isCrouching ? 1f : 0f, Time.deltaTime * stateChangeSpeed);

        cc.height = Mathf.Lerp(
            standSetting.colliderHeight,
            crouchSetting.colliderHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );


        var colliderCenter = cc.center;

        colliderCenter.y = Mathf.Lerp(
            standSetting.colliderCenterHeight,
            crouchSetting.colliderCenterHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );

        cc.center = colliderCenter;

        var cameraPos = myCamera.localPosition;
        
        cameraPos.y = Mathf.Lerp(
            standSetting.controlCameraHeight,
            crouchSetting.controlCameraHeight,
            stateChangeCurve.Evaluate(standStateBlend)
        );

        myCamera.localPosition = cameraPos;
    }

    private void ApplyGravity()
    {
        playerVelocity += Vector3.down * (-Physics.gravity.y * Time.deltaTime);
        cc.Move(playerVelocity * Time.deltaTime);
    }

    private Vector2Int ConvertPosition()
    {
        Vector3 terrainPosition = transform.position - currentTerrain.transform.position;

        Vector3 mapPosition = new Vector3
        (terrainPosition.x / currentTerrain.terrainData.size.x, 0,
            terrainPosition.z / currentTerrain.terrainData.size.z);

        var xCoord = mapPosition.x * currentTerrain.terrainData.alphamapWidth;
        var zCoord = mapPosition.z * currentTerrain.terrainData.alphamapHeight;

        return new Vector2Int((int)xCoord, (int)zCoord);
    }

    private float[] CheckTexture(int posX, int posZ)
    {
        var aMap = currentTerrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);

        textureValues[0] = aMap[0, 0, 0];
        textureValues[1] = aMap[0, 0, 1];
        textureValues[2] = aMap[0, 0, 2];
        textureValues[3] = aMap[0, 0, 3];

        return textureValues;
    }

    public void GetInUI()
    {
        IsInUI = true;
    }

    public void ExitUI()
    {
        IsInUI = false;

        Player.Active.Camera.LoadSensitivityValues();
    }

    public float[] GetTerrainTexture()
    {
        var pos = ConvertPosition();
        return CheckTexture(pos.x, pos.y);
    }

}
