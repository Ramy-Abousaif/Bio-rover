using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marimo : MonoBehaviour
{
    public Gradient debugColor;
    public GameObject relFace;
    private Material relMat;
    public LayerMask lm;
    private bool blocked = false;
    private float photosynthesis;
    private float rateGain = 20.0f;
    //private float rateLoss = 5.0f;
    public float energy = 0;
    private float maxEnergy = 100.0f;
    private Transform sunlight;
    public bool showUI = true;

    // Start is called before the first frame update
    void Start()
    {
        sunlight = RenderSettings.sun.transform;
        if (showUI)
            relMat = relFace.GetComponent<Renderer>().material;

        StartCoroutine(Photosynthesis());
    }

    IEnumerator Photosynthesis()
    {
        while(true)
        {
            photosynthesis = Vector3.Dot(transform.forward, -sunlight.forward);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -sunlight.forward, out hit, Mathf.Infinity, lm, QueryTriggerInteraction.Ignore))
            {
                blocked = true;
                /*
                if (photosynthesis >= 0 && photosynthesis <= 0.5f)
                    Debug.Log(gameObject.name + " is facing towards the sun at a low intensity and is currently blocked by " + hit.transform.name + ". Energy = " + energy);
                else if (photosynthesis > 0.5f)
                    Debug.Log(gameObject.name + " is facing towards the sun at a high intensity and is currently blocked by " + hit.transform.name + ". Energy = " + energy);
                */
            }
            else
            {
                blocked = false;
                /*
                if (photosynthesis >= 0 && photosynthesis <= 0.5f)
                    Debug.Log(gameObject.name + " is facing towards the sun at a low intensity and is currently not blocked" + ". Energy = " + energy);
                else if (photosynthesis > 0.5f)
                    Debug.Log(gameObject.name + " is facing towards the sun at a high intensity and is currently not blocked" + ". Energy = " + energy);
                */
            }
            float randomNumber = Random.Range(0f, 0.2f);
            yield return new WaitForSeconds(0.2f + randomNumber);
        }

        yield return null;
    }

    void Update()
    {
        if (photosynthesis > 0 && !blocked)
            energy += photosynthesis * (rateGain * Time.deltaTime);

        //energy -= rateLoss * Time.deltaTime;

        energy = Mathf.Clamp(energy, 0, maxEnergy);

        //Debug.DrawRay(transform.position, -sunlight.forward * 1000000000f, Color.yellow);
        if (showUI)
            relMat.SetVector("_Color", debugColor.Evaluate(energy / maxEnergy) * 3);
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor.Evaluate(energy / maxEnergy);
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
    */
}
