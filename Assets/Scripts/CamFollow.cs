using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public GameObject boat;
    public GameObject rover;
    public GameObject waterLens;

    void Update()
    {
        if ((Camera.main.transform.position.y <= WaveManager.instance.getHeight(transform.position.x, transform.position.z)))
            waterLens.SetActive(true);
        else
            waterLens.SetActive(false);

        switch (PlayerManager.instance.playerState)
        {
            case PlayerState.BOAT:
                transform.position = boat.transform.position;
                break;
            case PlayerState.ROVER:
                transform.position = rover.transform.position;
                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
                break;
            default:
                break;
        }
    }
}
