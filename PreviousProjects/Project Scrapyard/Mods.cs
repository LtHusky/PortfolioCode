using System;
using UnityEngine;

public class Mods : MonoBehaviour
{
    // VARIABLES
    public int newFireRate { get; set; }
    public int newReloadSpeed { get; set; }
    public int newMagSize { get; set; }
    public int newRecoilRate { get; set; }
    public int newSpread { get; set; }

    public int modType;
    public int[] boonTypes;
    public int[] boonsChance;


    System.Random rand = new System.Random();

    // FUNCTIONS
    // Set mod vars
    public Mods()
    {
        newFireRate = rand.Next(-10, 10);
        newReloadSpeed = rand.Next(-10, 10);
        newMagSize = rand.Next(-10, 10);
        newRecoilRate = rand.Next(-10, 10);
        newSpread = rand.Next(-10, 10);
    }
}