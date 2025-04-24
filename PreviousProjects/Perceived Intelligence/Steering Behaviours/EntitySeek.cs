using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySeek : Entity
{
    // Seek behaviour
    void Update()
    {
        targetVelocity = Vector3.Normalize(target.transform.position - transform.position) * 0.02f;
        steering = targetVelocity - velocity;
        steering = steering / mass;

        velocity = velocity + steering;

        transform.position = transform.position + velocity;
    }
}
