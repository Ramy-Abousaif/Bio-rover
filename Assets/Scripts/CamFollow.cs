using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private GameObject boat;
    public GameObject waterLens;

    private void Start()
    {
        boat = PlayerManager.instance.boat;
    }

    void Update()
    {
        if ((Camera.main.transform.position.y <= WaveManager.instance.getHeight(transform.position.x, transform.position.z)))
        {
            waterLens.SetActive(true);
            RenderSettings.fog = false;
        }
        else
        {
            waterLens.SetActive(false);
            RenderSettings.fog = true;
        }

        switch (PlayerManager.instance.playerState)
        {
            case PlayerState.BOAT:
                transform.position = boat.transform.position;
                break;
            case PlayerState.ROVER:
                transform.position = PlayerManager.instance.currentRover.transform.position;
                PlayerManager.instance.rc.followCam = transform;
                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
                break;
            default:
                break;
        }
    }
}
