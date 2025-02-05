using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public string horizontalInput; // Horizontal axis for movement
    public string verticalInput; // Vertical axis for movement
    public string jumpButton = "Jump"; // Jump action (if needed)

    private void Update()
    {
        // Get inputs from the gamepad for movement
        float moveX = Input.GetAxis(horizontalInput);
        float moveZ = Input.GetAxis(verticalInput);

        // Move the player based on input
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}
