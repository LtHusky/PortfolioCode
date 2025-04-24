using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityArrival : Entity
{
    public float maxDistance;

    // Arrival behaviour
    void Update()
    {
        float dist = Vector3.Distance(transform.position, target.transform.position);

        if (dist >= maxDistance)
        {
            targetVelocity = Vector3.Normalize(target.transform.position - transform.position) * 0.02f;
            steering = targetVelocity - velocity;
            steering = steering / mass;

            velocity = velocity + steering;

            transform.position = transform.position + velocity;
        }
    }
}
