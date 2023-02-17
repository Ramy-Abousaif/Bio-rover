using System;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public ParticleSystem splash;
    public ParticleSystem foam;
    public float speed;
    public float startSpeed;
    public bool isSpinning = true;

    private void Start()
    {
        this.startSpeed = this.speed;
    }

    private void Update()
    {
        if(isSpinning)
            base.transform.eulerAngles += Vector3.forward * this.speed * Time.deltaTime;
    }
}