using System.Collections;
using TMPro;
using UnityEngine;

public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
};

public class CarController : MonoBehaviour
{
    // Variables
    Rigidbody rb;

    // Engine variables
    [Header("Engine")]
    public float isEngineRunning;
    public float accInput;
    public float horsepower;
    public float RPM;
    public float idleRPM;
    public float redLineRPM;
    public float maxSpeed;
    public AnimationCurve hpToRPMCurve;

    float wheelRpm;
    float speed;
    float speedClamped;

    // Transmission variables
    [Header("Transmission")]
    GearState gearState;
    public int currentGear;
    public float[] gearRatios;

    public float differentialRatio;
    public float currentTorque;
    public float clutch;
    public float RPMToIncreaseGear;
    public float RPMToDecreaseGear;
    public float secondsToChangeGear = 0.5f;

    // Braking system
    [Header("Braking system")]
    public float brakeInput;
    public float brakePower;
    float slipAngle;
    public Material brakeLightMaterial;
    public Color brakingColor;
    public float brakeColorIntensity;

    // Steering
    [Header("Steering")]
    public AnimationCurve steeringCurve;
    public float steerInput;

    // Wheels
    [Header("Wheels")]
    public WheelColliders wColliders;
    public Wheels wheels;

    // VFX
    [Header("VFX")]
    public WheelParticles wParticles;
    public GameObject tireTrail;

