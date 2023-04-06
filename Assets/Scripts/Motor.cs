using System;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public ParticleSystem splash;
    public ParticleSystem foam;
    public GameObject sound;
    public float speed;
    public float startSpeed;
    public bool isSpinning = true;

    private void Start()
    {
        startSpeed = speed;
    }

    private void Update()
    {
        sound.SetActive(isSpinning);

        if (!isSpinning)
            return;

        transform.eulerAngles += Vector3.forward * speed * Time.deltaTime;
    }
}