using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPadScript : MonoBehaviour
{
    public Vector3 launchForce;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.attachedRigidbody;
        if (otherRb)
        {
            otherRb.AddForce(launchForce, ForceMode.VelocityChange);
        }
    }
}
