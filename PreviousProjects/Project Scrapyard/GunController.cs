using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    [Header("Setup")]
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;
    public float fireRate = 5f;
    public int magSize = 16;
    private float timeToFire = 1f;
    public float fireDelay = 0.1f;
    public float spread = 15.0f;
    public float reloadSpeed = 2f;
    public float recoilRate = 1f;
    public MouseCamera mouseCamera;

    [Header("Settings")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject bulletHoles;
    public Transform shootPoint;
    public Camera fpsCam;

    [Header("Misc")]
    public int currentAmmo;
    public bool isReloading = false;
    public bool readyToFire = true;
    public float spreadFactor;
    private bool isAiming;
    private Vector3 originalPosition;

    [Header("ADS")]
    public float aodSpeed = 8f;
    public Vector3 aimPosition;
    public Text ammoText;
    public Camera myCamera;

    [Header("ModSystem")]
    public NegativeBoons boonSystem;

    void OnEnable()
    {
        if (currentAmmo == -1)
            currentAmmo = magSize;

        originalPosition = transform.localPosition;

        UpdateAmmoText();
    }

    void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload(reloadSpeed));
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1f / (fireRate);
            Shoot();
            boonSystem.Boonchance();
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            currentAmmo = magSize;
        }
        AimDownSights();
        UpdateAmmoText();
    }

    public IEnumerator Reload(float speed)
    {
        // the boon system that has a chance to expant the reload time while reloading
        int boonChance = Random.Range(0, GetComponent<NegativeBoons>().chanceStuff[GetComponent<NegativeBoons>().chanceStuff.Length - 1]);
        if (boonChance >= GetComponent<NegativeBoons>().chanceStuff[3] && boonChance <= GetComponent<NegativeBoons>().chanceStuff[4] && boonSystem.boonStart != true)
        {
            StartCoroutine(boonSystem.ReloadProblem());
        }
        else
        {
            isReloading = true;
            yield return new WaitForSeconds(speed);
            currentAmmo = magSize;
            isReloading = false;
            UpdateAmmoText();
        }
    }
    void Shoot()
    {
        if (readyToFire == true && transform.GetComponent<Melee>().meleeAttack != true)
        {
            UpdateAmmoText();
            mouseCamera.UpdateRecoil(recoilRate);
            currentAmmo--;

            muzzleFlash.Play();

            RaycastHit hit;

            Vector3 shootDirection = shootPoint.transform.forward;
            shootDirection += shootPoint.TransformDirection(new Vector3(Random.Range(-spread / 100, spread / 100), Random.Range(-spread / 100, spread / 100)));

            if (Physics.Raycast(myCamera.transform.position, shootDirection, out hit, range))
            {
                EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }

                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2000f);
            }
        }
    }

    private void AimDownSights()
    {
        if (Input.GetButton("Fire2") && !isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * aodSpeed);
            isAiming = true;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aodSpeed);
            isAiming = false;
        }
    }
    void UpdateAmmoText()
    {
        ammoText.text = currentAmmo.ToString();
    }
}