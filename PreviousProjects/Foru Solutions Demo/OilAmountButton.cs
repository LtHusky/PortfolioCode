using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilAmountButton : MonoBehaviour
{
    public bool generateRandom;
    public float oilAmount;
    public float oilHeight;
    public float bestTime;

    float[] oilAmounts = { 2000, 2500, 3000, 3500, 4000 };
    float[] oilHeights = { 0.2f, 0.4f, 0.6f, 0.8f, 1f };
    float[] bestTimes = { 90, 90, 105, 110, 115 };

    public Pump pumpScript;

    // Set oil amount
    public void SetOilAmount()
    {
        if (generateRandom == false && oilAmount < 2000)
        {
            Debug.Log("Invalid oil amount.");
        }
        // Randomize oil amount
        else if (generateRandom)
        {
            int randomIDX = Random.Range(0, 4);
            oilAmount = oilAmounts[randomIDX];
            oilHeight = oilHeights[randomIDX];
            bestTime = bestTimes[randomIDX];
        }

        SubmitAmount();
    }

    void SubmitAmount()
    {
        pumpScript.startOilInWater = oilAmount;
        pumpScript.oilHeight = oilHeight;
    }
}
