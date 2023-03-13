using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreTemplate : MonoBehaviour
{
    public TMP_Text titleTxt;
    public TMP_Text descriptionTxt;
    public TMP_Text costTxt;
    public TMP_Text capacityTxt;

    public void UpdateProgress(int id)
    {
        switch (id)
        {
            case 0:
                StoreManager.instance.storeItems[id].currentProgress = PlayerManager.instance.bc.storedRovers.Count + PlayerManager.instance.bc.activeRovers.Count;
                break;
            case 1:
                StoreManager.instance.storeItems[id].currentProgress = GameManager.instance.sonarUpgrade;
                break;
            case 2:
                StoreManager.instance.storeItems[id].currentProgress = GameManager.instance.expandUpgrade;
                break;
            case 3:
                StoreManager.instance.storeItems[id].currentProgress = GameManager.instance.explosionUpgrade;
                break;
        }
    }

    public void Effect(int id)
    {
        switch(id)
        {
            case 0:
                GameObject roverInstance = Instantiate(PlayerManager.instance.roverPrefab, transform.position, Quaternion.identity);
                PlayerManager.instance.bc.TakeRover(roverInstance.transform.GetChild(0).gameObject);
                StoreManager.instance.storeItems[id].currentProgress = PlayerManager.instance.bc.storedRovers.Count + PlayerManager.instance.bc.activeRovers.Count;
                break;
            case 1:
                GameManager.instance.sonarUpgrade++;

                if(PlayerManager.instance.bc.activeRovers != null || PlayerManager.instance.bc.activeRovers.Count != 0)
                {
                    foreach (var rover in PlayerManager.instance.bc.activeRovers)
                        rover.transform.GetChild(0).GetComponent<AIController>().aiScan.transform.GetComponent<SphereCollider>().radius = 20 + (20 * GameManager.instance.sonarUpgrade);
                }

                if (PlayerManager.instance.bc.storedRovers != null || PlayerManager.instance.bc.storedRovers.Count != 0)
                {
                    foreach (var rover in PlayerManager.instance.bc.storedRovers)
                        rover.transform.GetChild(0).GetComponent<AIController>().aiScan.transform.GetComponent<SphereCollider>().radius = 20 + (20 * GameManager.instance.sonarUpgrade);
                }

                StoreManager.instance.storeItems[id].currentProgress = GameManager.instance.sonarUpgrade;
                break;
            case 2:
                GameManager.instance.expandUpgrade++;
                StoreManager.instance.storeItems[id].currentProgress = GameManager.instance.expandUpgrade;
                break;
            case 3:
                GameManager.instance.explosionUpgrade++;
                StoreManager.instance.storeItems[id].currentProgress = GameManager.instance.explosionUpgrade;
                break;
        }
    }
}
