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

        if (rock1 == null)
            return;

        for (int i = 0; i < rock1Count; i++)
        {
            GridNode randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
            float x = randomNode.RandomPointOnSurface().x / 2;
            float z = randomNode.RandomPointOnSurface().z / 2;
            float randomRot = Random.Range(0, 360);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z), Vector3.down, out hit, Mathf.Infinity, ground))
            {
                GameObject rock1GO = Instantiate(rock1, hit.point, Quaternion.LookRotation(hit.normal));
                rock1GO.transform.localEulerAngles = new Vector3(rock1GO.transform.localEulerAngles.x, rock1GO.transform.localEulerAngles.y, rock1GO.transform.localEulerAngles.z + randomRot);
                rock1GO.transform.SetParent(transform);
                rock1GO.name = "Rock";
            }
        }

        if (rock2 == null)
            return;

        for (int i = 0; i < rock2Count; i++)
        {
            GridNode randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
            float x = randomNode.RandomPointOnSurface().x / 2;
            float z = randomNode.RandomPointOnSurface().z / 2;
            float randomRot = Random.Range(0, 360);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z), Vector3.down, out hit, Mathf.Infinity, ground))
            {
                GameObject rock2GO = Instantiate(rock2, hit.point, Quaternion.LookRotation(hit.normal));
                rock2GO.transform.localEulerAngles = new Vector3(rock2GO.transform.localEulerAngles.x, rock2GO.transform.localEulerAngles.y, rock2GO.transform.localEulerAngles.z + randomRot);
                rock2GO.transform.SetParent(transform);
                rock2GO.name = "Rock";
            }
        }
    }
}
