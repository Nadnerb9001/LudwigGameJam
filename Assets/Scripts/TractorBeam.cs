using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    private float timer = 0.0f;

    private PlayerController controller;
    private EndScreen endScreen;

    void Start()
    {
        controller = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
        endScreen = Resources.FindObjectsOfTypeAll<EndScreen>()[0].GetComponent<EndScreen>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        MeshCollider beamCollider = this.gameObject.GetComponent<MeshCollider>();
        beamCollider.enabled = false;
        controller.isBeaming = true;
        endScreen.OnEnd(timer);
    }
}