    // UI
    [Header("UI")]
    public TMP_Text rpmText;
    public TMP_Text gearText;
    public Transform rpmNeedle;
    public float minNeedleRotation;
    public float maxNeedleRotation;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // UI
        rpmNeedle.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(minNeedleRotation, maxNeedleRotation, RPM / (redLineRPM * 1.1f)));
        rpmText.text = RPM.ToString("0.000") + "RPM";
        gearText.text = (gearState == GearState.Neutral)?"N":(currentGear + 1).ToString();

        speed = wColliders.RRWheel_collider.rpm * wColliders.RRWheel_collider.radius * 2 * Mathf.PI / 10;
        speedClamped = Mathf.Lerp(speedClamped, speed, Time.deltaTime);


        CheckInput();
        SetParticles();
        ApplyWheelMesh();
        ApplyTorque();
        ApplySteering();
        ApplyBrake();
    }

    // Check player input. (Acceleration, steering, braking and use of clutch)
    void CheckInput()
    {
        accInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(accInput) > 0 && isEngineRunning == 0)
        {
            StartCoroutine(GetComponent<EngineAudio>().StartEngine());
            gearState = GearState.Running;
        }

        slipAngle = Vector3.Angle(transform.forward, rb.velocity - transform.forward);


        if (slipAngle < 120f)
        {
            if (accInput < 0)
            {
                brakeInput = Mathf.Abs(accInput);
                accInput = 0;
            }
        }
        else
        {
            brakeInput = 0;
        }

        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0;
                if (accInput > 0)
                {
                    gearState = GearState.Running;
                }
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
            }
        }
        else
        {
            clutch = 0;
        }
    }

    // Enable particles. (Tire smoke and tire trails)
    void SetParticles()
    {
        WheelHit[] wheelHits = new WheelHit[2];
        wColliders.RRWheel_collider.GetGroundHit(out wheelHits[0]);
        wColliders.RLWheel_collider.GetGroundHit(out wheelHits[1]);

        float slipAllowance = 0.3f; // HIGHER = LESS SMOKE WHEN SLIPPING

        if (Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance &! wParticles.RRWheelParticle.isPlaying)
        {
            wParticles.RRWheelParticle.Play();
            wParticles.RRWheelTrail.emitting = true;
        }
        else if (Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) < slipAllowance && wParticles.RRWheelParticle.isPlaying)
        {
            wParticles.RRWheelParticle.Stop();
            wParticles.RRWheelTrail.emitting = false;
        }
        if (Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance &! wParticles.RLWheelParticle.isPlaying)
        {
            wParticles.RLWheelParticle.Play();
            wParticles.RLWheelTrail.emitting = true;
        }
        else if (Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) < slipAllowance && wParticles.RLWheelParticle.isPlaying)
        {
            wParticles.RLWheelParticle.Stop();
            wParticles.RLWheelTrail.emitting = false;
        }
    }

    // Move vehicle with calculated wheel power.
    void ApplyTorque()
    {
        currentTorque = CalculateTorque();
        wColliders.RRWheel_collider.motorTorque = currentTorque * accInput;
        wColliders.RLWheel_collider.motorTorque = currentTorque * accInput;
    }

    // Calculate engine power and user input to wheel power.
    float CalculateTorque()
    {
        float torque = 0;
        if (RPM < idleRPM + 200 && accInput == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }

        // Initiate gearshift with assigned gears to shift to.
        if (gearState == GearState.Running && clutch > 0)
        {
            if (RPM > RPMToIncreaseGear)
            {
                StartCoroutine(ChangeGear(1));
            }
            else if (RPM < RPMToDecreaseGear)
            {
                // Check if car has stopped to instantly shift to neutral.
                if (RPM <= 900)
                {
                    StartCoroutine(ChangeGear(-currentGear));
                }
                // Regular downshift.
                else
                {
                    StartCoroutine(ChangeGear(-1));
                }
            }
        }

        if (isEngineRunning > 0)
        {
            if (clutch < 0.1f)
            {
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLineRPM * accInput) + Random.Range(-50, 50), Time.deltaTime);
            }
            else
            {
                wheelRpm = Mathf.Abs((wColliders.RRWheel_collider.rpm + wColliders.RLWheel_collider.rpm) / 2f) * gearRatios[currentGear] * differentialRatio;
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRpm), Time.deltaTime * 3f);
                torque = hpToRPMCurve.Evaluate(RPM / redLineRPM) * horsepower / RPM * gearRatios[currentGear] * differentialRatio * 5252f * clutch;
            }
        }
        return torque;
    }

    // Steer vehicle with player input and steering angle. (Steering)
    void ApplySteering()
    {
        float steeringAngle = steerInput * steeringCurve.Evaluate(speed);
        wColliders.FRWheel_collider.steerAngle = steeringAngle;
        wColliders.FLWheel_collider.steerAngle = steeringAngle;
    }

    // Brake vehicle with player input and brake power. (Braking)
    void ApplyBrake()
    {
        wColliders.FRWheel_collider.brakeTorque = brakeInput * brakePower * 0.7f;
        wColliders.FLWheel_collider.brakeTorque = brakeInput * brakePower * 0.7f;

        wColliders.RRWheel_collider.brakeTorque = brakeInput * brakePower * 0.3f;
        wColliders.RLWheel_collider.brakeTorque = brakeInput * brakePower * 0.3f;

        if (brakeLightMaterial)
        {
            if (brakeInput > 0)
            {
                brakeLightMaterial.EnableKeyword("_EMISSION");
                brakeLightMaterial.SetColor("_EmissionColor", brakingColor * Mathf.Pow(2, brakeColorIntensity));
            }
            else
            {
                brakeLightMaterial.DisableKeyword("_EMISSION");
                brakeLightMaterial.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    // Move wheel mesh with wheel colliders.
    void ApplyWheelMesh()
    {
        UpdateWheels(wColliders.FRWheel_collider, wheels.FRWheelHolder);
        UpdateWheels(wColliders.FLWheel_collider, wheels.FLWheelHolder);
        UpdateWheels(wColliders.RRWheel_collider, wheels.RRWheelHolder);
        UpdateWheels(wColliders.RLWheel_collider, wheels.RLWheelHolder);
    }

    // Update position and rotation of a single wheel.
    void UpdateWheels(WheelCollider coll, GameObject wheelMesh)
    {
        Quaternion quat;
        Vector3 pos;
        coll.GetWorldPose(out pos, out quat);
        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = quat;
    }

    // Get speed to play engine sounds.
    public float GetSpeedRatio()
    {
        var gas = Mathf.Clamp(Mathf.Abs(accInput), 0.5f, 1f);
        return RPM * gas / redLineRPM;
    }

    // Change gear of vehicle's transmission.
    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;
        if (currentGear + gearChange >= 0)
        {
            if (gearChange > 0)
            {
                // Increase gear.
                yield return new WaitForSeconds(0.7f);
                if (RPM < RPMToIncreaseGear || currentGear > gearRatios.Length - 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if (gearChange < 0)
            {
                // Decrease gear.
                yield return new WaitForSeconds(0.1f);
                if (RPM > RPMToDecreaseGear || currentGear <= 0)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            gearState = GearState.Changing;
            yield return new WaitForSeconds(secondsToChangeGear);
            currentGear += gearChange;
        }

        if (gearState != GearState.Neutral)
        {

        }
        gearState = GearState.Running;
    }
}

// - CLASSES -
[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel_collider;
    public WheelCollider FLWheel_collider;
    public WheelCollider RRWheel_collider;
    public WheelCollider RLWheel_collider;
}

[System.Serializable]
public class Wheels
{
    public GameObject FRWheelHolder;
    public GameObject FLWheelHolder;
    public GameObject RRWheelHolder;
    public GameObject RLWheelHolder;
}

[System.Serializable]
public class WheelParticles
{
    public ParticleSystem RRWheelParticle;
    public ParticleSystem RLWheelParticle;

    public TrailRenderer RRWheelTrail;
    public TrailRenderer RLWheelTrail;
}
