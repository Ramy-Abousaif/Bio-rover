using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScanRarity
{
    COMMON,
    UNCOMMON,
    RARE,
    EXOTIC
}

public class Scannable : MonoBehaviour
{
    public bool scanned = false;
    public ScanRarity rarity = ScanRarity.COMMON;
}
