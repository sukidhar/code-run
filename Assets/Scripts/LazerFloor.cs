using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class LazerFloor : MonoBehaviour
{
    public SetQuestionEvent OnTriggerEntered;
    public bool isUnlocked = false;
    public GameObject lazers;
    public GameObject lead;
    public GameObject hackButtonGroup;
    public GameObject codePanel;
    
    
    // Start is called before the first frame update
    void Start()
    {
        hackButtonGroup.SetActive(false);
        lazers.SetActive(!isUnlocked);
    }

    // Update is called once per frame
    void Update()
    {
        lazers.SetActive(!isUnlocked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == lead && !isUnlocked)
        {
            OnTriggerEntered?.Invoke(gameObject);
            hackButtonGroup.SetActive(true);
        }
        else
        {
            hackButtonGroup.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isUnlocked)
        {
            hackButtonGroup.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == lead)
        {
            hackButtonGroup.SetActive(false);
            codePanel.SetActive(false);
            lead.GetComponent<LeadController>().isHacking = false;
        }
    }
}
