using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private Camera cam;
    public GameObject boat;
    public GameObject rover;
    public GameObject waterLens;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        switch(PlayerManager.instance.playerState)
        {
            case PlayerState.BOAT:
                transform.position = boat.transform.position;
                transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, 0);

                waterLens.SetActive(false);
                break;
            case PlayerState.ROVER:
                transform.position = rover.transform.position;
                transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, 0);

                if ((cam.transform.position.y <= WaveManager.instance.getHeight(transform.position.x, transform.position.z)))
                    waterLens.SetActive(true);
                else
                    waterLens.SetActive(false);
                break;
            default:
                break;
        }
    }
}
