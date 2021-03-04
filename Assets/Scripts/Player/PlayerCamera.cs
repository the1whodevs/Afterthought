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

    [SerializeField] private float mouseSensitivity_X = 1.0f;
    [SerializeField] private float mouseSensitivity_Y = 1.0f;
    [SerializeField] private float cameraMinY = -60f;
    [SerializeField] private float cameraMaxY = 75f;
    [SerializeField] private float rotationSmoothSpeed = 10f;
    [SerializeField] private float zoomSpeed = 2.5f;

    [SerializeField] private Camera zoomCamera;

    private Vector3 offsetFromPlayer;

    private Dictionary<string, float> zoomLevels = new Dictionary<string, float>();
    
    private GameObject zoomCameraGameObject;
    
    private Transform cameraT;

    private float bodyRotationX = 1f;
    private float camRotationY;

    private bool initialized;

    public void Init()
    {
        if (Active) Destroy(this);

        Active = this;

        RecalculateOffset();

        if (!zoomLevels.ContainsKey("X1")) zoomLevels.Add("X1", 60.0f);
        if (!zoomLevels.ContainsKey("X2")) zoomLevels.Add("X2", 30.0f);
        if (!zoomLevels.ContainsKey("X4")) zoomLevels.Add("X4", 15.0f);
        if (!zoomLevels.ContainsKey("X8")) zoomLevels.Add("X8", 7.5f);
        
        zoomCameraGameObject = zoomCamera.gameObject;
        zoomCameraGameObject.SetActive(false);
        
        cameraT = transform;

        LoadSensitivityValues();

        RotatePlayer(playerT.rotation.eulerAngles.y, cameraT.localRotation.x, Time.deltaTime);

        initialized = true;
    }

    public void RecalculateOffset()
    {
        offsetFromPlayer = transform.parent.position - transform.position;
    }

    private void Update()
    {
        if (!initialized) return; 

        if (Player.Active.Controller.IsInUI) return;

        var d = Time.deltaTime;

        AdjustCameraToCurrentZoom(d);
        LookRotation(d);
    }

    private void LookRotation(float delta)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        var xMove = Input.GetAxis("Mouse X") * mouseSensitivity_X;
        var yMove = Input.GetAxis("Mouse Y") * mouseSensitivity_Y;

        RotatePlayer(xMove, yMove, delta);
    }

    private void RotatePlayer(float xInput, float yInput, float delta)
    {
        bodyRotationX += xInput;
        camRotationY += yInput;

        //Stop the camera rotation 360 Degrees
        camRotationY = Mathf.Clamp(camRotationY, cameraMinY, cameraMaxY);

        var camTargetRotation = Quaternion.Euler(-camRotationY, 0, 0);
        var bodyTargetRotation = Quaternion.Euler(0, bodyRotationX, 0);

        playerT.rotation = bodyTargetRotation;

        cameraT.localRotation = camTargetRotation;
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

    public void FixCameraOffset()
    {
        transform.position = transform.parent.position + offsetFromPlayer;
    }


    public void LoadSensitivityValues()
    {
        mouseSensitivity_X = PlayerPrefs.GetFloat(MainMenu.HORIZONTAL_SENS_KEY, 1.5f);
        mouseSensitivity_Y = PlayerPrefs.GetFloat(MainMenu.VERTICAL_SENS_KEY, 1.5f);
    }

    public void ApplyRecoil(float horizontalForce, float verticalForce)
    {
        var p = Player.Active.Controller;

        var talent = Player.Active.Loadout.HasMinimalHorizontalRecoil();
        if (talent) horizontalForce = talent.value;

        talent = Player.Active.Loadout.HasMinimalVerticalRecoil();
        if (talent) verticalForce = talent.value;

        if (p.IsADS)
        {
            horizontalForce /= 2.0f;
            verticalForce /= 2.0f;
        }

        if (p.IsMoving)
        {
            horizontalForce *= 1.25f;
            verticalForce *= 1.25f;
        }

        UIManager.Active.AddRecoil(horizontalForce, verticalForce);

        var randX = UnityEngine.Random.Range(-horizontalForce, horizontalForce);
        var randY = UnityEngine.Random.Range(0.0f, verticalForce);

        RotatePlayer(randX, randY, Time.deltaTime);
    }

    public void ToggleZoom(bool status)
    {
        if (!Player.Active.Loadout.HasScope) return;
        
        zoomCameraGameObject.SetActive(status);

        var scope = Player.Active.Loadout.ScopeGameObject;

        if (status) currentZoom = Player.Active.Loadout.CurrentWeapon.scopeZoom;
        else currentZoom = ZoomLevels.X1;

        if (scope) scope.SetActive(status);
    }

    private void AdjustCameraToCurrentZoom(float delta)
    {
        var targetFov = zoomLevels[ZoomEnumToString(currentZoom)];
        
        var currentFov = zoomCamera.fieldOfView;

        const float tolerance = 0.01f;

        if (Mathf.Abs(targetFov - currentFov) <= tolerance) return;
        
        zoomCamera.fieldOfView = Mathf.SmoothStep(currentFov, targetFov, delta * zoomSpeed);
    }
}