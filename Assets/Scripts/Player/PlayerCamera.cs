using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Active;

    public enum ZoomLevels
    {
        X1,
        X2,
        X4,
        X8
    }

    public ZoomLevels currentZoom = ZoomLevels.X1;

    [SerializeField] private Transform playerT;
    [SerializeField] private Transform playerWeaponCameraT;

    [SerializeField] private float mouseSensitivity_X = 1.0f;
    [SerializeField] private float mouseSensitivity_Y = 1.0f;
    [SerializeField] private float cameraMinY = -60f;
    [SerializeField] private float cameraMaxY = 75f;
    [SerializeField] private float zoomSpeed = 2.5f;

    [SerializeField] private Camera zoomCamera;

    [Header("Weapon Sway & Recoil")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private bool useSway = true;
    [SerializeField] private float swayAmount;
    [SerializeField] private float smoothing;
    [SerializeField] private float swayCapX = 0.15f;
    [SerializeField] private float swayCapY = 0.15f;

    private Vector3 weaponHolderStartPos;
    private Quaternion weaponHolderStartRot;

    public Vector3 OffsetFromPlayer => offsetFromPlayer;
    private Vector3 offsetFromPlayer;

    private Dictionary<string, float> zoomLevels = new Dictionary<string, float>();
    
    private GameObject zoomCameraGameObject;
    
    private Transform cameraT;

    public float BodyRotationX => bodyRotationX;
    private float bodyRotationX = 1f;

    public float CamRotationY => camRotationY;
    private float camRotationY;

    private bool initialized;

    /// <summary>
    /// Input.Axis(Mouse X) * xSensitivity
    /// </summary>
    public float MouseInputX => xInput;


    /// <summary>
    /// Input.Axis(Mouse Y) * ySensitivity
    /// </summary>
    public float MouseInputY => yInput;
    private float xInput;
    private float yInput;
    private float delta;
    
    private static PlayerController _playerController;

    public void Init(bool cleanInit)
    {
        if (Active) Destroy(this);

        Active = this;

        if (!zoomLevels.ContainsKey("X1")) zoomLevels.Add("X1", 60.0f);
        if (!zoomLevels.ContainsKey("X2")) zoomLevels.Add("X2", 30.0f);
        if (!zoomLevels.ContainsKey("X4")) zoomLevels.Add("X4", 15.0f);
        if (!zoomLevels.ContainsKey("X8")) zoomLevels.Add("X8", 7.5f);
        
        zoomCameraGameObject = zoomCamera.gameObject;
        zoomCameraGameObject.SetActive(false);
        
        cameraT = transform;
        weaponHolderStartPos = weaponHolder.localPosition;
        weaponHolderStartRot = weaponHolder.localRotation;
        LoadSensitivityValues();

        if (cleanInit)
        {
            RecalculateOffset();
            RotatePlayer(playerT.rotation.eulerAngles.y, cameraT.localRotation.x, Time.deltaTime);
        }

        initialized = true;
    }

    public void LoadData(Vector3 offsetFromPlayer, float bodyRotationX, float camRotationY)
    {
        this.offsetFromPlayer = offsetFromPlayer;
        this.bodyRotationX = bodyRotationX;
        this.camRotationY = camRotationY;

        FixCameraOffset();
    }

    public void RecalculateOffset()
    {
        if (!cameraT) cameraT = transform;
        
        offsetFromPlayer = cameraT.parent.position - cameraT.position;
    }

    public void FixCameraOffset()
    {
        if (!cameraT || !cameraT.parent) return;

        cameraT.position = cameraT.parent.position - offsetFromPlayer;
    }

    private void Update()
    {
        if (LoadingManager.Active.Loading) return;
        if (!initialized) return; 

        if (Player.Active.Controller.IsInUI) return;

        delta = Time.deltaTime;

        AdjustCameraToCurrentZoom(delta);

        xInput = Input.GetAxis("Mouse X") * mouseSensitivity_X;
        yInput = Input.GetAxis("Mouse Y") * mouseSensitivity_Y;

        if (useSway) WeaponSway(delta);

        LookRotation(delta);
        
        // If moving, head bob.
        HeadBob(delta);
    }

    private void WeaponSway(float delta)
    {
        var x = -MouseInputX * swayAmount;
        var y = -MouseInputY * swayAmount;

        if (x < 0 && x < -swayCapX)
            x = -swayCapX;
        else if (x > 0 && x > swayCapX)
            x = swayCapX;

        if (y < 0 && y < -swayCapY)
            y = -swayCapY;
        else if (y > 0 && y > swayCapY)
            y = swayCapY;

        var finalPos = new Vector3(x, y, 0);
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, finalPos + weaponHolderStartPos, delta * smoothing);
    }
    
    private void HeadBob(float delta)
    {
        
    }

    private void LookRotation(float delta)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        RotatePlayer(xInput, yInput, delta);
    }

    private void RotatePlayer(float xInput, float yInput, float delta)
    {
        bodyRotationX += xInput;
        camRotationY += yInput;

        //Stop the camera rotation 360 Degrees
        camRotationY = Mathf.Clamp(camRotationY, cameraMinY, cameraMaxY);

//        var camTargetRotation = Quaternion.Euler(-camRotationY, 0, 0);
//        var bodyTargetRotation = Quaternion.Euler(0, bodyRotationX, 0);

        playerT.rotation = Quaternion.Euler(0, bodyRotationX, 0);

        cameraT.localRotation = Quaternion.Euler(-camRotationY, 0, 0);
    }
    
    private string ZoomEnumToString(ZoomLevels zoom)
    {
        switch (zoom)
        {
            case ZoomLevels.X1:
                return "X1";
            
            case ZoomLevels.X2:
                return "X2";
            
            case ZoomLevels.X4:
                return "X4";
            
            case ZoomLevels.X8:
                return "X8";
            
            default:
                throw new ArgumentOutOfRangeException(nameof(zoom), zoom, null);
        }
    }

    public void LoadSensitivityValues()
    {
        mouseSensitivity_X = PlayerPrefs.GetFloat(SettingsManager.HORIZONTAL_SENS_KEY, 1.5f);
        mouseSensitivity_Y = PlayerPrefs.GetFloat(SettingsManager.VERTICAL_SENS_KEY, 1.5f);
    }

    public void ApplyRecoil(float horizontalForce, float verticalForce)
    {
        Debug.Log("ADDING RECOIL...");

        if (!_playerController) _playerController = Player.Active.Controller;

        //var talent = Player.Active.Loadout.HasMinimalHorizontalRecoil();
        if (Player.Active.Loadout.HasMinimalHorizontalRecoil()) horizontalForce = Player.Active.Loadout.HasMinimalHorizontalRecoil().value;

        //talent = Player.Active.Loadout.HasMinimalVerticalRecoil();
        if (Player.Active.Loadout.HasMinimalVerticalRecoil()) verticalForce = Player.Active.Loadout.HasMinimalVerticalRecoil().value;

        if (_playerController.IsADS)
        {
            horizontalForce /= 2.0f;
            verticalForce /= 2.0f;
        }

        if (_playerController.IsMoving)
        {
            horizontalForce *= 1.25f;
            verticalForce *= 1.25f;
        }

        UIManager.Active.AddRecoil(horizontalForce, verticalForce);

        //RotateWeapon(UnityEngine.Random.Range(-horizontalForce, horizontalForce), UnityEngine.Random.Range(0.0f, verticalForce), Player.Active.Loadout.CurrentWeapon.recoil_speed, Time.deltaTime);
        RotateWeapon(horizontalForce, verticalForce, Player.Active.Loadout.CurrentWeapon.recoil_speed, Time.deltaTime);
    }

    public void RotateWeapon(float xRot, float yRot, float speed, float delta)
    {
        var targetRot = weaponHolderStartRot * Quaternion.Euler(weaponHolder.right * yRot + weaponHolder.up * xRot);
        //weaponHolder.Rotate(weaponHolder.right * (yRot * speed * delta) + weaponHolder.up * (xRot * speed * delta), Space.Self);
        weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, targetRot, speed * delta);
    }

    public void ToggleZoom(bool status)
    {
        if (!Player.Active.Loadout.HasScope) return;
        
        zoomCameraGameObject.SetActive(status);

//        var scope = Player.Active.Loadout.ScopeGameObject;

        currentZoom = status ? Player.Active.Loadout.CurrentWeapon.scopeZoom : ZoomLevels.X1;

        if (Player.Active.Loadout.ScopeGameObject) Player.Active.Loadout.ScopeGameObject.SetActive(status);
    }

    private void AdjustCameraToCurrentZoom(float delta)
    {
//        var targetFov = zoomLevels[ZoomEnumToString(currentZoom)];
//        
//        var currentFov = zoomCamera.fieldOfView;

        const float tolerance = 0.01f;

        if (Mathf.Abs(zoomLevels[ZoomEnumToString(currentZoom)] - zoomCamera.fieldOfView) <= tolerance) return;
        
        zoomCamera.fieldOfView = Mathf.SmoothStep(zoomCamera.fieldOfView, zoomLevels[ZoomEnumToString(currentZoom)], delta * zoomSpeed);
    }
}