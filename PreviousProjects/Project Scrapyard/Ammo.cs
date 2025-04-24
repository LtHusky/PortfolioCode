using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public GunController singleShot;
    public Switch weaponSwitch;
    public AudioSource ammoPickUp;

    public int magSize = 36;
    public int currentAmmo;
    public int totalAmmo = 120;

    public float reloadTime = 2f;
    public bool isReloading;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = magSize;

        if (isReloading)
            return;

        if (currentAmmo == -1)
        {
            currentAmmo = magSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentAmmo = Mathf.Clamp(currentAmmo, 0, 36);
        totalAmmo = Mathf.Clamp(totalAmmo, 0, 120);

        if (Input.GetKeyDown(KeyCode.R) && singleShot.enabled)
        {
            StartCoroutine(Reload());
        }

        if (isReloading)
        {
            singleShot.enabled = false;
        }
        else
        {
            singleShot.enabled = true;
        }

        if (totalAmmo == 0 && currentAmmo == 0)
        {
            weaponSwitch.enabled = false;
        }
    }

    public IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        totalAmmo -= magSize;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magSize;
        isReloading = false;
    }

    public void AddAmmo(int ammoToAdd)
    {
        currentAmmo = currentAmmo + ammoToAdd;
        ammoPickUp.Play();
    }
}
