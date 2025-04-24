using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    QuestionScreen qs;
    Interaction inter;
    GarageManager gm;

    GameObject problem;
    Camera cam;
    GameObject player;
    GameObject playerUI;

    bool goodAnswer = false;
    bool goodTool = false;

    int problemIndex;

    // Setup
    private void Awake()
    {
        cam = Camera.main;

        player = GameObject.Find("PlayerHolder");
        playerUI = GameObject.Find("PlayerUI");

        qs = GetComponentInParent<QuestionScreen>();
        problem = qs.transform.parent.gameObject;
        inter = player.GetComponent<Interaction>();

        // Find index for objective completion
        GameObject gmHolder = GameObject.Find("GarageManagerHolder");
        gm = gmHolder.GetComponent<GarageManager>();

        problemIndex = gm.setProblems.IndexOf(problem);
    }

    // Button press
    public void CheckAnswer()
    {
        GameObject buttonTextOBJ = gameObject.transform.GetChild(0).gameObject;

        Text buttonText = buttonTextOBJ.GetComponent<Text>();

        // Question
        if (buttonText.text == qs.rightAnswer)
        {
            qs.signImage.sprite = qs.orangeSign;
            goodAnswer = true;
        }
        else
        {
            goodAnswer = false;
        }

        // Tool
        if (inter.activeTool.name == qs.requiredTool.name)
        {
            qs.signImage.sprite = qs.orangeSign;
            goodTool = true;
        }
        else
        {
            goodTool = false;
        }

        // Check final answer
        if (goodTool == true & goodAnswer == true)
        {
            qs.signImage.sprite = qs.correct;
            problem.tag = "Untagged";
            gm.setObjectives[problemIndex].color = Color.green;
        }
        else if (goodAnswer == false | goodTool == false)
        {
            qs.liveImage.sprite = qs.emptyLive;
            qs.lives -= 1;
        }

        if (qs.lives <= 0)
        {
            qs.signImage.sprite = qs.wrong;
            problem.tag = "Untagged";
            gm.setObjectives[problemIndex].color = Color.red;
        }

        cam.GetComponent<Looking>().enabled = true;
        player.GetComponent<Movement>().enabled = true;

        qs.gameObject.SetActive(false);
        playerUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
