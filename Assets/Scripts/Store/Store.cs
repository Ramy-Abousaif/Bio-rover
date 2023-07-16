using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Store : MonoBehaviour
{
    
    private PlayerManager playerManager;
    private CinemachineVirtualCamera uiVCam;
    private ResearchBaseManager manager;
    public Canvas storeCanvas;


    void Start()
    {
        GameObject player = GameObject.Find("Player");
        playerManager = player.GetComponent<PlayerManager>();
        GameObject cam = GameObject.Find("MainBaseVCam");
        uiVCam = cam.gameObject.GetComponent<CinemachineVirtualCamera>();
        manager = storeCanvas.gameObject.GetComponent<ResearchBaseManager>();

    }

    private void OnTriggerStay(Collider other)
    {
        /*
        if (other.CompareTag("Player"))
            Debug.Log("Player in vicinity");
        */

        if (!PlayerInputManager.instance.goToStore)
            return;

        if (PlayerManager.instance.playerState == PlayerState.BOAT && other.CompareTag("Player") && !StoreManager.instance.inStore)
        {
            //StoreManager.instance.CheckStore();
            manager.enterResearchBase();
            //storeCanvas.gameObject.SetActive(true);
            //PlayerManager.instance.boatVCam.gameObject.SetActive(false);
            //uiVCam.gameObject.SetActive(true);
        }
    }
}
