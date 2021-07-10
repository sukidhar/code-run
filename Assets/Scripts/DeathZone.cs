using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathZone : MonoBehaviour
{
    public GameObject lead;
    public UnityEvent deathEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == lead)
        {
            deathEvent?.Invoke();
            lead.GetComponent<LeadController>().LerpToDeath();
        }
    }
}
