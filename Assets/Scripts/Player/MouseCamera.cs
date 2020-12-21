using System;
using UnityEngine;
using System.Collections.Generic;

public class MouseCamera : MonoBehaviour
{
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

        bodyRotationX += Input.GetAxis("Mouse X") * camRotationSpeed;
        camRotationY += Input.GetAxis("Mouse Y") * camRotationSpeed;

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

    public void ToggleZoom(bool status)
    {
        zoomCameraGameObject.SetActive(status);
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