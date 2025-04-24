using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    // Navigation
    public Node startNode;
    public Node endNode;

    Node currentNode;

    public List<Node> path;

    public List<Node> unvisitedNodes = new List<Node>();
    public List<Node> visitedNodes = new List<Node>();

    // Walk
    bool canWalk;
    int pathIndex;

    public float mass;
    public Vector3 velocity;

    Vector3 steering;

    GameObject target;
    Vector3 targetVelocity;

    void Start()
    {
        CalculatePath();
    }

    void Update()
    {
        if (canWalk)
            WalkPath();
    }

    void CalculatePath()
    {
        // Setup
        startNode.distance = 0;
        unvisitedNodes.Add(startNode);

        // Loop
        while (unvisitedNodes.Count > 0)
        {
            currentNode = unvisitedNodes[0];

            // Check for end node.
            if (currentNode == endNode)
            {
                visitedNodes.Add(currentNode);
                unvisitedNodes.Remove(currentNode);
                continue;
            }

            // Calculate 'neighbour' nodes.
            List<NodeTuple> neighbours = currentNode.neighbours;

            foreach (NodeTuple t in neighbours)
            {
                if (visitedNodes.Contains(t.node))
                {
                    continue;
                }

                int dist = currentNode.distance + t.weight;

                if (t.node.distance > dist)
                {
                    t.node.distance = dist;
                    t.node.prevNode = currentNode;
                }

                if (!visitedNodes.Contains(t.node))
                {
                    if (!unvisitedNodes.Contains(t.node))
                    {
                        unvisitedNodes.Add(t.node);
                    }
                }
            }

            // Reset for next node.
            visitedNodes.Add(currentNode);
            unvisitedNodes.Remove(currentNode);

            unvisitedNodes.OrderBy(n => n.distance);
        }

        // Finish path.
        SetPath();
        visitedNodes.Clear();
        unvisitedNodes.Clear();
    }
    
    // Set path to walk.
    void SetPath()
    {
        Node pathNode = endNode;

        // Loop to fill path with nodes.
        while (pathNode != startNode)
        {
            path.Add(pathNode);
            pathNode = pathNode.prevNode;
        }
        path.Add(startNode);

        pathIndex = path.Count - 1;
        target = path[pathIndex].gameObject;
        canWalk = true;
    }

    void WalkPath()
    {
        // Walk calculated path.
        targetVelocity = Vector3.Normalize(target.transform.position - transform.position) * 0.025f;
        steering = targetVelocity - velocity;

        steering /= mass;
        velocity += steering;

        transform.position = transform.position + velocity;

        float dist = Vector3.Distance(transform.position, target.transform.position);

        // Continue to next Node.
        if (dist <= 0.2f)
        {
            pathIndex--;

            // Check if walking should be stopped.
            if (target == endNode.gameObject)
            {
                canWalk = false;
            }
            else
            {
                target = path[pathIndex].gameObject;
            }
         }
    }
}
