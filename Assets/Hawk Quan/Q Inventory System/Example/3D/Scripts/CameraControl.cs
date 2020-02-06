using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private float Y_ANGLE_MIN = 0.0f;
    [SerializeField]
    private float Y_ANGLE_MAX = 10.0f;

    public Transform lookAt;
    public Transform camTransform;
    public Vector3 camOffset;
    public float distance = 10.0f;
    public Vector3 lookAtOffset;
    //public Transform dir;

    private float currentX = 0.0f;
    private float currentY = 45.0f;
    private float sensitivityX = 4.0f;
    private float sensitivityY = 1.0f;

    private void Start()
    {
        camTransform = transform;
    }

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X");
        currentY -= Input.GetAxis("Mouse Y");

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        camTransform.position = lookAt.position + rotation * dir + camOffset;
        camTransform.LookAt(lookAt.position + lookAtOffset);
    }

    //public Vector3 GetDir()
    //{
    //    Vector3 direction = (dir.position - transform.position).normalized;
    //    direction.x = 0;
    //    direction.z = 0;
    //    return direction;
    //}
}
