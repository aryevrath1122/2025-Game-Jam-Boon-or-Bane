using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookCamera : MonoBehaviour
{
    public Transform player;  // The player object (camera is a child of this)
    public float rotationSpeed = 1.0f;
    public float sensitivity = 2.0f;
    public float minY = -40f;
    public float maxY = 80f;

    private Vector2 lookInput;
    private float currentXRotation = 0f;

    private PlayerMovement controls;
    private Camera cam;

    void Awake()
    {
        controls = new PlayerMovement();

        // Get the Camera component
        cam = Camera.main;

       
    }

    

    void Update()
    {
        HandleCameraRotation();
    }

    private void HandleCameraRotation()
    {
        // Rotate the camera based on input from the right thumbstick
        float mouseX = lookInput.x * rotationSpeed * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * rotationSpeed * sensitivity * Time.deltaTime;

        currentXRotation -= mouseY;
        currentXRotation = Mathf.Clamp(currentXRotation, minY, maxY); // Clamp Y axis rotation

        // Rotate the camera locally (around the X axis)
        cam.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);

        // Rotate the player (Y axis rotation) based on the right thumbstick's X-axis movement
        player.Rotate(Vector3.up * mouseX);
    }
}
