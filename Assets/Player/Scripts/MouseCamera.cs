using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    [SerializeField] private Transform playerT;
    
    [SerializeField] private Vector2 rotationXminMax = new Vector2(-90.0f, 67.5f);

    [SerializeField] private float verticalSensitivity = 1.0f; 
    [SerializeField] private float horizontalSensitivity = 1.0f; 
    
    private Transform cameraT;

    private Vector3 lastMousePos;

    private void Start()
    {
        cameraT = transform;

        lastMousePos = Input.mousePosition;

        SetCursorVisible(false);
    }

    private void SetCursorVisible(bool visible)
    {
        cameraT.localRotation = Quaternion.identity;

        Cursor.visible = visible;
    }

    private void Update()
    {
        var delta = Time.deltaTime;

        var currentMousePos = Input.mousePosition;

        // This rotates the entire player transform around Vector3.up.
        var diffX = currentMousePos.x - lastMousePos.x;
        
        // This rotates the camera transform around Vector3.right.
        var diffY = currentMousePos.y - lastMousePos.y;
        
        diffX *= horizontalSensitivity * delta;
        diffY *= verticalSensitivity * delta;

        //cameraT.Rotate(Vector3.right, -diffY);
        
        var rot = cameraT.localRotation;
        rot.x = Mathf.Clamp(rot.x + -diffY, rotationXminMax.x, rotationXminMax.y);
        cameraT.localRotation = rot;

        playerT.Rotate(Vector3.up, diffX);
        
        lastMousePos = currentMousePos;
    }
}
