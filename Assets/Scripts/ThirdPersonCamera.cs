using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform lookAt;
    [SerializeField] private Transform camTransform;

    private Camera cam;

    private float distance = 10f;
    private float currentX = 0f;
    private float currentY = 0f;
    private float sensitivityScroll = -2f;
    private float minAngle = -10f;
    private float maxAngle = 80f;
    private float minDistance = 3f;
    private float maxDistance = 40f;
    private bool isColliding = false;
    private float collisionZoom = .5f;
    private float preCollideDistance;

    private void Start()
    {
        camTransform = transform;
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X");
        currentY -= Input.GetAxis("Mouse Y");
        distance += Input.mouseScrollDelta.y * sensitivityScroll;
        currentY = Mathf.Clamp(currentY, minAngle, maxAngle);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        if (isColliding)
            distance -= collisionZoom;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Terrain") || other.gameObject.CompareTag("Obstacle"))
        {
            preCollideDistance = distance;
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Terrain") || other.gameObject.CompareTag("Obstacle"))
        {
            distance = preCollideDistance;
            isColliding = false;
        }
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        camTransform.position = lookAt.position + rotation * dir;
        cam.transform.LookAt(lookAt.position);
    }
}
