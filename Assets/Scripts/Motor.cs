using System;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public float speed;
    public float startSpeed;

    private void Start()
    {
        this.startSpeed = this.speed;
    }

    private void Update()
    {
        base.transform.eulerAngles += Vector3.forward * this.speed * Time.deltaTime;
    }
}