using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWander : Entity
{
    public Vector3 wander;

    // Wander behaviour
    private void Start()
    {
        SetRandomWander();
    }

    void Update()
    {
        targetVelocity = Vector3.Normalize(wander - transform.position) * 0.02f;
        steering = targetVelocity - velocity;
        steering = steering / mass;

        velocity = velocity + steering;

        transform.position = transform.position + velocity;

        if (transform.position.x + 1 <= wander.x && transform.position.z + 1 <= wander.z || transform.position.x - 1 <= wander.x && transform.position.z - 1 <= wander.z)
        {
            SetRandomWander();
        }
    }

    void SetRandomWander()
    {
        wander.x = Random.Range(-10, 10);
        wander.z = Random.Range(-10, 10);
    }
}
