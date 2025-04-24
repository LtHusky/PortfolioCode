using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDynamicWall : MonoBehaviour
{
    bool isDown;
    Vector3 originPos;
    Vector3 shrinkedPos;
    float distance = 4f;

    void Start()
    {
        originPos = transform.position;
        shrinkedPos = new Vector3(originPos.x, originPos.y - 5f, originPos.z);
    }

    public void MoveWall()
    {
        if (isDown)
        {
            StartCoroutine(MoveWallUp());
        }
        else
        {
            StartCoroutine(MoveWallDown());
        }
    }

    IEnumerator MoveWallUp()
    {
        while (distance > 0.02f)
        {
            transform.position = Vector3.Lerp(transform.position, originPos, 1f * Time.deltaTime);
            distance = Vector3.Distance(transform.position, originPos);
            yield return null;
        }
        isDown = false;
        distance = 4f;
        StopCoroutine(MoveWallUp());
    }

    IEnumerator MoveWallDown()
    {
        while ( distance > 0.02f)
        {
            transform.position = Vector3.Lerp(transform.position, shrinkedPos, 1f * Time.deltaTime);
            distance = Vector3.Distance(transform.position, shrinkedPos);
            yield return null;
        }
        isDown = true;
        distance = 4f;
        StopCoroutine(MoveWallDown());
    }
}
