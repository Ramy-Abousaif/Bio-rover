using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScan : MonoBehaviour
{
    private SphereCollider sc;
    public GameObject scanner;

    private void Start()
    {
        sc = transform.GetComponent<SphereCollider>();
        sc.radius = 20 + (20 * GameManager.instance.sonarUpgrade);
    }

    void Scan()
    {
        if (scanner != null)
        {
            Instantiate(scanner, transform.position, Quaternion.identity);
            AudioManager.instance.PlayOneShotWithParameters("Sonar", transform, ("Underwater", (transform.position.y > WaveManager.instance.getHeight(transform.position.x, transform.position.z)) ? 0f : 1f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Scannable"))
        {
            if (other.GetComponent<Renderer>().materials[1].GetInt("_isHighlighted") == 0)
                Scan();
        }
    }
}
