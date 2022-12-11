using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour
{
    public float speed = 5.0f;
    public float duration = 5.0f;
    private float fadeDuration = 1.0f;
    private float timeElapsed = 0.0f;
    private Material mat;

    void Start()
    {
        mat = this.gameObject.GetComponent<Renderer>().material;
        Destroy(gameObject, duration);
    }

    void Update()
    {
        float expand = speed * Time.deltaTime;
        transform.localScale = new Vector3(transform.localScale.x + expand, transform.localScale.y + expand, transform.localScale.z + expand);

        if(timeElapsed > duration - fadeDuration)
        {
            mat.SetFloat("_Alpha", fadeDuration);
            fadeDuration -= Time.deltaTime;
        }

        timeElapsed += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        //Highlight 
    }
}
