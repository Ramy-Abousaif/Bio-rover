using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverMinimap : MonoBehaviour
{
    public GameObject roverMM;

    void Update()
    {
        roverMM.transform.position = new Vector3(transform.position.x, 45, transform.position.z);
    }
}
