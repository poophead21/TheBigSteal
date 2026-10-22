using System;
using UnityEngine;

public class TriggerCheckpoint : MonoBehaviour
{
    public bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isTriggered  = true;
        }
    }
}
