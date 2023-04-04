using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/*
 * UPDATE WITH EVERY CHANGE!
 * Current Rarity List:
 * Name                 Rarity                  Count               Actual Count                Rarity Actual Count:
 * Acropora             Rare                    15                  30                          Common: 336 = 336
 * Barnacles            Uncommon                12                  36                          Uncommon: 162 = 3240
 * Bone Coral           Uncommon                10                  30                          Rare: 72 = 4320
 * Branch Coral 1       Uncommon                8                   24                          Exotic: 20 = 7500
 * Branch Coral 2       Rare                    6                   12
 * Diamond              Exotic                  10                  10
 * Feather Bush         Rare                    7                   14
 * Flower               Exotic                  10                  10
 * Foliage              Common                  20                  80
 * Leaves               Common                  30                  120
 * Radioactive Waste    Rare                    8                   16
 * Rock                 Common                  14                  56
 * Seaweed              Common                  20                  80
 * Tube Corals          Uncommon                10                  30
 * Vase 1               Uncommon                8                   24
 * Vase 2               Uncommon                6                   18
 */

public class DistributeSceneObjs : MonoBehaviour
{
    public LayerMask ground;
    public Gradient overrideColor;
    public GameObject seaweed;
    public GameObject leaves;
    public GameObject foliage;
    public GameObject rock;
    public GameObject diamond;
    public GameObject barrel;
    public GameObject seaMine;
    public GameObject acropora;
    public GameObject barnacles;
    public GameObject boneCoral;
    public GameObject branchCoral1;
    public GameObject branchCoral2;
    public GameObject featherBush;
    public GameObject flower;
    public GameObject tubeCorals;
    public GameObject vase1;
    public GameObject vase2;
    private int rarityMulti = 1;
    private int seaweedCount = 20;
    private int leavesCount = 30;
    private int foliageCount = 20;
    private int rockCount = 14;
    private int diamondCount = 10;
    private int barrelCount =  8;
    private int acroporaCount = 15;
    private int barnaclesCount = 12;
    private int boneCoralCount = 10;
    private int branchCoral1Count = 8;
    private int branchCoral2Count = 6;
    private int featherBushCount = 7;
    private int flowerCount = 10;
    private int tubeCoralsCount = 10;
    private int vase1Count = 8;
    private int vase2Count = 6;
    private int seaMineCount = 15;
    private GridGraph grid;

    void Start()
    {
        grid = AstarPath.active.data.gridGraph;

        DistributeObjs(seaweed, 0f, 0f, seaweedCount, "Seaweed", true, true, false, false);
        DistributeObjs(leaves, 0f, 0f, leavesCount, "Grass", true, true, false, false);
        DistributeObjs(foliage, 0f, 0f, foliageCount, "Seaweed", true, true, false, false);
        DistributeObjs(rock, 0f, 0f, rockCount, "Rock", true, true, false, false);
        DistributeObjs(diamond, 0f, 0f, diamondCount, "Diamond", true, true, false, false);
        DistributeObjs(acropora, 0f, 0f, acroporaCount, "Acropora", true, true, false, true);
        DistributeObjs(barnacles, 0.5f, 0.5f, barnaclesCount, "Barnacles", true, true, false, true);
        DistributeObjs(boneCoral, 0f, 0f, boneCoralCount, "Bone Coral", true, true, false, true);
        DistributeObjs(branchCoral1, 0f, 0f, branchCoral1Count, "Small Branch Coral", true, true, false, true);
        DistributeObjs(branchCoral2, 0f, 0f, branchCoral2Count, "Large Branch Coral", true, true, false, true);
        DistributeObjs(featherBush, 0f, 0f, featherBushCount, "Feather Bush", true, true, false, true);
        DistributeObjs(flower, 0f, 0f, flowerCount, "Flower", true, true, false, true);
        DistributeObjs(tubeCorals, 0f, 0f, tubeCoralsCount, "Tube Corals", true, true, false, true);
        DistributeObjs(vase1, 0f, 0f, vase1Count, "Small Sea Vase", true, true, false, true);
        DistributeObjs(vase2, 0f, 0f, vase2Count, "Large Sea Vase", true, true, false, true);
        DistributeObjs(barrel, 0f, 0f, barrelCount, "Radioactive Barrel", true, true, false, false);
        DistributeObjs(seaMine, 40f, 5f, seaMineCount, "Sea Mine", false, false, true, false);
    }

    void DistributeObjs(GameObject prefab, float defaultOffset, float lowestOffset, int baseCount, string name, bool useNormal, bool useRandomRot, bool randomOffset, bool isOverrideColor)
    {
        if (prefab == null)
            return;

        if (prefab.GetComponent<Scannable>() != null)
        {
            switch(prefab.GetComponent<Scannable>().rarity)
            {
                case Rarity.COMMON:
                    rarityMulti = 4;
                    break;
                case Rarity.UNCOMMON:
                    rarityMulti = 3;
                    break;
                case Rarity.RARE:
                    rarityMulti = 2;
                    break;
                case Rarity.EXOTIC:
                    rarityMulti = 1;
                    break;
                default:
                    break;
            }
        }

        for (int i = 0; i < baseCount * rarityMulti; i++)
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
                GameObject goInstance = Instantiate(prefab, new Vector3(hit.point.x, hit.point.y + newOffset, hit.point.z), prefab.transform.rotation);

                if (useNormal)
                    goInstance.transform.up = hit.normal;
                else
                    goInstance.transform.rotation = prefab.transform.rotation;

                if(isOverrideColor)
                {
                    Color colorPicked = overrideColor.Evaluate(Random.Range(0f, 1f));
                    Material[] mats = goInstance.GetComponent<Renderer>().materials;
                    foreach (var mat in mats)
                    {
                        mat.SetVector("_TintColor1", colorPicked);
                        mat.SetVector("_TintColor2", colorPicked);
                    }
                }

                //if (useRandomRot)
                    //goInstance.transform.localEulerAngles = new Vector3(goInstance.transform.localEulerAngles.x, goInstance.transform.localEulerAngles.y, goInstance.transform.localEulerAngles.z + randomRot);

                goInstance.transform.SetParent(transform);
                goInstance.name = name;
                goInstance.isStatic = true;
            }
        }
    }
}
