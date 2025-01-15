using System;
using UnityEngine;

/**
 * From Fusion tutorial https://doc.photonengine.com/fusion/current/tutorials/shared-mode-basics/3-movement-and-camera
 */
public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField]
    private float MouseSensitivity = 10f;

    [SerializeField]
    private float clampRotationAngle = 45f;

    private float verticalRotation;
    private float horizontalRotation;

    private Transform target;

    internal void SetTarget(Transform transform)
    {
        this.target = transform;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        transform.position = target.position;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -clampRotationAngle, clampRotationAngle);

        horizontalRotation += mouseX * MouseSensitivity;
        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
}
