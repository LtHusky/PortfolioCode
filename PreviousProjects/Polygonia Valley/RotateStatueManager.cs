using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotateStatueManager : MonoBehaviour
{
    // == Variables ==
    // Objects
    public List<StatueToRotate> statues = new List<StatueToRotate>();
    List<GameObject> correctStatues = new List<GameObject>();

    // Completion
    public UnityEvent eventToTrigger;
    bool isSolved;

    public void CheckStatues()
    {
        // Check if all statues are rotated correctly.
        foreach (StatueToRotate statue in statues)
        {
            if (statue.isFacingTarget == true && !correctStatues.Contains(statue.gameObject))
            {
                correctStatues.Add(statue.gameObject);
            }
        }

        // Check if puzzle can be solved.
        if (correctStatues.Count >= statues.Count)
        {
            SolvePuzzle();
        }
    }

    void SolvePuzzle()
    {
        // Solve puzzle.
        if (isSolved == false)
        {
            isSolved = true;
            eventToTrigger.Invoke();
        }
    }
}
