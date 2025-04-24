using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WheelBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform Wheel;

    bool wheelBeingHeld = false;
    float wheelAngle = 0f;
    float lastWheelAngle = 0f;
    Vector2 center;

    public float maxSteerAngle;
    public float output;

    public GameObject skimmerWheel;
    public Pump pumpScript;

    void Update()
    {
        if (!wheelBeingHeld && wheelAngle != 0f)
        {
            if (0 > Mathf.Abs(wheelAngle))
            {
                wheelAngle = 0f;
            }
            else if (wheelAngle > 0f)
            {
                wheelAngle -= 0;
            }
            else
            {
                wheelAngle += 0;
            }
        }

        Wheel.localEulerAngles = new Vector3(0, 0, -maxSteerAngle * output);
        output = wheelAngle / maxSteerAngle;
        output++;
        pumpScript.wheelValue = output;

        //flip wheel rotation!!! (Left: open, Right: close)

        // Rotate Skimmer wheel obj
        skimmerWheel.transform.localRotation = Wheel.localRotation;
    }

    public void OnPointerDown(PointerEventData data)
    {
        wheelBeingHeld = true;
        center = RectTransformUtility.WorldToScreenPoint(data.pressEventCamera, Wheel.position);
        lastWheelAngle = Vector2.Angle(Vector2.up, data.position - center);
    }

    public void OnDrag(PointerEventData data)
    {
        float NewAngle = Vector2.Angle(Vector2.up, data.position - center);

        if ((data.position - center).sqrMagnitude >= 400)
        {
            if (data.position.x > center.x)
            {
                wheelAngle += NewAngle - lastWheelAngle;
            }
            else
            {
                wheelAngle -= NewAngle - lastWheelAngle;
            }
        }

        wheelAngle = Mathf.Clamp(wheelAngle, -maxSteerAngle, maxSteerAngle);
        lastWheelAngle = NewAngle;
    }

    public void OnPointerUp(PointerEventData data)
    {
        OnDrag(data);
        wheelBeingHeld = false;
    }
}