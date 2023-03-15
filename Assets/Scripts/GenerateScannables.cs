using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GenerateScannables : MonoBehaviour
{
    public LayerMask ground;
    public GameObject seaweed;
    public int seaweedCount = 70;
    private GridGraph grid;

    void Start()
    {
        grid = AstarPath.active.data.gridGraph;

        if (seaweed == null)
            return;

        for (int i = 0; i < seaweedCount; i++)
        {
            GridNode randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
            float x = randomNode.RandomPointOnSurface().x / 2;
            float z = randomNode.RandomPointOnSurface().z / 2;
            float randomRot = Random.Range(0, 360);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z), Vector3.down, out hit, Mathf.Infinity, ground))
            {
                GameObject seaweedGO = Instantiate(seaweed, hit.point, Quaternion.LookRotation(hit.normal));
                seaweedGO.transform.localEulerAngles = new Vector3(seaweedGO.transform.localEulerAngles.x, seaweedGO.transform.localEulerAngles.y, seaweedGO.transform.localEulerAngles.z + randomRot);
                seaweedGO.transform.SetParent(transform);
                seaweedGO.name = "Seaweed";
            }
        }
    }
}
