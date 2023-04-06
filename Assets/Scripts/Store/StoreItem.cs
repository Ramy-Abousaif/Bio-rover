using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "storeMenu", menuName ="Scriptable Objects/Store Item")]
public class StoreItem : ScriptableObject
{
    public string title;
    public string description;
    public int baseCost;
    public int currentProgress;
    public int capacity;
}
