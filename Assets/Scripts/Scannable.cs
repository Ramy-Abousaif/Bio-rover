using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    COMMON = 1,
    UNCOMMON = 2,
    RARE = 3,
    EXOTIC = 5
}

public class Scannable : MonoBehaviour
{
    public bool scanned = false;
    public float baseValue = 10;
    public Rarity rarity;
}
