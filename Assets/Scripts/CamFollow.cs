using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public GameObject boat;
    public GameObject rover;

    void Update()
    {
        switch(PlayerManager.instance.playerState)
        {
            case PlayerState.BOAT:
                transform.position = boat.transform.position;
                break;
            case PlayerState.ROVER:
                transform.position = rover.transform.position;
                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
                if ((Camera.main.transform.position.y <= WaveManager.instance.getHeight(transform.position.x, transform.position.z)))
                    PlayerManager.instance.waterLens.SetActive(true);
                else
                    PlayerManager.instance.waterLens.SetActive(false);
                break;
            default:
                break;
        }
    }
}
