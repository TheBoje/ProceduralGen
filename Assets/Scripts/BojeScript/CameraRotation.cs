using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [Range(0.2f, 10f)]
    public float mouseSensitivity = 1f;
    public float maxYAngle = 80f;
    private Vector2 currentRotation;

    public Transform transform;

    private void Rotate()
    {
        if (Input.GetMouseButton(0) == false){
            currentRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity;
            currentRotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
            Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        }
    }

    private void FixedUpdate()
    {
        Rotate();
    }


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

}