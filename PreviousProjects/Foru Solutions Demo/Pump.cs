using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Pump : MonoBehaviour
{
	// Skimmer.
	[Header("Skimmer")]
	public GameObject floatPiece;
	public BoxCollider floatCol;
	Vector3 floatColOrigin;
	bool isAdjustingFloat;

	float inlateGap;
	public float wheelValue;

	// Pump.
	[Header("Pump")]
	public float pumpSpeed = 0;
	public float pumpPercentage;

	public float startOilInWater = 1000;
	public float PumpedUpOil;

	float runningPumped = 0;
	bool pumpOn;
	bool pumpingWater;
	bool pumpingOil;

	float pumpTimer;
	int pumpTimerSeconds;
	float maxSpeed = 20;
	float speedDifference;

	// Oil & water.
	[Header("Oil & water")]
	public GameObject oilHolder;
	public GameObject oilLayer;

	public float oilHeight = 1;
	float oilHeightSteps = 100;
	float oilHeightPercentage = 100;

	public MeshRenderer oilMatTM;
	public Shader waterShader;

	public Color endColor_Shallow;
	public Color endColor_Deep;

	// Points system.
	[Header("Points")]
	float startOilHeight;

	float currentTime;
	float bestTime = 115;
	float endTimeScore;
	string endTimeString;

	float waterPumped;
	float bestWaterScore = 1;
	float endWaterScore;

	float points;

	// UI.
	[Header("UI")]
	public TextMeshProUGUI oilLayerThicknessText;
	public TextMeshProUGUI inlateGapText;
	public TextMeshProUGUI percentageText;
	public TextMeshProUGUI pumpSpeedText;
	public Image oilPercentageFillImage;
	[Space]
	public GameObject warningSign;
	public TextMeshProUGUI warningText;
	public TextMeshProUGUI contextText;
	[Space]
	public TextMeshProUGUI endTimerText;
	public TextMeshProUGUI endOilText;
	public TextMeshProUGUI endWaterText;
	public TextMeshProUGUI endPointsText;
	bool warningAbleToCall = true;

	// Animation.
	[Header("Animation")]
	public Image fadeImage;
	public Animator animator;

	[Header("Other scripts")]
	public SliderManager sliderManagerScript;
	public SensorOutputManager sensorOutputScript;
	public AnimationsManager animationManager;
	bool sensorPipeCanStart = true;

	// Fixed start function; Apply main menu settings visually.
	public void FixedStart()
    {
		oilHolder.transform.localScale = new Vector3(oilLayer.transform.localScale.x, oilHeight, oilLayer.transform.localScale.z);
		startOilHeight = oilHeight; 
		floatColOrigin = floatCol.center;
	}

	void FixedUpdate()
	{
		TextUpdate();
		InlateGapSettings();

		// Activate oil pump.
		if (pumpOn && inlateGap > 0)
		{
			float tempPumpPercentage = PumpedUpOil / startOilInWater;
			pumpPercentage = tempPumpPercentage * 100;

			UpdateTimers();
			Oilpump();
			OilSettings();
			PumpSpeedSettings();
			PumpWater();

			if (sensorPipeCanStart)
			{
				sensorOutputScript.StartFlow();
				sensorPipeCanStart = false;
			}
			else if (pumpSpeed == 0)
			{
				sensorOutputScript.StopFlow();
				sensorPipeCanStart = true;
			}
		}
		else if (sensorPipeCanStart == false)
        {
			sensorOutputScript.StopFlow();
			sensorPipeCanStart = true;
		}
	}

	// UI elements.
	public void SetSliderSpeed(float value)
	{
		pumpOn = true;
		pumpSpeed = (int)value / 10;
	}

	IEnumerator EnableWarning(string warning, string context)
    {
		if (!warningSign.activeSelf)
        {
			warningText.text = warning;
			contextText.text = context;
			warningSign.SetActive(true);

			yield return new WaitForSeconds(4);
			warningSign.SetActive(false);
			warningAbleToCall = true;
		}
	}

    void TextUpdate()
	{
		float tempThickness = oilHeight * 10;
		oilLayerThicknessText.text = "Oil thickness: " + tempThickness.ToString("0.0") + "CM";
		inlateGapText.text = "Inlet gap: " + inlateGap.ToString("0.0") + "CM";
		percentageText.text = (int)pumpPercentage + "%";
		oilPercentageFillImage.fillAmount = pumpPercentage / 100;
		pumpSpeedText.text = "Speed: " + pumpSpeed + " m3/h";
	}

	// Timer.
	void UpdateTimers()
    {
		currentTime += Time.deltaTime;
		TimeSpan timePlaying = TimeSpan.FromSeconds(currentTime);
		endTimeString = "Time spent: " + timePlaying.ToString("mm':'ss'.'ff");

		pumpTimer += Time.deltaTime;
		pumpTimerSeconds = (int)pumpTimer % 60;
	}

	// Game settings.
	void OilSettings()
	{
		// Calculate & set oil height.
		oilHeightPercentage = 100 - pumpPercentage;

		if (oilHeightPercentage <= oilHeightSteps - 1)
		{
			oilHeight -= startOilHeight / 100;
			oilHeightSteps -= 1;

			// Scale update.
			Vector3 oilLayerScale = new Vector3(oilLayer.transform.localScale.x, oilHeight, oilLayer.transform.localScale.z);
			oilHolder.transform.localScale = oilLayerScale;
		}
	}

	void InlateGapSettings()
	{
		inlateGap = wheelValue;
		inlateGap *= 2.5f;

		float adjustment = inlateGap / 20;

		if (isAdjustingFloat == false)
        {
			floatCol.center = new Vector3(0, 0, 0.13f + adjustment);
		}
		floatPiece.transform.position = new Vector3(0, gameObject.transform.position.y + adjustment - 0.25f, gameObject.transform.position.z);

		// Check inlate gap settings.
		float tempHeight = oilHeight * 10;

		if (inlateGap > 0.2f && inlateGap > tempHeight && pumpSpeed > 0)
        {
			if (warningAbleToCall)
			{
				StartCoroutine(EnableWarning("Skimmer is pumping water!", "Cause: Inlet gap is too wide."));
				pumpingWater = true;
				warningAbleToCall = false;
			}
		}
		else if (inlateGap <= 2 && inlateGap > 0 && pumpSpeed > 40)
		{
			StartCoroutine(RiseSkimmer());

		}
		else if (inlateGap <= 1 && inlateGap > 0 && pumpSpeed > 20)
        {
			StartCoroutine(RiseSkimmer());
		}
        else if (pumpSpeed == 0 || pumpingWater)
        {
			floatCol.center = Vector3.Lerp(floatCol.center, floatColOrigin, 0.5f * Time.deltaTime);
			pumpingWater = false;
		}
    }

	IEnumerator RiseSkimmer()
    {
		if (warningAbleToCall)
		{
			StartCoroutine(EnableWarning("Skimmer is rising out of the water!", "Cause: You are pumping too fast."));
			warningAbleToCall = false;
		}

		isAdjustingFloat = true;
		waterPumped += 100f;
		PumpOff();
		floatCol.center = new Vector3(0, 0, -0.5f);

		yield return new WaitForSeconds(4);
		sliderManagerScript.EnableInteraction();
		isAdjustingFloat = false;
	}

	void PumpSpeedSettings()
	{
        switch (pumpTimerSeconds)
        {
			case 0: maxSpeed = 20; break;
			case 8: maxSpeed = 30; break;
			case 16: maxSpeed = 40; break;
			case 24: maxSpeed = 60; break;
			case 30: maxSpeed = 100; break;
		}

		if (pumpSpeed > maxSpeed)
        {
			if (warningAbleToCall)
			{
				StartCoroutine(EnableWarning("Skimmer is pumping water!", "Cause: You are pumping too fast!"));
			}
			pumpingWater = true;
			warningAbleToCall = false;

			speedDifference = pumpSpeed - maxSpeed;
			if (speedDifference > 10)
            {
				pumpTimer = 0;
			}
		}
	}

	// Pump.
	void Oilpump()
	{
		// Oilpump process.
		if (PumpedUpOil < startOilInWater)
		{
			if (pumpSpeed > 0 && pumpingOil)
			{
				runningPumped += Time.deltaTime;
				if (runningPumped < 1.0f)
				{
					runningPumped *= pumpSpeed / 10;
				}
				else
				{
					runningPumped = 0;
				}
				PumpedUpOil += runningPumped;
			}
		}
		else
		{
			EndGame();
		}
	}

	void PumpOff()
	{
		pumpSpeed = 0;
		pumpOn = false;
		pumpingWater = false;
		pumpingOil = false;
		sliderManagerScript.DisableInteraction();
		sensorOutputScript.StopFlow();
		sensorPipeCanStart = true;
		pumpTimer = 0;
	}

	void PumpWater()
    {
		if (pumpingWater)
        {
			float speedMultiplier = pumpSpeed * 2.777777777777778f;
			waterPumped += Time.deltaTime * speedMultiplier;

			// Set sensorpipe flow
			float tempOilHeight = oilHeight * 10;
			float temp = inlateGap - tempOilHeight;
			if (temp > 0.5f || speedDifference > 10)
			{
				sensorOutputScript.AddWaterToFlow(1);
				pumpingOil = false;
			}
			else
			{
				sensorOutputScript.AddWaterToFlow(0.5f);
			}
        }
        else
		{
			pumpingOil = true;
			sensorOutputScript.AddWaterToFlow(0);
		}
    }

	// End game.
	void EndGame()
    {
		// End game.
		oilHeight = 0;
		pumpPercentage = 100;

		oilMatTM.material.shader = waterShader;
		oilMatTM.material.SetColor("_DeepWaterColor", endColor_Deep);
		oilMatTM.material.SetColor("_ShallowWaterColor", endColor_Shallow);
		oilHolder.transform.localScale = new Vector3(1, 0.0001f, 1);

		PumpOff();

		// To summary screen.
		fadeImage.enabled = true;
		animator.SetTrigger("FadeIn");
		animationManager.EnableSummary();

		float tempWaterPumped = waterPumped / 100;

		endTimeScore = 60 * Mathf.Pow(1.1f, bestTime - currentTime);
		endWaterScore = 40 * Mathf.Pow(1.04f, bestWaterScore - tempWaterPumped);

		points = endTimeScore + endWaterScore;
		points = Mathf.Clamp(points, 0, 100);
		Debug.Log("TIME POINTS: " + endTimeScore);
		Debug.Log("WATER POINTS: " + endWaterScore);

		endTimerText.text = endTimeString;
		endOilText.text = "Oil pumped: " + PumpedUpOil.ToString("000") + "L";
		endWaterText.text = "Water pumped: " + waterPumped.ToString("000") + "L";
		endPointsText.text = "Points: " + points.ToString("000");
	}
}