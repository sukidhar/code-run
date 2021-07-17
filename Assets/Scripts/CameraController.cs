using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject lead;
    public float smoothSpeed =10f;
    public Vector3 offset;

    private LeadController controller;

    private void Start()
    {
        controller = lead.GetComponent<LeadController>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (lead.transform != null)
        {
            Vector3 desiredPosition = lead.transform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            transform.LookAt(lead.transform);
        }
    }
}
