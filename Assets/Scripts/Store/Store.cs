using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        /*
        if (other.CompareTag("Player"))
            Debug.Log("Player in vicinity");
        */

        if (!PlayerInputManager.instance.goToStore)
            return;

        if (PlayerManager.instance.playerState == PlayerState.BOAT && other.CompareTag("Player") && !StoreManager.instance.inStore)
            StoreManager.instance.CheckStore();
    }
}