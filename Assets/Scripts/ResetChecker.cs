using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetChecker : MonoBehaviour
{
    public bool noBlocks = true;

    private void Awake()
    {
        noBlocks = true;
    }

    private void OnEnable()
    {
        noBlocks = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Default") || other.gameObject.layer == LayerMask.NameToLayer("Homebase"))
            noBlocks = false;
    }
}
