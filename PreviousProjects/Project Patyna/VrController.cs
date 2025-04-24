using UnityEngine;
using UnityEngine.UI;

public class VrController : MonoBehaviour
{
    // VARIABLES
    const KeyCode GoTrigger = KeyCode.JoystickButton15;
    public Transform raycastOutPoint;

    private Transform transformImage;

    // FUNCTIONS
    void Update()
    { // Hit object with raycast.
        RaycastHit hit;

        // Change image color.
        if (Physics.Raycast(raycastOutPoint.position, transform.forward, out hit, Mathf.Infinity))
        {
            transformImage = hit.transform;
            transformImage.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        else
        {
            if (transformImage != null)
            {
                if (transformImage.GetComponent<Image>() != null)
                    transformImage.GetComponent<Image>().color = Color.white;
            }
        }
        // Call function on raycast hit.
        if (Input.GetKeyDown(GoTrigger))
        { 
            if (Physics.Raycast(raycastOutPoint.position, transform.forward, out hit, Mathf.Infinity))
            {
                ButtonBehaviour target = hit.transform.GetComponent<ButtonBehaviour>();
                target.Active();
            }
        }
    }
}