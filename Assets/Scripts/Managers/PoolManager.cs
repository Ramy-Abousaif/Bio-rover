using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance { get; private set; }

    [SerializeField] private GameObject scannerPrefab;
    [HideInInspector] public ObjectPool<GameObject> scannerPool;

    [SerializeField] private GameObject smokeRingPrefab;
    [HideInInspector] public ObjectPool<GameObject> smokeRingPool;

    [SerializeField] private GameObject explosionPrefab;
    [HideInInspector] public ObjectPool<GameObject> explosionPool;

    [SerializeField] private GameObject poofPrefab;
    [HideInInspector] public ObjectPool<GameObject> poofPool;

    [SerializeField] private GameObject scanTextPrefab;
    [HideInInspector] public ObjectPool<GameObject> scanTextPool;

    [SerializeField] private GameObject creditTextPrefab;
    [HideInInspector] public ObjectPool<GameObject> creditTextPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        scannerPool = CreateObjectPool(scannerPrefab, 300, 500);
        smokeRingPool = CreateObjectPool(smokeRingPrefab, 120, 250);
        explosionPool = CreateObjectPool(explosionPrefab, 70, 100);
        poofPool = CreateObjectPool(poofPrefab, 70, 100);
        scanTextPool = CreateObjectPool(scanTextPrefab, 200, 500);
        creditTextPool = CreateObjectPool(creditTextPrefab, 200, 500);
    }

    private ObjectPool<GameObject> CreateObjectPool(GameObject prefab, int initialCapacity, int maxCapacity)
    {
        var pool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(prefab);
        }, obj =>
        {
            obj.SetActive(true);
        }, obj =>
        {
            obj.SetActive(false);
        }, obj =>
        {
            Destroy(obj);
        }, false, initialCapacity, maxCapacity);

        return pool;
    }

    public void SpawnScanner(Vector3 pos, Quaternion rot)
    {
        var scanner = scannerPool.Get();
        scanner.transform.position = pos;
        StartCoroutine(DestroyScanner(scanner, scanner.GetComponent<Scanner>().duration + GameManager.instance.sonarUpgrade));
    }

    IEnumerator DestroyScanner(GameObject scanner, float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);
        scannerPool.Release(scanner);
        yield return null;
    }

    public void SpawnSmokeRing(Vector3 pos, Quaternion rot)
    {
        var smokeRing = smokeRingPool.Get();
        smokeRing.transform.position = pos;
        StartCoroutine(DestroySmokeRing(smokeRing, 2.0f));
    }

    IEnumerator DestroySmokeRing(GameObject smokeRing, float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);
        smokeRingPool.Release(smokeRing);
        yield return null;
    }

    public void SpawnExplosion(Vector3 pos, Quaternion rot)
    {
        var explosion = explosionPool.Get();
        explosion.transform.position = pos;
        StartCoroutine(DestroyExplosion(explosion, 3.0f));
    }

    IEnumerator DestroyExplosion(GameObject explosion, float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);
        explosionPool.Release(explosion);
        yield return null;
    }

    public void SpawnPoof(Vector3 pos, Quaternion rot)
    {
        var poof = poofPool.Get();
        poof.transform.position = pos;
        StartCoroutine(DestroyPoof(poof, 2.0f));
    }

    IEnumerator DestroyPoof(GameObject poof, float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);
        poofPool.Release(poof);
        yield return null;
    }

    public void SpawnScanText(Vector3 pos, Quaternion rot, Transform parent, out GameObject text)
    {
        var scanText = scanTextPool.Get();
        scanText.transform.SetParent(parent);
        scanText.transform.position = pos;
        text = scanText;
        StartCoroutine(DestroyScanText(scanText, 1.7f));
    }

    IEnumerator DestroyScanText(GameObject scanText, float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);
        scanTextPool.Release(scanText);
        yield return null;
    }

    public void SpawnCreditText(Vector3 pos, Quaternion rot, Transform parent, out GameObject text)
    {
        var creditText = creditTextPool.Get();
        creditText.transform.SetParent(parent);
        creditText.transform.position = pos;
        text = creditText;
        StartCoroutine(DestroyCreditText(creditText, 1.7f));
    }

    IEnumerator DestroyCreditText(GameObject creditText, float timeLimit)
    {
        yield return new WaitForSeconds(timeLimit);
        creditTextPool.Release(creditText);
        yield return null;
    }
}
