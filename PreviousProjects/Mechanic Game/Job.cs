using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class Job : MonoBehaviour
{
    // Transfer data variables
    GarageManager gm;

    // Job variables
    public GameObject model;
    public string jobName;
    public TMP_Text jobText;
    public Sprite jobSprite;
    public difficulties difficulty;
    public TMP_Text difficultyText;
    public List<string> problems;
    public int problemAmount;

    // Setup
    void Start()
    {
        // Set GarageManager
        GameObject gmHolder = GameObject.Find("GarageManagerHolder");
        gm = gmHolder.GetComponent<GarageManager>();

        // Set job name
        GameObject jobNameOBJ = gameObject.transform.GetChild(0).gameObject;
        jobText = jobNameOBJ.GetComponent<TMP_Text>();
        jobText.text = jobName;

        // Set job image & sprite
        Image jobImage = gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
        jobImage.sprite = jobSprite;

        // Set job difficulty
        GameObject difficultyOBJ = gameObject.transform.GetChild(3).gameObject;
        difficultyText = difficultyOBJ.GetComponent<TMP_Text>();
        difficultyText.text = difficulty.ToString();

        // Set start button
        GameObject startButtonOBJ = gameObject.transform.GetChild(4).gameObject;
        Button startButton = startButtonOBJ.GetComponent<Button>();
        startButton.onClick.AddListener(StartJob);
    }

    // Start button
    void StartJob()
    {
        // Transfer selected job to garage scene
        gm.SpawnCar(jobName, model, problems, problemAmount);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
