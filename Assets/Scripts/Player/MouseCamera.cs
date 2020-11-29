using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    [SerializeField] private Transform playerT;
    
    [SerializeField] private float camRotationSpeed = 5f;
    [SerializeField] private float cameraMinY = -60f;
    [SerializeField] private float cameraMaxY = 75f;
    [SerializeField] private float rotationSmoothSpeed = 10f;
    
    private Transform cameraT;
    
    private float bodyRotationX = 1f;
    private float camRotationY;

    private void Start()
    {
        cameraT = transform;
    }

    private void Update()
    {
        LookRotation(Time.deltaTime);
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
}
