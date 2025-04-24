using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    // States
    enum states { NAVIGATE, ATTACK, SLEEP }
    states state;

    // Graph
    public Node startNode;
    Node currentNode;

    public List<Node> graph;
    List<Node> unvisitedNodes = new List<Node>();
    List<Node> visitedNodes = new List<Node>();
    public List<Node> path;

    public GameObject target;

    int pathIndex;
    bool canWalk;

    // AI Stats
    public int health;

    Vector3 velocity;
    Vector3 targetVelocity;
    Vector3 steering;
    public float mass;

    // Navigate
    public Node randomNode;

    // Attack
    float targetDist;
    GameObject player;

    // Sleep
    float sleepDist;
    public Node sleepPoint;

    // Set start variables.
    void Start()
    {
        health = 100;
        mass = 100;

        state = states.NAVIGATE;
        SetRandomNavigationPos();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Check states & distance.
    void Update()
    {
        switch (state)
        {
            default: state = states.NAVIGATE; break;
            case states.NAVIGATE: StartCoroutine(Navigate()); break;
            case states.ATTACK: Attack(); break;
            case states.SLEEP: StartCoroutine(Sleep()); break;
        }

        targetDist = Vector3.Distance(gameObject.transform.position, player.transform.position);
    }

    // Change state.
    void ChangeState(states stateToChangeTo)
    {
        state = stateToChangeTo;
    }

    // Set random navigation position.
    void SetRandomNavigationPos()
    {
        graph.Remove(startNode);
        randomNode = graph[UnityEngine.Random.Range(0, graph.Count)];
        graph.Add(startNode);
        CalculatePath(randomNode);
    }

    // Calculate path to random navigation position.
    void CalculatePath(Node toNode)
    {
        // Setup
        startNode.distance = 0;
        unvisitedNodes.Add(startNode);

        while (unvisitedNodes.Count > 0)
        {
            currentNode = unvisitedNodes[0];

            // Check for end node.
            if (currentNode == toNode)
            {
                visitedNodes.Add(currentNode);
                unvisitedNodes.Remove(currentNode);
                continue;
            }

            // Calculate 'neighbour' nodes.
            List<NodeTuple> neighbours = currentNode.neighbours;

            foreach (NodeTuple t in neighbours)
            {
                if (unvisitedNodes.Contains(t.node))
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

            visitedNodes.Add(currentNode);
            unvisitedNodes.Remove(currentNode);

            unvisitedNodes.OrderBy(n => n.distance);
        }

        SetPath(toNode);

        // Clear nodes to set new path.
        foreach (Node n in graph)
        {
            n.ResetNode();
        }
        unvisitedNodes.Clear();
        visitedNodes.Clear();
    }
    
    // Set path to walk to random navigation position.
    void SetPath(Node toNode)
    {
        Node pathNode = toNode;

        // Loop to fill path with nodes.
        while (pathNode != startNode)
        {
            path.Add(pathNode);
            if (pathNode.prevNode == null)
            {
                Debug.Log("Previous Node of " + pathNode.gameObject + " is empty!");
                break;
            }
            else
            {
                pathNode = pathNode.prevNode;
            }
        }
        path.Add(startNode);

        pathIndex = path.Count - 1;
        target = path[pathIndex].gameObject;
        canWalk = true;
    }

    // Navigate AI.
    IEnumerator Navigate()
    {
        // Walk calculated path.
        if (canWalk == true)
        {
            Move(target);

            float dist = Vector3.Distance(transform.position, target.transform.position);

            // Continue to next Node.
            if (dist <= 0.2f)
            {
                pathIndex--;

                // Check for target node.
                if (target == randomNode.gameObject)
                {
                    // Reset values & continue.
                    canWalk = false;
                    path.Clear();
                    startNode = randomNode;

                    yield return new WaitForSeconds(2f);
                    SetRandomNavigationPos();
                }
                else
                {
                    target = path[pathIndex].gameObject;
                }
            }
        }

        // // Change state from 'NAVIGATE'.
        if (targetDist <= 5.1f)
        {
            ChangeState(states.ATTACK);
        }
        if (health <= 50)
        {
            ChangeState(states.SLEEP);
        }
    }

    // Attack player.
    void Attack()
    {
        // Chase player.
        if (targetDist <= 5.1f)
        {
            Move(player);
        }
        else
        {
            velocity = new Vector3(0, 0, 0);
        }

        // Change state from 'ATTACK'.
        if (targetDist > 5.1f)
        {
            ChangeState(states.NAVIGATE);
        }
        if (health <= 50)
        {
            ChangeState(states.SLEEP);
        }
    }

    // Sleep and regenerate health points.
    IEnumerator Sleep()
    {
        // To sleep point.
        Move(sleepPoint.gameObject);

        // Regenerate health points.
        sleepDist = Vector3.Distance(gameObject.transform.position, sleepPoint.gameObject.transform.position);
        if (sleepDist <= 1f)
        {
            velocity = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(2f);
            health = 100;
        }

        // Change state from 'SLEEP'.
        if (health >= 50 && targetDist >= 5.1f)
        {
            ChangeState(states.NAVIGATE);
        }
        else if (health >= 50 && targetDist <= 5.1f)
        {
            ChangeState(states.ATTACK);
        }
    }

    // Move AI.
    void Move(GameObject moveTo)
    {
        targetVelocity = Vector3.Normalize(moveTo.transform.position - transform.position) * 0.01f;
        steering = targetVelocity - velocity;
        steering /= mass;

        velocity += steering;

        transform.position = transform.position + velocity;
    }
}
