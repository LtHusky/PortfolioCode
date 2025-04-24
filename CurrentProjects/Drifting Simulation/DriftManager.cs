using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class DriftManager : MonoBehaviour
{
    public Rigidbody playerRB;

    [Header("Scoring")]
    float speed;
    float driftAngle;
    float driftFactor = 1;
    float currentScore;
    float totalScore;
    bool isDrifting;

    public float minSpeed = 5;
    public float minAngle = 10;
    public float driftingDelay = 0.2f;

    IEnumerator stopDriftingCoroutine = null;

    [Header("UI")]
    public TMP_Text currentScoreText;
    public TMP_Text totalScoreText;
    public TMP_Text factorText;
    public TMP_Text driftAngleText;
    public GameObject driftingUI;
    public Color normalDriftColor;
    public Color nearDriftEndColor;
    public Color endDriftColor;


    void Start()
    {
        driftingUI.SetActive(false);
    }

    void Update()
    {
        ManageDrift();
        ManageUI();
    }

    // Manage vehicle drift points by calculating vehicle speed & angle.
    void ManageDrift()
    {
        // Calculate vehicle speed & angle.
        speed = playerRB.velocity.magnitude;
        driftAngle = Vector3.Angle(playerRB.transform.forward, (playerRB.velocity + playerRB.transform.forward).normalized);
        if (driftAngle > 120)
        {
            driftAngle = 0;
        }

        // Enable drift when vehicle speed & angle are within limits.
        if (driftAngle >= minAngle && speed > minSpeed)
        {
            if (!isDrifting || stopDriftingCoroutine != null)
            {
                StartDrift();
            }
        }
        else
        {
            if (isDrifting && stopDriftingCoroutine == null)
            {
                StopDrift();
            }
        }

        // Set score & UI.
        if (isDrifting)
        {
            currentScore += Time.deltaTime * driftAngle * driftFactor;
            driftFactor += Time.deltaTime;
            driftingUI.SetActive(true);
        }
    }

    // Enable drifting.
    async void StartDrift()
    {
        if (!isDrifting)
        {
            await Task.Delay(Mathf.RoundToInt(1000*driftingDelay));
            driftFactor = 1;
        }
        if (stopDriftingCoroutine != null)
        {
            StopCoroutine(stopDriftingCoroutine);
            stopDriftingCoroutine = null;
        }
        currentScoreText.color = normalDriftColor;
        isDrifting = true;
    }

    // Stop drifting.
    void StopDrift()
    {
        stopDriftingCoroutine = StoppingDrift();
        StartCoroutine(stopDriftingCoroutine);
    }

    // Show player score & resetting stats before stopping drift fully.
    IEnumerator StoppingDrift()
    {
        yield return new WaitForSeconds(0.1f);
        currentScoreText.color = nearDriftEndColor;
        yield return new WaitForSeconds(driftingDelay * 4f);
        totalScore += currentScore;
        isDrifting = false;
        currentScoreText.color = endDriftColor;
        yield return new WaitForSeconds(0.5f);
        currentScore = 0;
        driftingUI.SetActive(false);
    }

    void ManageUI()
    {
        totalScoreText.text = "Total score: " + totalScore.ToString("###,###,##0");
        factorText.text = driftFactor.ToString("###,###,##0.0") + "x";
        currentScoreText.text = currentScore.ToString("###,###,##0");
        driftAngleText.text = driftAngle.ToString("###,###,##0") + "°";
    }
}
