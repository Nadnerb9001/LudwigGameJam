using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tripping : MonoBehaviour
{
    private PlayerController controller;

    private void Start()
    {
        controller = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
            controller.SetTripping(this.gameObject);
    }
}