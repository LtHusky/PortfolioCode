using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    // Variables
    [Header("Running sound")]
    public AudioSource runningSound;
    public float runningMaxVolume;
    public float runningMaxPitch;

    [Header("Reverse sound")]
    public AudioSource reverseSound;
    public float reverseMaxVolume;
    public float reverseMaxPitch;

    [Header("Idle sound")]
    public AudioSource idleSound;
    public float idleMaxVolume;

    [Header("Starting sound")]
    public AudioSource startingSound;

    [Header("Rev Limiter Sound Settings")]
    float revLimiter;
    public float revLimiterSound = 3f;
    public float revLimiterFrequency = 10f;
    public float revLimiterEngage = 0.8f;

    CarController carController;

    public bool isEngineRunning;

    float speedRatio;

    // Setup
    void Start()
    {
        carController = GetComponent<CarController>();
        idleSound.volume = 0;
        runningSound.volume = 0;
        reverseSound.volume = 0;
    }

    void Update()
    {
        float speedSign = 0;
        if (carController)
        {
            speedSign = Mathf.Sign(carController.GetSpeedRatio());
            speedRatio = Mathf.Abs(carController.GetSpeedRatio());
        }

        if (speedRatio > revLimiterEngage)
        {
            revLimiter = (Mathf.Sin(Time.time * revLimiterFrequency) + 1f) * revLimiterSound * (speedRatio - revLimiterEngage);
        }
        

        if (isEngineRunning)
        {
            idleSound.volume = Mathf.Lerp(0f, idleMaxVolume, speedRatio);

            if (speedSign > 0)
            {
                reverseSound.volume = 0;
                runningSound.volume = Mathf.Lerp(0f, runningMaxVolume, speedRatio);
                runningSound.pitch = Mathf.Lerp(0f, runningMaxPitch, speedRatio);
            }
            else
            {
                runningSound.volume = 0;
                reverseSound.volume = Mathf.Lerp(0f, reverseMaxVolume, speedRatio);
                reverseSound.pitch = Mathf.Lerp(0f, reverseMaxPitch, speedRatio);
            }
        }
        else
        {
            idleSound.volume = 0;
            runningSound.volume = 0;
            reverseSound.volume = 0;
        }
    }

    public IEnumerator StartEngine()
    {
        startingSound.Play();
        carController.isEngineRunning = 1;
        yield return new WaitForSeconds(0.6f);
        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);
        carController.isEngineRunning = 2;
    }
}
