using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeBoons : MonoBehaviour
{
    // VARIABLES
    int chance;
    int minChance, maxChance;

    int boonChance;
    bool boonStart; 
    
    public ModSystem modSystem;

    int activeMods;
    int reloadTime;
    int fireRate;
    bool canShoot; //test (gun var)
    bool isReloading; //test (gun var)

    // FUNCTIONS
    void Start()
    {
        minChance = 0;
        boonStart = true;
        reloadTime = 1; //test (unknown...)
    }

    // Boon chance per bullet.
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space)) //test (per bullet)
        {
            activeMods = modSystem.activeMods;
            maxChance = activeMods * 3;
            chance = Random.Range(0, 100);
            if (chance >= minChance && chance <= maxChance)
            {
                BoonActive();
            }
        }
    }

    // Boon system.
    public void BoonActive()    
    {
        if (boonStart == true)
        {
            // Default chances.
            int[] chanceStuff = { 0, 25, 50, 75, 100 };
            // Chances the default chances for all the mods.
            if (activeMods != 0)
            {
                for (int i = 0; i < activeMods; i++)
                {
                    chanceStuff = ChangeChance(modSystem.activeModsList[i], chanceStuff);
                }
            }
            // Pick Boon.
            boonChance = Random.Range(0, chanceStuff[chanceStuff.Length - 1]);
            if (boonChance >= chanceStuff[0] && boonChance <= chanceStuff[1])
            {
                int oldFireRate = fireRate;
                StartCoroutine(Sputter(oldFireRate));
            }
            else if (boonChance >= chanceStuff[1] && boonChance <= chanceStuff[2])
            {
                StartCoroutine(Reload());
            }
            else if (boonChance >= chanceStuff[2] && boonChance <= chanceStuff[3])
            {
                StartCoroutine(DropMag());
            }
            else if (boonChance >= chanceStuff[3] && boonChance <= chanceStuff[4])
            {
                StartCoroutine(Jam());
            }
        }
    }

    // Change specific Boon chances.
    public int[] ChangeChance(GameObject modType, int[] chanceStuff) 
    {
        for (int x = 0; x < modType.GetComponent<Mods>().boonTypes.Length; x++)
        {
            for (int i = modType.GetComponent<Mods>().modType; i < chanceStuff.Length; i++)
            {
                chanceStuff[i] = chanceStuff[i] + modType.GetComponent<Mods>().boonsChance[x];
            }
            boonChance += modType.GetComponent<Mods>().boonsChance[x];
        }
        return (chanceStuff);
    }

    // Barrel Sputter Boon.
    IEnumerator Sputter(int oldFireRate, int counter = 0)  
    {
        Debug.Log("Sputter"); //test
        boonStart = false;
        float waitTime = 5;
        do // Sputter
        {
            yield return new WaitForSecondsRealtime(2f);
            if (counter < waitTime)
            {
                fireRate = Random.Range(1, 1000);
            }
            else if (counter == waitTime)
            {
                fireRate = oldFireRate;
            }
            counter += 1;
        }
        while (counter <= waitTime);
        boonStart = true;
    }

    // Stock Reload Boon.
    IEnumerator Reload()   
    {
        Debug.Log("Reload"); //test
        boonStart = false;
        //Gun.Reload();
        yield return new WaitForSeconds(reloadTime * 2); //test
        boonStart = true;
    }

    // Magazine DropMag Boon.
    IEnumerator DropMag()  
    {
        Debug.Log("DropMag"); //test
        canShoot = false;
        boonStart = false;
        // Drop magazine & reload.
        yield return new WaitForSeconds(reloadTime * 2);
        canShoot = true;
        boonStart = true;
    }

    // Trigger Jam Boon.
    IEnumerator Jam()   
    {
        Debug.Log("Jam"); //test
        canShoot = false;
        boonStart = false;
        yield return new WaitForSeconds(2);
        canShoot = true;
        boonStart = true;
    }
}