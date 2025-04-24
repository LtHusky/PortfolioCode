using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorOutputManager : MonoBehaviour
{
    ParticleSystem sensorOutput;

    void Start()
    {
        sensorOutput = gameObject.GetComponent<ParticleSystem>();
    }

    public void AddWaterToFlow(float amount)
    {
        var main = sensorOutput.main;
        var emission = sensorOutput.emission;

        switch (amount)
        {
            default: Debug.Log("SensorOutPutManager's AddWater(amount) is not valid."); break;
            // 100% water
            case 1:
                main.startLifetime = 0.5f;
                main.startSpeed = -50;
                main.startSize = 0.05f;
                main.startColor = Color.white;
                emission.rateOverTime = 50;
                break;
            // 50 water
            case 0.5f:
                main.startLifetime = 0.5f;
                main.startSpeed = -50;
                main.startSize = 0.05f;
                main.startColor = Color.black;
                emission.rateOverTime = 50;
                break;
            // 0% water
            case 0:
                main.startLifetime = 1;
                main.startSpeed = -1;
                main.startSize = 0.03f;
                main.startColor = Color.black;
                emission.rateOverTime = 20;
                break;
        }
    }

    public void StartFlow()
    {
        sensorOutput.Play();
    }

    public void StopFlow()
    {
        sensorOutput.Stop();
    }
}
