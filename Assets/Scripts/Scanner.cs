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
            if(!other.GetComponent<Scannable>().scanned)
            {
                if (MissionsManager.instance.GetMissionByName("Scan All Plants").GetObjectiveByTitle("Scan All Plants").currentProgress <
                        MissionsManager.instance.GetMissionByName("Scan All Plants").GetObjectiveByTitle("Scan All Plants").targetProgress)
                {
                    MissionsManager.instance.GetMissionByName("Scan All Plants").GetObjectiveByTitle("Scan All Plants").currentProgress++;
                }

                MissionsManager.instance.GetMissionByName("Scan All Plants").CheckCompletion();

                ShowFloatingText(other.gameObject);
                StoreManager.instance.AddCredit(10);
            }

            other.GetComponent<Renderer>().materials[1].SetInt("_isHighlighted", 1);
        }
    }

    void ShowFloatingText(GameObject other)
    {
        //Play Audio

        GameObject text = Instantiate(UIManager.instance.floatingTextPrefab, UIManager.instance.floatTextPos.position, Quaternion.identity, UIManager.instance.inGame.transform);
        text.layer = LayerMask.NameToLayer("UI");
        text.GetComponent<TMP_Text>().text = "+ " + other.transform.name + "scanned";
        text.GetComponent<TMP_Text>().color = rarityColors[(((int)other.GetComponent<Scannable>().rarity))];
    }
}
