using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMoveState { Idle, CrouchIdle, Run, CrouchRun, Sprint, Jumping }
    
    public PlayerMoveState CurrentMoveState = PlayerMoveState.Idle;

    public bool IsInUI { get; private set; }
    public bool IsADS { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsCrouching => CurrentMoveState == PlayerMoveState.CrouchIdle || 
        CurrentMoveState == PlayerMoveState.CrouchRun;

    public bool IsOnTerrain => (groundedCheckHit.collider != null && groundedCheckHit.collider.tag == "Terrain");
    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, out groundedCheckHit, cc.bounds.extents.y + 0.5f);

    public float MoveSpeed => cc.velocity.magnitude;

    [SerializeField, TagSelector] private string loadoutChangerTag;

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
     
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        pad = Player.Active.Audio;
        pa = Player.Active.Animator;
        pl = Player.Active.Loadout;

        currentTerrain = Terrain.activeTerrain;
    }
    
    private void Update()
    {
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
        if (cc.isGrounded) MoveControls();
        else ApplyGravity();
    }
    
    private void WeaponControls(float dTime)
    {
        attackTimer += dTime;

        if (pl.UsingEquipment) return;

        var fire = Input.GetAxisRaw("Fire1") > 0.0f;
        var aim = Input.GetAxisRaw("Fire2") > 0.0f;
        var reload = Input.GetAxisRaw("Reload") > 0.0f;
        var tryEquipPrimary = Input.GetAxisRaw("EquipPrimary") < 0.0f;
        var tryEquipSecondary = Input.GetAxisRaw("EquipSecondary") > 0.0f;
        var tryThrowEquipment1 = Input.GetAxisRaw("Equipment 1") > 0.0f;
        var tryThrowEquipment2 = Input.GetAxisRaw("Equipment 2") > 0.0f;

        IsADS = aim;

        if (!tryThrowEquipment2) tryThrowEquipment2 = Input.GetAxisRaw("Equipment 1") < 0.0f;

        if (tryThrowEquipment1) 
        {
            pl.UseEquipmentA();
            return;
        } 
        else if (tryThrowEquipment2) 
        {
            pl.UseEquipmentB();
            return;
        } 

        if (tryEquipPrimary) pl.EquipPrimaryWeapon();
        else if (tryEquipSecondary) pl.EquipSecondaryWeapon();
         
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
                    var fireType = pl.CurrentWeapon.fireType;

                    if (fireType == WeaponData.FireType.Burst) pl.CurrentWeapon.ammoInMagazine -= PlayerDamage.BURST_FIRE_COUNT;
                    else pl.CurrentWeapon.ammoInMagazine--;
                     
                    attackTimer = 0.0f;

                    fireResetRequired = fireType != WeaponData.FireType.FullAuto;
                     
                    pa.Fire();
                }
                else
                {
                    //pa.ResetAttack();
                    pad.PlayEmptyClip(pl.CurrentWeapon);
                }
            }
        }
        else if (!fire)
        {
            //pa.ResetAttack();
            fireResetRequired = false;
        }

        if (!pl.IsReloading && reload && pl.CurrentWeapon.ammoType.currentAmmo > 0 && pl.CurrentWeapon.ammoInMagazine < pl.CurrentWeapon.magazineCapacity)
            pl.Reload();
         
        var ads = !pl.IsReloading && aim;
         
        pa.AimDownSights(ads);
        Player.Active.PostProcessing.ADS(ads);
    }
    
    private void MoveControls()
    { 
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        var isSprinting = Input.GetAxisRaw("Sprint") >= 1.0f;
        var isCrouching = Input.GetAxisRaw("Crouch") >= 1.0f;
        var isJumping = Input.GetAxisRaw("Jump") >= 1.0f;
        
        var isMoving = !Mathf.Approximately(v, 0.0f) ||
                        !Mathf.Approximately(h, 0.0f);

        IsMoving = isMoving;

        if (isJumping && cc.isGrounded)
        {
            CurrentMoveState = PlayerMoveState.Jumping;
        }
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
        
        StandStateChange(isCrouching);
        
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
     
    private void StandStateChange(bool isCrouching)
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

        var myCamera = Camera.main.transform;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(loadoutChangerTag))
        {
            UIManager.Active.ShowInteractPrompt(KeyCode.F);

            var interact = Input.GetAxisRaw("Interact");
            //var backUi = Input.GetAxisRaw("BackUI");

            //Debug.LogFormat("I: {0} __ BACK: {1}", interact.ToString("F1"), backUi.ToString("F1"));

            if (!IsInUI && interact > 0.0f)
            {
                IsInUI = true;
                Time.timeScale = 0.0f;
                LoadoutEditor.Instance.ShowWindow();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(loadoutChangerTag)) UIManager.Active.HideInteractPrompt();
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

    public void ExitUI()
    {
        IsInUI = false;
    }

    public float[] GetTerrainTexture()
    {
        var pos = ConvertPosition();
        return CheckTexture(pos.x, pos.y);
    }
}
