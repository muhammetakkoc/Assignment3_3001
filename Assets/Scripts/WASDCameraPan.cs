using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDCameraPan : MonoBehaviour
{
    public Vector2 zoomRange = new Vector2 (100, 400 );
    public float zoomSpeed = 50;

    public float panSpeedBase = 5f; // Initial speed of panning
    public float panSpeed = 5f; // Initial speed of panning
    public float acceleration = 2f; // Acceleration rate
    public float drag = 2f; // Drag force
    private Vector3 currentVelocity;

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput == 0 && verticalInput == 0)
        {
            // Decelerate when no input
            currentVelocity -= currentVelocity * drag * Time.deltaTime;
            transform.Translate(currentVelocity * Time.deltaTime, Space.World);
            panSpeed = panSpeedBase;
        }
        else
        {

            Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

            panSpeed += acceleration * Time.deltaTime;

            // Apply acceleration
            currentVelocity = (direction * panSpeed);

            // Apply drag to current velocity
            currentVelocity -= currentVelocity * drag * Time.deltaTime;

            transform.Translate(currentVelocity * Time.deltaTime, Space.World);
        }

        if(Input.mouseScrollDelta.sqrMagnitude > 0)
        {
            float size = Camera.main.orthographicSize;
            Camera.main.orthographicSize = Mathf.Clamp(size + -Input.mouseScrollDelta.y * zoomSpeed *  Time.deltaTime, zoomRange.x, zoomRange.y);
        }
    }
}
