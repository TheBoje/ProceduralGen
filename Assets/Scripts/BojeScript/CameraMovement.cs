using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
 
    private void FixedUpdate()
    {

        Vector3 vectorMove = GetInput();

        controller.Move(vectorMove * speed * Time.deltaTime);
    
    }
    private Vector3 GetInput()
    {
        
        float x = Input.GetAxis("Horizontal");                                                  
        float z = Input.GetAxis("Vertical");
        Vector3 vectorMove = transform.right * x + transform.forward * z;
        return vectorMove;

    }
}
