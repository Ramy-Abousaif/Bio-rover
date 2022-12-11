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
    }
}
