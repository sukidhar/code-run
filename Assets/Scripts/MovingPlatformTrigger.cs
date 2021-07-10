using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformTrigger : MonoBehaviour
{
    public GameObject lead;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == lead)
        {
            lead.transform.parent = gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == lead)
        {
            lead.transform.parent = null;
        }
    }
}
