using System.Collections.Generic;
using UnityEngine;

public class OceanGeometry : MonoBehaviour
{
    private Transform viewer;
    [SerializeField]
    private GameObject water;
    private int count = 4;

    private void Start()
    {
        viewer = Camera.main.transform;
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                Instantiate(water, new Vector3(transform.position.x + (i * 512) - (192 * count), transform.position.y, transform.position.z + (j * 512) - (192 * count)), Quaternion.identity, transform);
            }
        }
    }

    private void Update()
    {
        transform.position = new Vector3(viewer.position.x, transform.position.y, viewer.position.z);
    }
}