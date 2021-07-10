using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerBullet : MonoBehaviour
{
    public LayerMask enemyMask;
    public LayerMask shield;
    public float velocity = 20f;
    public float life = 1f;
    public LayerMask[] layers;
    private float lifeTimer;
    private LayerMask mask = 0;


    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < layers.Length; i++)
        {
            mask |= layers[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit, velocity * Time.deltaTime, ~(mask)))
        {
            transform.position = hit.point;
            Vector3 reflected = Vector3.Reflect(transform.forward, hit.normal);
            Vector3 direction = transform.forward;
            Vector3 projectPlane = Vector3.ProjectOnPlane(reflected, Vector3.forward);
            transform.forward = projectPlane;
            transform.rotation = Quaternion.LookRotation(projectPlane, Vector3.forward);
            Hit(transform.position, direction, reflected, hit.collider);
        }
        else
        {
            transform.Translate(Vector3.forward * velocity * Time.deltaTime);
        }

        if (lifeTimer + life < Time.time)
        {
            Destroy(gameObject);
        }
    }

    private void Hit(Vector3 position, Vector3 direction, Vector3 reflected, Collider collider)
    {
        
        if (((1 << collider.gameObject.layer) & (enemyMask)) != 0 && !(((1 << collider.gameObject.layer) & (shield)) != 0))
        {
            collider.gameObject.GetComponent<BotController>().LerpAndDestroy();
            Destroy(gameObject);
        }

    }

    public void Fire(Vector3 position, Vector3 euler)
    {
        lifeTimer = Time.time;
        transform.eulerAngles = euler;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        Vector3 projectPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.forward);
        transform.forward = projectPlane;
        transform.rotation = Quaternion.LookRotation(projectPlane, Vector3.forward);
    }
}
