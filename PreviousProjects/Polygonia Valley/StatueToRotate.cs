using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueToRotate : MonoBehaviour
{
    public GameObject target;
    public bool isFacingTarget;

    bool disableStatue;
    float rotateSpeed = 50.0f;

    RotateStatueManager puzzleManager;

    void Start()
    {
        puzzleManager = transform.parent.GetComponent<RotateStatueManager>();
    }

    // Rotate object with mouse.
    void OnMouseDrag()
    {
        if (isFacingTarget == false)
        {
            // Rotate object.
            float rotateX = Input.GetAxis("Mouse X") * rotateSpeed * Mathf.Deg2Rad;
            transform.Rotate(Vector3.up, -rotateX);
        }
    }

    // Check if object is facing target.
    void OnMouseUp()
    {
        if (isFacingTarget == false)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(direction, transform.forward);

            if (dot >= 0.99f)
            {
                isFacingTarget = true;
                puzzleManager.CheckStatues();
            }
            else
            {
                isFacingTarget = false;
            }
        }
    }
}
