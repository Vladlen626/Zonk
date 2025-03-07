using System;
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
        playerCamera.enabled = isLocalPlayer;
        playerCamera.GetComponent<AudioListener>().enabled = isLocalPlayer;
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        xRotation = -40f;
        yRotation = 0f;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, rotationClamp.x, rotationClamp.y);
        yRotation = Mathf.Clamp(yRotation, rotationClamp.x + 15f, rotationClamp.y + 15f);
        
        transform.localRotation =  Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
