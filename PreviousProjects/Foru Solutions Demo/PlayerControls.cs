using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    public Slider slider;
    public WheelBehaviour wheelBehaviour;
    bool isPlaying = false;

    void Update()
    {
        if (isPlaying)
        {
            // Modify slider values with W & S input keys.
            if (Input.GetKey(KeyCode.W))
            {
                slider.value++;
            }
            if (Input.GetKey(KeyCode.S))
            {
                slider.value--;
            }
        }
    }

    public void ToggleControls()
    {
        if (isPlaying)
        {
            isPlaying = false;
        }
        else
        {
            isPlaying = true;
        }
    }
}
