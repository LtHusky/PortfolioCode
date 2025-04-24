using UnityEngine;

public class TouchObject : Puzzle
{
    // VARIABLES
    public bool playerStay;
    public GameObject door;
    public Transform moveToPosition;
    public float time;

    // FUNCTIONS
    void Start()
    {   // Set Start Position.
        startPos = door.transform;      // ???
    }

    void Update()
    {
        Debug.Log(startPos.position);
        if (touched == true)
        {   // Open door.
            door.transform.position = moveToPosition.position;
        }
        else
        {   // Close door.
            door.transform.position = startPos.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {   // Check if Player collides with Trigger. (TOGGLE)
        if (other.gameObject.tag == "Player" && touched == false && playerStay == false)
        {
            touched = true;
        }
        else if (other.gameObject.tag == "Player" && touched == true && playerStay == false)
        {
            touched = false;
        }
    }

    void OnTriggerStay(Collider other)
    {   // Check if Player stays on Trigger. (STAY)
        if (other.gameObject.tag == "Player" && touched == false && playerStay == true)
        {
            touched = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {   // Check if Player leaves Trigger. (STAY) 
        if (other.gameObject.tag == "Player" && touched == true && playerStay == true)
        {
            touched = false;
        }
    }
}