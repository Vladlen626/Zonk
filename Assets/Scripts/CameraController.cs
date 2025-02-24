using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Vector2 rotationClamp;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, rotationClamp.x * 2, rotationClamp.y * 2);
        yRotation = Mathf.Clamp(yRotation, rotationClamp.x, rotationClamp.y);
        
        playerBody.localRotation =  Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
