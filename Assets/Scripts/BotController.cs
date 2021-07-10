using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotController : MonoBehaviour
{
    public Transform[] wayPoints;
    private NavMeshAgent agent;

    public Vector3 offset;
    public int startIndex = 0;
    public bool hasAgent = true;
    private bool shouldLerpToInvisible = false;
    private int currentIndex = 0;
    private bool isNextWayPointSet = false;
    private Vector3 nextWayPoint;
    private MeshRenderer meshRenderer;
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        agent = GetComponent<NavMeshAgent>();
        if (hasAgent)
        {
            agent.transform.position = wayPoints[startIndex].position + offset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAgent)
        {
            if (!isNextWayPointSet) FindNextWayPoint();
            else
            {
                agent.SetDestination(nextWayPoint);
                agent.transform.LookAt(nextWayPoint);
            }
            if (Vector3.Distance(transform.position, nextWayPoint) < 1f)
            {
                isNextWayPointSet = false;
            }
        }
       

        if (shouldLerpToInvisible)
        {
            float lerp = Mathf.Lerp(meshRenderer.material.GetFloat("_Visibility"), 1, 2f * Time.deltaTime);
            meshRenderer.material.SetFloat("_Visibility", lerp);

            if (lerp > 0.8)
            {
                Destroy(gameObject);
            }
        }
    }

    void FindNextWayPoint()
    {
        currentIndex += 1;
        currentIndex %= wayPoints.Length;
        nextWayPoint = new Vector3(wayPoints[currentIndex].transform.position.x + offset.x, transform.position.y, 0);
        isNextWayPointSet = true;
    }

    public void LerpAndDestroy()
    {
        shouldLerpToInvisible = true;
    }
}
