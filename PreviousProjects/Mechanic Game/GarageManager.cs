using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GarageManager : MonoBehaviour
{
    // Question screen
    public string carName;

    // Change scenes variables
    public GameObject mainMenuOBJ;
    public GameObject garageOBJ;

    // Objectives variables
    public GameObject objectivesOBJ;
    public GameObject objectivePrefab;
    TMP_Text buttonText;

    // Problems variables
    Car carScript;

    // Objectives completion list
    public List<TMP_Text> setObjectives;
    public List<GameObject> setProblems;

    // Garage start
    public void SpawnCar(string carName_, GameObject carModel, List<string> allProblems, int totalProblems)
    {
        carName = carName_;

        // Spawn car
        GameObject carOBJ = Instantiate(carModel, new Vector3(3f, 0.35f, 3.5f), carModel.transform.rotation);

        // Set script for problems
        carScript = carOBJ.GetComponent<Car>();

        // Set problems
        List<GameObject> tempProblems = new List<GameObject>();
        foreach (GameObject go in carScript.totalProblemsOBJs)
        {
            tempProblems.Add(go);
        }

        // Set objectives
        List<string> tempObjectives = new List<string>();
        foreach (string s in allProblems)
        {
            tempObjectives.Add(s);
        }

        // Setup objectives & problems
        for (int i = 0; i < totalProblems; i++)
        {
            // Set random problem to activate
            int randomProblem = Random.Range(0, tempProblems.Count - 1);

            // Set objectives
            GameObject objective = Instantiate(objectivePrefab, objectivesOBJ.transform);
            objective.transform.SetParent(objectivesOBJ.transform);

            buttonText = objective.GetComponent<TMP_Text>();
            buttonText.text = tempObjectives[randomProblem];

            // Set problems OBJs
            tempProblems[randomProblem].SetActive(true);

            // Set objectives & problems for objective completion
            setObjectives.Add(buttonText);
            setProblems.Add(tempProblems[randomProblem]);

            // Avoid duplicating objectives & problems
            tempObjectives.RemoveAt(randomProblem);
            tempProblems.RemoveAt(randomProblem);
        }

        // Change set
        mainMenuOBJ.SetActive(false);
        garageOBJ.SetActive(true);
    }
}


