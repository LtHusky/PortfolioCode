using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class FishingManager : Item
{
    // *** PLACE THIS SCRIPT ON THE "FishingRodHand" OBJECT AT "Mondegreen" ***

    // Variables
    // *Item SCRIPT VARIABLES SHOW UP HERE*
    [Header("Minigame OBJs")]
    public Camera playerCamera;
    [Space]
    public GameObject desktopPlayer;
    public GameObject fishingFloatPrefab;
    public GameObject fishingArea;
    public GameObject fishSnappingPoint;
    public Mesh fishingrod2;
    GameObject fishingMinigameOBJ;
    GameObject castedFloat;
    GameObject fishingFloatPS;
    Mesh fishingrod1;

    [Header("Fish")]
    /* ###### OM HET AANTAL VISSEN UIT TE BREIDEN: ######
     * 1. Voeg het nieuwe vis object toe aan de 'fishList' list in de Inspector. (Deze lijst gaat van klein tot grote vissen)
     * 2. Voeg de informatie van deze vis toe aan de 'fishInfo' array op dezelfde locatie/index als de toegevoegde vis in 'fishList'.
     * 3. Voeg de wachttijd van de vis toe aan de 'timerLimits' array op dezelfde locatie/index als de toegevoegde vis in 'fishList'.
     * 4. Zorg ervoor dat alle lists en arrays van kleine tot grote vissen zijn!
     */

    // SYNC THE FOLLOWING 3 LISTS & ARRAYS FROM SMALL TO LARGE FISH!!!

    public List<GameObject> fishList = new List<GameObject>(); // (Fill this list in inspector.)
    
    string[] fishInfo = {
    "Vrouwelijke clownvissen leggen tot duizend eieren op koraal of rotsen.", // Clownfish/nemo
    "De blauwe tang vis verandert van kleur onder blauw en ultraviolet licht.", // Blue tang/dory
    "De gemiddelde snelheid van een karper is 3 km per uur.", // Carp
    "De tonijn wordt ook wel skipjack genoemd." // Tuna
    };

    float[] timerLimits = { 5, 10, 15, 20 };

    float fishTimer;
    bool hasBaitedFish;
    bool hasCaughtFish;
    GameObject caughtFish;
    GameObject instantiatedFish;

    [Header("Math Question")]
    public GameObject MathUI;

    public TMP_Text mathQuestionText;
    public TMP_Text fishInfoText;
    public GameObject infoUI;
    public List<TMP_Text> answers = new List<TMP_Text>();

    float answer;

    bool canStart;

    // Functions
    // Setup
    public override void Start()
    {
        base.Start();
        fishingrod1 = gameObject.GetComponent<MeshFilter>().mesh;
        fishingMinigameOBJ = transform.parent.gameObject;

        // Check if setup is executed correctly (Prevent error crashes).
        if (!fishingMinigameOBJ)
            print("Cannot assign fishingMinigameOBJ. Possible fix: Place this script on correct object! (READ LINE 7)");
        else
            canStart = true;
    }

    // Right mouse click; cast line or reel in line.
    public override void Interact()
    {
        if (canStart && !hasBaitedFish)
        {
            CastOut();
            canStart = false;
        }
        else
        {
            ReelIn();
            canStart = true;
        }
    }

    // Show area to cast line out in.
    public override void OnPickup()
    {
        DefreezeRigidBody();
        fishingArea.SetActive(true);
    }

    // Disable area when fishing rod is dropped.
    public override void OnDrop()
    {
        ResetGame(1);
    }

    // Create float in fishing area.
    void CastOut()
    {
        // Cast float to raycasthit point.
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20))
        {
            GameObject lastHit = hit.transform.gameObject;
            if (lastHit == fishingArea)
            {
                // Hide fishing area.
                fishingArea.SetActive(false);

                // Create float
                castedFloat = Instantiate(fishingFloatPrefab);
                castedFloat.transform.position = hit.point;
                fishingFloatPS = castedFloat.transform.GetChild(0).gameObject;
                fishingFloatPS.SetActive(false);
                StartCoroutine(StartTimer());
            }
        }
    }

    // Quit fishing; Delete float that has been casted out and check if any fish was caught.
    void ReelIn()
    {
        // Delete float and check for fish.
        Destroy(castedFloat);

        if (hasBaitedFish && !hasCaughtFish)
        {
            CatchFish();
        }
        // Setup game to be playable again.
        else
        {
            fishingArea.SetActive(true);
            StopCoroutine(StartTimer());
        }
    }

    // Start timer to catch fish.
    IEnumerator StartTimer()
    {
        fishTimer = 0;
        int randomFish = UnityEngine.Random.Range(0, timerLimits.Length);

        float timerLimit = timerLimits[randomFish];

        while(fishTimer < timerLimit)
        {
            fishTimer++;
            yield return new WaitForSeconds(1);
        }

        if (fishTimer >= timerLimit)
        {
            caughtFish = fishList[randomFish];
            fishInfoText.text = fishInfo[randomFish];
            hasBaitedFish = true; 
            fishingFloatPS.SetActive(true);
        }
    }

    // Catch caught fish and instantiate math question.
    void CatchFish()
    {
        fishingFloatPS.SetActive(false);
        hasCaughtFish = true;
        gameObject.GetComponent<MeshFilter>().mesh = fishingrod2;
        instantiatedFish = Instantiate(caughtFish, fishSnappingPoint.transform);

        // Generate random variables for question.
        string mathQuestion = null;
        float mathNumber1 = UnityEngine.Random.Range(1, 10);
        float mathNumber2 = UnityEngine.Random.Range(1, 10);

        // Generate question with operators: + or * or /.
        int mathPicker = UnityEngine.Random.Range(1, 3);
        switch (mathPicker)
        {
            default: print("Could not define mathPicker."); break;
            case 1: mathQuestion = mathNumber1.ToString() + " + " + mathNumber2.ToString(); answer = mathNumber1 += mathNumber2; break;
            case 2: mathQuestion = mathNumber1.ToString() + " x " + mathNumber2.ToString(); answer = mathNumber1 * mathNumber2; break;
            case 3: mathQuestion = mathNumber1.ToString() + " : " + mathNumber2.ToString(); answer = mathNumber1 / mathNumber2; break; 
        }

        // Setup UI elements.
        mathQuestionText.text = mathQuestion;

        mathPicker = UnityEngine.Random.Range(1, 3);
        TMP_Text correctAnswer = answers[mathPicker];
        int correctAnswerIndex = mathPicker;

        correctAnswer.text = answer.ToString();

        answers.Remove(answers[mathPicker]);

        foreach(TMP_Text t in answers)
        {
            mathPicker = UnityEngine.Random.Range(1, 20);
            if (mathPicker == answer)
            {
                mathPicker = UnityEngine.Random.Range(1, 20);
            }
            t.text = mathPicker.ToString();
        }

        answers.Insert(correctAnswerIndex, correctAnswer);

        // Enable question.
        MathUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        desktopPlayer.GetComponent<DesktopMovement>().enabled = false;
        desktopPlayer.GetComponent<Inventory>().forceHold = true;
    }

    // Register answer input from player.
    public void ButtonInput(int a)
    {
        AnswerQuestion(a);
    }

    // Check if chosen answer is identical to actual answer.
    public void AnswerQuestion(int input)
    {
        if (hasCaughtFish)
        {
            string selectedAnswer = answers[input].text;
            string answerString = answer.ToString();

            // Check answers
            if (selectedAnswer == answerString)
            {
                // Show fish information.
                ResetGame(3);
            }
            else
            {
                // Return fish to water.
                ResetGame(2);
            }
        }
    }

    // Reset minigame to original state.
    void ResetGame(int resetType)
    {
        switch (resetType)
        {
            default: break;
            case 1: fishingArea.SetActive(false); break; // Complete reset.
            case 2: fishingArea.SetActive(true); break; // Reset minigame to be instantly replayable.
            case 3: fishingArea.SetActive(false); // Complete question; show fish info & delete after 7.5s.
                    infoUI.SetActive(true); 
                    StartCoroutine(RemoveInfo()); break;
        }

        gameObject.GetComponent<MeshFilter>().mesh = fishingrod1;

        caughtFish = null;
        hasBaitedFish = false;
        hasCaughtFish = false;
        canStart = true;

        MathUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        desktopPlayer.GetComponent<DesktopMovement>().enabled = true;
        desktopPlayer.GetComponent<Inventory>().forceHold = false;

        Destroy(castedFloat);
        Destroy(instantiatedFish);
    }

    // Remove fish info after 7.5s.
    IEnumerator RemoveInfo()
    {
        yield return new WaitForSeconds(7.5f);
        infoUI.SetActive(false);
        fishingArea.SetActive(true);
    }

    // Defreeze the RigidBody of the pickupable fishing rod to be able to hold it and drop it.
    void DefreezeRigidBody()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
}