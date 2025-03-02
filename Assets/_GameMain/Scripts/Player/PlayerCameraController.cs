using Mirror;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector2 rotationClamp;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera.enabled = isLocalPlayer;
        playerCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, rotationClamp.x * 2, rotationClamp.y * 2);
        yRotation = Mathf.Clamp(yRotation, rotationClamp.x, rotationClamp.y);
        
        transform.localRotation =  Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
