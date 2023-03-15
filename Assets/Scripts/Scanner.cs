using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scanner : MonoBehaviour
{
    public Color[] rarityColors;
    public float speed = 5.0f;
    private float duration = 1.0f;
    private float fadeDuration = 1.0f;
    private float timeElapsed = 0.0f;
    private Material mat;

    void Start()
    {
        mat = this.gameObject.GetComponent<Renderer>().material;
        Destroy(gameObject, (duration + GameManager.instance.sonarUpgrade));
    }

    void Update()
    {
        float expand = speed * Time.deltaTime;
        transform.localScale = new Vector3(transform.localScale.x + expand, transform.localScale.y + expand, transform.localScale.z + expand);

        if(timeElapsed > (duration + GameManager.instance.sonarUpgrade) - fadeDuration)
        {
            mat.SetFloat("_Alpha", fadeDuration);
            fadeDuration -= Time.deltaTime;
        }

        timeElapsed += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Scannable"))
        {
            Scannable scannedObj = other.gameObject.GetComponent<Scannable>();
            if(!scannedObj.scanned)
            {
                other.GetComponent<Scannable>().scanned = true;
                if (MissionsManager.instance.GetMissionByName("Scan objects").GetObjectiveByTitle("Scan " + scannedObj.rarity.ToString().ToLower() + " objects") != null && 
                    !MissionsManager.instance.GetMissionByName("Scan objects").GetObjectiveByTitle("Scan " + scannedObj.rarity.ToString().ToLower() + " objects").completed)
                {
                    MissionsManager.instance.GetMissionByName("Scan objects").GetObjectiveByTitle("Scan " + scannedObj.rarity.ToString().ToLower() + " objects").currentProgress++;
                }

                MissionsManager.instance.GetMissionByName("Scan objects").CheckCompletion();
                Reward(other.gameObject);
            }

            other.GetComponent<Renderer>().materials[1].SetInt("_isHighlighted", 1);
        }
    }

    void Reward(GameObject other)
    {
        GameObject text = Instantiate(UIManager.instance.scanTextPrefab, UIManager.instance.scanTextPos.position, Quaternion.identity, UIManager.instance.inGame.transform);
        text.layer = LayerMask.NameToLayer("UI");
        text.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("UI");
        text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "+ " + other.transform.name + "scanned";
        switch (other.GetComponent<Scannable>().rarity)
        {
            case ScanRarity.COMMON:
                //Play Audio

                text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = rarityColors[0];
                StoreManager.instance.AddCredit(100);
                break;
            case ScanRarity.UNCOMMON:
                //Play Audio

                text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = rarityColors[1];
                StoreManager.instance.AddCredit(200);
                break;
            case ScanRarity.RARE:
                //Play Audio

                text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = rarityColors[2];
                StoreManager.instance.AddCredit(500);
                break;
            case ScanRarity.EXOTIC:
                //Play Audio

                text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = rarityColors[3];
                StoreManager.instance.AddCredit(1000);
                break;
        }
    }
}
