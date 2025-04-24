using UnityEngine;
using System.Collections.Generic;

public class ModSystem : MonoBehaviour
{
    // VARIABLES
    public int fireRate, reloadSpeed, magSize, recoilRate, spread; // Placeholder Gun Variables
    public int activeMods;

    public Mods mod1, mod2, mod3;

    public List<GameObject> totalModsList;
    public List<GameObject> activeModsList;

    // FUNCTIONS
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))    //test (mod pickup)
        {
            GameObject modd = Instantiate(totalModsList[Random.Range(0, 6)], Vector3.zero, Quaternion.identity) as GameObject;
            ModCheck(modd.GetComponent<Mods>());
        }
    }

    // Check if mod is installed or not.
    public void ModCheck(Mods mod)    
    {
        switch (mod.modType)
        {
            case 1: if (mod1 == null) { InstallMod(mod); mod1 = mod; } else { RemoveMod(mod1); InstallMod(mod); mod1 = mod; } break;
            case 2: if (mod2 == null) { InstallMod(mod); mod2 = mod; } else { RemoveMod(mod2); InstallMod(mod); mod2 = mod; } break;
            case 3: if (mod3 == null) { InstallMod(mod); mod3 = mod; } else { RemoveMod(mod3); InstallMod(mod); mod3 = mod; } break;
            case 4: if (mod1 == null) { InstallMod(mod); mod1 = mod; } else { RemoveMod(mod1); InstallMod(mod); mod1 = mod; } break;
            case 5: if (mod2 == null) { InstallMod(mod); mod2 = mod; } else { RemoveMod(mod2); InstallMod(mod); mod2 = mod; } break;
            case 6: if (mod3 == null) { InstallMod(mod); mod3 = mod; } else { RemoveMod(mod3); InstallMod(mod); mod3 = mod; } break;
        }
    }

    // Install stats set by mod.
    public void InstallMod(Mods mod)   
    {
        activeModsList.Add(mod.gameObject);
        fireRate += mod.newFireRate;
        reloadSpeed += mod.newReloadSpeed;
        magSize += mod.newMagSize;
        recoilRate += mod.newRecoilRate;
        spread += mod.newSpread;
        activeMods++;
        ValClamp();
    }

    // Remove stats set by mod.
    public void RemoveMod(Mods mod)    
    {
        activeModsList.Remove(mod.gameObject);
        fireRate -= mod.newFireRate;
        reloadSpeed -= mod.newReloadSpeed;
        magSize -= mod.newMagSize;
        recoilRate -= mod.newRecoilRate;
        spread -= mod.newSpread;
        activeMods--;
        ValClamp();
    }

    // Clamp variables.
    public void ValClamp()    
    {
        fireRate = Mathf.Clamp(fireRate, 1, 1000);
        reloadSpeed = Mathf.Clamp(reloadSpeed, 1, 1000);
        magSize = Mathf.Clamp(magSize, 1, 1000);
        recoilRate = Mathf.Clamp(recoilRate, 1, 1000);
        spread = Mathf.Clamp(spread, 1, 1000);
    }
}