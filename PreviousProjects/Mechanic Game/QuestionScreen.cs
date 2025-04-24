using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class QuestionScreen : MonoBehaviour
{
    GarageManager gm;

    [Header("Questions")]
    public Text questionText;
    public List<string> questions = new List<string>();
    public List<string> answersNames = new List<string>();
    public List<Text> answerTexts = new List<Text>();

    public string rightAnswer;
    public GameObject requiredTool;
    public GameObject live;

    [Header("Images")]
    public Image signImage;
    public Image liveImage;

    [Header("Sprites")]
    public Sprite orangeSign;
    public Sprite warning;
    public Sprite wrong;
    public Sprite correct;
    public Sprite emptyLive;

    public int lives = 2;

    void Start()
    {
        GameObject gmHolder = GameObject.Find("GarageManagerHolder");
        gm = gmHolder.GetComponent<GarageManager>();

        // Set question
        int randomQuestion = UnityEngine.Random.Range(0, questions.Count);

        string question = questions[randomQuestion];

        questionText.text = question;

        // Check question to match answers
        int answerIDX = 0;

        if (question == questions[0])
        {
            answerIDX = 0;

            for (int i = 0; i < 4; i++)
            {
                Text answerText = answerTexts[UnityEngine.Random.Range(0, answerTexts.Count - 1)];

                answerText.text = answersNames[answerIDX];
                answerIDX++;

                rightAnswer = answersNames[0];
                answerTexts.Remove(answerText);
            }
        }
        else if (question == questions[1])
        {
            answerIDX = 4;

            for (int i = 0; i < 4; i++)
            {
                Text answerText = answerTexts[UnityEngine.Random.Range(0, answerTexts.Count - 1)];

                answerText.text = answersNames[answerIDX];
                answerIDX++;

                rightAnswer = answersNames[4];
                answerTexts.Remove(answerText);
            }
        }
        else if (question == questions[2])
        {
            answerIDX = 8;

            for (int i = 0; i < 4; i++)
            {
                Text answerText = answerTexts[UnityEngine.Random.Range(0, answerTexts.Count - 1)];

                answerText.text = answersNames[answerIDX];
                answerIDX++;

                rightAnswer = answersNames[8];
                answerTexts.Remove(answerText);
            }
        }
    }
}
