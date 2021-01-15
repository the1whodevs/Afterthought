using System;
using UnityEngine;
using System.Collections.Generic;

public class MouseCamera : MonoBehaviour
{
    public static MouseCamera Instance;

    public enum ZoomLevels
    {
        X1,
        X2,
        X4,
        X8
    }

    public ZoomLevels currentZoom = ZoomLevels.X1;

    [SerializeField] private Transform playerT;

    [SerializeField] private float camRotationSpeed = 5f;
    [SerializeField] private float cameraMinY = -60f;
    [SerializeField] private float cameraMaxY = 75f;
    [SerializeField] private float rotationSmoothSpeed = 10f;
    [SerializeField] private float zoomSpeed = 2.5f;

    [SerializeField] private Camera zoomCamera;

    private Dictionary<string, float> zoomLevels = new Dictionary<string, float>();
    
    private GameObject zoomCameraGameObject;
    
    private Transform cameraT;

    private float bodyRotationX = 1f;
    private float camRotationY;

    private void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    private void Start()
    {
        zoomLevels.Add("X1", 60.0f);
        zoomLevels.Add("X2", 30.0f);
        zoomLevels.Add("X4", 15.0f);
        zoomLevels.Add("X8", 7.5f);
        
        zoomCameraGameObject = zoomCamera.gameObject;
        zoomCameraGameObject.SetActive(false);
        
        cameraT = transform;
    }

    private void Update()
    {
        var d = Time.deltaTime;
        
        AdjustCameraToCurrentZoom(d);
        LookRotation(d);
    }

    private void LookRotation(float delta)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        var xMove = Input.GetAxis("Mouse X") * camRotationSpeed;
        var yMove = Input.GetAxis("Mouse Y") * camRotationSpeed;
        RotatePlayer(xMove, yMove, delta);
    }

    private void RotatePlayer(float xInput, float yInput, float delta)
    {
        bodyRotationX += xInput;
        camRotationY += yInput;

        //Stop the camera rotation 360 Degrees
        camRotationY = Mathf.Clamp(camRotationY, cameraMinY, cameraMaxY);

        var canTargetRotation = Quaternion.Euler(-camRotationY, 0, 0);
        var bodyTargetRotation = Quaternion.Euler(0, bodyRotationX, 0);

        playerT.rotation = Quaternion.Lerp(playerT.rotation, bodyTargetRotation, delta * rotationSmoothSpeed);

        cameraT.localRotation = Quaternion.Lerp(cameraT.localRotation, canTargetRotation, delta * rotationSmoothSpeed);
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

    public void ApplyRecoil(float horizontalForce, float verticalForce)
    {
        var p = Player.Instance.Controller;

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

        var randX = UnityEngine.Random.Range(-horizontalForce, horizontalForce);
        var randY = UnityEngine.Random.Range(0.0f, verticalForce);

        RotatePlayer(randX, randY, Time.deltaTime);
    }

    public void ToggleZoom(bool status)
    {
        if (!Player.Instance.Equipment.HasScope) return;
        
        zoomCameraGameObject.SetActive(status);

        var scope = Player.Instance.Equipment.ScopeGameObject;
        
        if (scope) scope.SetActive(status);
    }

    private void AdjustCameraToCurrentZoom(float delta)
    {
        if (!zoomCameraGameObject.activeInHierarchy) return;
        
        var targetFov = zoomLevels[ZoomEnumToString(currentZoom)];
        
        var currentFov = zoomCamera.fieldOfView;

        const float tolerance = 0.01f;

        if (Mathf.Abs(targetFov - currentFov) <= tolerance) return;
        
        zoomCamera.fieldOfView = Mathf.SmoothStep(currentFov, targetFov, delta * zoomSpeed);
    }
}