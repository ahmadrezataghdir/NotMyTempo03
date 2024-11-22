using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionHandler : MonoBehaviour
{
    private Rigidbody handRigidbody;

    private void Start()
    {
        handRigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Surface"))
        {
            // Stop hand movement or apply logic
            Debug.LogError("Hand collided with a surface!");
            // Optional: Lock movement
            //handRigidbody.velocity = Vector3.zero;
            //handRigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Surface"))
        {
            Debug.LogError("Hand left the surface!");
        }
    }
}

