using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    public Transform return_point;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StoreManager.instance.CheckStore();
    }
}
