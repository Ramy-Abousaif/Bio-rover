using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyVFX : MonoBehaviour
{
    public float duration = 1f;

    void Start()
    {
        StartCoroutine(DestroyAfterEffect());
    }

    IEnumerator DestroyAfterEffect()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
