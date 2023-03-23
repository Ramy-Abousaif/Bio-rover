using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DistributeSceneObjs : MonoBehaviour
{
    public LayerMask ground;
    public GameObject seaweed;
    public GameObject rock1;
    public GameObject rock2;
    private int seaweedCount = 70;
    private int rock1Count = 70;
    private int rock2Count = 70;
    private GridGraph grid;

    void Start()
    {
        grid = AstarPath.active.data.gridGraph;

        DistributeObjs(seaweed, seaweedCount, "Seaweed");
        DistributeObjs(rock1, rock1Count, "Rock");
        DistributeObjs(rock2, rock2Count, "Rock");
    }

    void DistributeObjs(GameObject prefab, int count, string name)
    {
        if (prefab == null)
            return;

        for (int i = 0; i < count; i++)
        {
            GridNode randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
            float x = randomNode.RandomPointOnSurface().x / 2;
            float z = randomNode.RandomPointOnSurface().z / 2;
            float randomRot = Random.Range(0, 360);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z), Vector3.down, out hit, Mathf.Infinity, ground, QueryTriggerInteraction.Ignore))
            {
                GameObject goInstance = Instantiate(rock2, hit.point, Quaternion.LookRotation(hit.normal));
                goInstance.transform.localEulerAngles = new Vector3(goInstance.transform.localEulerAngles.x, goInstance.transform.localEulerAngles.y, goInstance.transform.localEulerAngles.z + randomRot);
                goInstance.transform.SetParent(transform);
                goInstance.name = name;
                goInstance.isStatic = true;
            }
        }
    }
}
