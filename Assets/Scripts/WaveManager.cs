using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public Material water;

    Vector4 timescales;
    Vector3 wA, wB, wC, wD;
    float aA, aB, aC, aD;
    float gravity, phase, depth;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(this);
    }

    private void Update()
    {
        GetWaveData();
    }

    private void GetWaveData()
    {
        timescales = Shader.GetGlobalVector("_Time")[1] * water.GetVector("_Timescales");
        wA = water.GetVector("_Direction1");
        wB = water.GetVector("_Direction2");
        wC = water.GetVector("_Direction3");
        wD = water.GetVector("_Direction4");
        aA = water.GetFloat("_Amplitude1");
        aB = water.GetFloat("_Amplitude2");
        aC = water.GetFloat("_Amplitude3");
        aD = water.GetFloat("_Amplitude4");
        gravity = water.GetFloat("_Gravity");
        phase = water.GetFloat("_Phase");
        depth = water.GetFloat("_Depth");
    }

    public float getHeight(float x, float z)
    {
        Vector3 p = new Vector3(x, 0, z);
        float y = 0;

        y += GerstnerWave(wA, aA, p, timescales.x);
        y += GerstnerWave(wB, aB, p, timescales.y);
        y += GerstnerWave(wC, aC, p, timescales.z);
        y += GerstnerWave(wD, aD, p, timescales.w);

        return y;
    }

    float GerstnerWave(Vector3 wave, float amp, Vector3 point, float ts)
    {
        float frequency = Mathf.Sqrt((gravity * wave.magnitude) * (float)System.Math.Tanh(wave.magnitude * depth));
        float theta = ((wave.x * point.x + wave.z * point.z) - (frequency * ts)) - phase;
        return amp * Mathf.Cos(theta);
    }
}
