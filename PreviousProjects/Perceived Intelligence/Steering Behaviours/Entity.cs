using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Basic behaviour
    public Vector3 velocity;
    public Vector3 targetVelocity;

    public Vector3 steering;

    public GameObject target;
    public float mass;
}
