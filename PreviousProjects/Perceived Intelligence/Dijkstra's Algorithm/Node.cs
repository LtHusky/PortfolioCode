using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node prevNode;
    public int distance = 99999;

    public List<NodeTuple> neighbours = new List<NodeTuple>();

    // Visuals (Gizmos)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        foreach (NodeTuple t in neighbours)
            Gizmos.DrawLine(transform.position, t.node.transform.position);
    }

    // Reset
    public void ResetNode()
    {
        prevNode = null;
        distance = 99999;
    }
}
