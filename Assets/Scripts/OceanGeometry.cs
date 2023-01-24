using System.Collections.Generic;
using UnityEngine;

public class OceanGeometry : MonoBehaviour
{
    [SerializeField]
    private Transform viewer;
    [SerializeField]
    private GameObject water;
    private int count = 3;

    private void Start()
    {
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                Instantiate(water, new Vector3(transform.position.x + (i * 512) - 512, transform.position.y, transform.position.z + (j * 512) - 512), Quaternion.identity, transform);
            }
        }
    }

    private void Update()
    {
        transform.position = new Vector3(viewer.position.x, transform.position.y, viewer.position.z);
    }
}