using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private Camera cam;
    public GameObject rover;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.position = rover.transform.position;
        transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, 0);

        Debug.Log((cam.transform.position.y < WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? "Camera is underwater" : "Camera is not underwater");
    }
}
