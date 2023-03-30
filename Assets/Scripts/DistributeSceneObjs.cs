using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DistributeSceneObjs : MonoBehaviour
{
    public LayerMask ground;
    public GameObject seaweed;
    public GameObject rock;
    public GameObject diamond;
    public GameObject barrel;
    public GameObject seaMine;
    private int seaweedCount = 200;
    private int rockCount = 40;
    private int diamondCount = 10;
    private int barrelCount = 15;
    private int seaMineCount = 15;
    private GridGraph grid;

    void Start()
    {
        grid = AstarPath.active.data.gridGraph;

        DistributeObjs(seaweed, 0f, 0f, seaweedCount, "Seaweed", true, true, false);
        DistributeObjs(rock, 0f, 0f, rockCount, "Rock", true, true, false);
        DistributeObjs(diamond, 0f, 0f, diamondCount, "Diamond", true, true, false);
        DistributeObjs(barrel, 1.5f, 1.5f, barrelCount, "Radioactive Barrel", true, true, false);
        DistributeObjs(seaMine, 40f, 5f, seaMineCount, "Sea Mine", false, false, true);
    }

    void DistributeObjs(GameObject prefab, float defaultOffset, float lowestOffset, int count, string name, bool useNormal, bool useRandomRot, bool randomOffset)
    {
        if (prefab == null)
            return;

        for (int i = 0; i < count; i++)
        {
            GridNode randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
            float x = randomNode.RandomPointOnSurface().x / 2;
            float z = randomNode.RandomPointOnSurface().z / 2;
            float randomRot = Random.Range(0, 360);
            float newOffset = defaultOffset;

            if (randomOffset)
                newOffset = Random.Range(lowestOffset, defaultOffset);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z), Vector3.down, out hit, Mathf.Infinity, ground, QueryTriggerInteraction.Ignore))
            {
                GameObject goInstance = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + newOffset, hit.point.z), useNormal ? Quaternion.LookRotation(hit.normal) : Quaternion.Euler(Vector3.zero));

                if(useRandomRot)
                    goInstance.transform.localEulerAngles = new Vector3(goInstance.transform.localEulerAngles.x, goInstance.transform.localEulerAngles.y, goInstance.transform.localEulerAngles.z + randomRot);

                goInstance.transform.SetParent(transform);
                goInstance.name = name;
                goInstance.isStatic = true;
            }
        }
    }
}
