using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    float distance = 1.5f;

    GameObject questionUI;
    public GameObject playerUI;
    public Camera cam;

    bool interacted;

    public List<GameObject> tools = new List<GameObject>();

    public GameObject activeTool;

    void Update()
    {
        if (Input.GetButtonDown("Interact") && interacted == false)
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, distance))
            {
                if (hit.collider.gameObject.tag == "Problem")
                {
                    questionUI = hit.collider.gameObject.transform.GetChild(0).gameObject;
                    cam.GetComponent<Looking>().enabled = false;
                    gameObject.GetComponent<Movement>().enabled = false;
                    questionUI.SetActive(true);
                    playerUI.SetActive(false);
                    Cursor.lockState = CursorLockMode.None;
                    interacted = true;
                }

                switch (hit.collider.gameObject.tag)
                {
                    case "1": ToolChange(); tools[0].SetActive(true); activeTool = tools[0]; break;
                    case "2": ToolChange(); tools[1].SetActive(true); activeTool = tools[1]; break;
                    case "3": ToolChange(); tools[2].SetActive(true); activeTool = tools[2]; break;
                    case "4": ToolChange(); tools[3].SetActive(true); activeTool = tools[3]; break;
                    case "5": ToolChange(); tools[4].SetActive(true); activeTool = tools[4]; break;
                    case "6": ToolChange(); tools[5].SetActive(true); activeTool = tools[5]; break;
                    case "7": ToolChange(); tools[6].SetActive(true); activeTool = tools[6]; break;
                    default: break;
                }
            }
        }
        else if (Input.GetButtonDown("Interact"))
        {
            cam.GetComponent<Looking>().enabled = true;
            gameObject.GetComponent<Movement>().enabled = true;
            questionUI.SetActive(false);
            playerUI.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            interacted = false;
        }
    }

    void ToolChange()
    {
        foreach (GameObject gameObject in tools)
        {
            gameObject.SetActive(false);
        }

        activeTool = null;

    }
}
