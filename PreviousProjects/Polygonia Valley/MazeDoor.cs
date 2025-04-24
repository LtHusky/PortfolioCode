using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDoor : MonoBehaviour
{
    // -- PLACE THIS SCRIPT ON "DoorHolder"! --

    bool isOpen;
    public float rotationSpeed;
    public Transform targetToLookTowards;
    public Transform targetToReturnTo;
    float dot;

    public void ActivateDoor()
    {
        if (isOpen)
        {
            StartCoroutine(CloseDoor());
        }
        else
        {
            StartCoroutine(OpenDoor());
        }
    }

    public IEnumerator OpenDoor()
    {
        Vector3 direction = (targetToLookTowards.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        while (dot <= 0.9995f)
        {
            dot = Vector3.Dot(direction, transform.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        dot = 0;
        isOpen = true;
        StopCoroutine(OpenDoor());
    }
    public IEnumerator CloseDoor()
    {
        Vector3 direction = (targetToReturnTo.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        while (dot <= 0.9995f)
        {
            dot = Vector3.Dot(direction, transform.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        dot = 0;
        isOpen = false;
        StopCoroutine(CloseDoor());
    }
}
