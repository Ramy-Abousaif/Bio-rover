using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scanner : MonoBehaviour
{
    public Color[] rarityColors;
    public float speed = 5.0f;
    [HideInInspector]
    public float duration = 1.0f;
    private float fadeDuration = 1.0f;
    private float timeElapsed = 0.0f;
    private Material matLOD0;
    private Material matLOD1;
    private Material matLOD2;
    private Vector3 localScale;

    private void Awake()
    {
        matLOD0 = this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material;
        matLOD1 = this.gameObject.transform.GetChild(1).GetComponent<Renderer>().material;
        matLOD2 = this.gameObject.transform.GetChild(2).GetComponent<Renderer>().material;
    }

    void Update()
    {
        float expand = speed * Time.deltaTime;
        transform.localScale = new Vector3(transform.localScale.x + expand, transform.localScale.y + expand, transform.localScale.z + expand);

        if(timeElapsed > (duration + GameManager.instance.sonarUpgrade) - fadeDuration)
        {
            matLOD0.SetFloat("_Alpha", fadeDuration);
            matLOD1.SetFloat("_Alpha", fadeDuration);
            matLOD2.SetFloat("_Alpha", fadeDuration);
            fadeDuration -= Time.deltaTime;
        }

        timeElapsed += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Scannable"))
        {
            Scannable scannedObj = other.gameObject.GetComponent<Scannable>();
            if(!scannedObj.scanned)
            {
                scannedObj.scanned = true;
                // Rarity scan mission
                if (MissionsManager.instance.GetMissionByName("Scan objects").GetObjectiveByTitle("Scan " + scannedObj.rarity.ToString().ToLower() + " objects") != null &&
                    !MissionsManager.instance.GetMissionByName("Scan objects").GetObjectiveByTitle("Scan " + scannedObj.rarity.ToString().ToLower() + " objects").completed)
                {
                    MissionsManager.instance.GetMissionByName("Scan objects").GetObjectiveByTitle("Scan " + scannedObj.rarity.ToString().ToLower() + " objects").currentProgress++;
                }

                // Scan radioactive barrels mission
                if (MissionsManager.instance.GetMissionByName("Scan for radioactive materials").GetObjectiveByTitle("Scan leaking " + scannedObj.name.ToLower() + "s") != null &&
                    !MissionsManager.instance.GetMissionByName("Scan for radioactive materials").GetObjectiveByTitle("Scan leaking " + scannedObj.name.ToLower() + "s").completed)
                {
                    MissionsManager.instance.GetMissionByName("Scan for radioactive materials").GetObjectiveByTitle("Scan leaking " + scannedObj.name.ToLower() + "s").currentProgress++;
                }

                MissionsManager.instance.GetMissionByName("Scan objects").CheckCompletion();
                MissionsManager.instance.GetMissionByName("Scan for radioactive materials").CheckCompletion();
                Reward(scannedObj);
            }

            if(other.GetComponent<Renderer>() != null)
            {
                foreach (Material mat in other.GetComponent<Renderer>().materials)
                {
                    if (mat.GetInt("_isScannable") == 1)
                        mat.SetInt("_isHighlighted", 1);
                }
            }
        }
    }

    void Reward(Scannable other)
    {
        GameObject text;
        PoolManager.instance.SpawnScanText(UIManager.instance.scanTextPos.position, Quaternion.identity, UIManager.instance.inGame.transform, out text);
        text.layer = LayerMask.NameToLayer("UI");
        text.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("UI");
        text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "+ " + other.transform.name + " scanned";
        switch (other.rarity)
        {
            case Rarity.COMMON:
                AudioManager.instance.PlayOneShotWithParameters("RewardCommon", transform);
                text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = rarityColors[0];
                break;
            case Rarity.UNCOMMON:
                AudioManager.instance.PlayOneShotWithParameters("RewardUncommon", transform);
                text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = rarityColors[1];
                break;
            case Rarity.RARE:
                AudioManager.instance.PlayOneShotWithParameters("RewardRare", transform);
                text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = rarityColors[2];
                break;
            case Rarity.EXOTIC:
                AudioManager.instance.PlayOneShotWithParameters("RewardExotic", transform);
                text.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = rarityColors[3];
                break;
        }
        StoreManager.instance.AddCredit((int)(other.baseValue * ((float)other.rarity / 2)));
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        fadeDuration = 1.0f;
        timeElapsed = 0.0f;
        matLOD0.SetFloat("_Alpha", fadeDuration);
        matLOD1.SetFloat("_Alpha", fadeDuration);
        matLOD2.SetFloat("_Alpha", fadeDuration);
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.zero;
        fadeDuration = 1.0f;
        timeElapsed = 0.0f;
        matLOD0.SetFloat("_Alpha", fadeDuration);
        matLOD1.SetFloat("_Alpha", fadeDuration);
        matLOD2.SetFloat("_Alpha", fadeDuration);
    }

    private void OnDestroy()
    {
        transform.localScale = Vector3.zero;
        fadeDuration = 1.0f;
        timeElapsed = 0.0f;
        matLOD0.SetFloat("_Alpha", fadeDuration);
        matLOD1.SetFloat("_Alpha", fadeDuration);
        matLOD2.SetFloat("_Alpha", fadeDuration);
    }
}
