using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbing : MonoBehaviour
{
    [SerializeField] private int mouseButton;

    private PlayerController controller;
    private HingeJoint grabJoint;
    private float grabCd = .1f;
    private float timeSinceGrab = 0;

    private void Start()
    {
        controller = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
    }

    private void Update()
    {
        timeSinceGrab += Time.deltaTime;
        if (!Input.GetMouseButton(mouseButton) && controller.isGrabbing[mouseButton])
        {
            controller.isGrabbing[mouseButton] = false;
            Destroy(grabJoint);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Input.GetMouseButton(mouseButton) && (collision.gameObject.tag == "Terrain" || collision.gameObject.tag == "Obstacle") && !controller.isGrabbing[mouseButton] && !controller.isRagdoll && timeSinceGrab > grabCd)
        {
            controller.isGrabbing[mouseButton] = true;
            controller.SetGrabbing();

            grabJoint = gameObject.AddComponent<HingeJoint>();
            grabJoint.connectedBody = collision.rigidbody;
        }
    }
}
