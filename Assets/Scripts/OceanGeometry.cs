using System.Collections.Generic;
using UnityEngine;

public class OceanGeometry : MonoBehaviour
{
    private Transform viewer;
    [SerializeField]
    private GameObject water;
    private int count = 4;
    private float distanceBetween = 508.5f;
    public float offset = 100;

    private void Start()
    {
        viewer = Camera.main.transform;
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                Instantiate(water, new Vector3(transform.position.x + (i * distanceBetween) - (offset * count),
                    transform.position.y, transform.position.z + (j * distanceBetween) - (offset * count)), Quaternion.identity, transform);
            }
        }
    }

    private void Update()
    {
        if ((transform.position - new Vector3(viewer.position.x, transform.position.y, viewer.position.z)).sqrMagnitude >= 512 * 512)
            transform.position = new Vector3(viewer.position.x, transform.position.y, viewer.position.z);
    }
}