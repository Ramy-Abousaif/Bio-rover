using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marimo : MonoBehaviour
{
    public LayerMask lm;
    private bool blocked = false;
    private float photosynthesis;
    private float rate = 2.0f;
    public float energy = 0;
    private Transform sunlight;

    // Start is called before the first frame update
    void Start()
    {
        sunlight = RenderSettings.sun.transform;
    }

    void Update()
    {
        photosynthesis = Vector3.Dot(transform.forward, -sunlight.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -sunlight.forward, out hit, Mathf.Infinity, lm))
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

        //Debug.DrawRay(transform.position, -sunlight.forward * 1000000000f, Color.yellow);

        if (photosynthesis > 0 && !blocked)
            energy += photosynthesis * (rate * Time.deltaTime);

        energy = Mathf.Clamp(energy, 0, 10);
    }
}
