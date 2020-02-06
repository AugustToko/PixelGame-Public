using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public float runSpeed = 5f;
    public float walkSpeed = 1.5f;

    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 moveVector;
    private float currentX = 0.0f;
    private float currentY = 45.0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        moveVector = Vector3.zero;

        float speed = (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.z = Input.GetAxis("Vertical");

        //currentX += Input.GetAxis("Mouse X");
        //currentY -= Input.GetAxis("Mouse Y");

        moveVector = RotateWithView();

        controller.Move(moveVector * speed * Time.deltaTime);

        if (controller.velocity != Vector3.zero)
            transform.forward = controller.velocity;
    }

    //private void LateUpdate()
    //{
    //   transform.rotation = Quaternion.Euler(currentY, currentX, 0);
    //}

    private Vector3 RotateWithView()
    {
        if (cameraTransform != null)
        {
            Vector3 dir = cameraTransform.TransformDirection(moveVector);
            dir.Set(dir.x, 0, dir.z);
            return dir.normalized * moveVector.magnitude;
        }
        else
        {
            cameraTransform = Camera.main.transform;
            return moveVector;
        }
    }
}
