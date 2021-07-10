using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMotionHorizontal : MonoBehaviour
{
    public Vector3 offset;
    private bool isNextPositionSet;
    private Vector3 initialPosition;
    private Vector3 nextPosition;
    private bool isAtStart = true;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        nextPosition = initialPosition + offset;
        isNextPositionSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isNextPositionSet)
        {
            transform.position = Vector3.Lerp(transform.position, nextPosition, 0.5f * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextPosition) < 1f)
            {
                isNextPositionSet = false;
                isAtStart = !isAtStart;
            }
        }
        else
        {
            if (isAtStart)
            {
                nextPosition = initialPosition + offset;
            }
            else
            {
                nextPosition = initialPosition;
            }
            isNextPositionSet = true;
        }
    }
}
