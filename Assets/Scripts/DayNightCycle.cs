using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;

    [Header("Time")]
    public float cycleInMinutes = 1;


    [Header("Sun")]
    public Transform sun1;
    public AnimationCurve sunBrightness = new AnimationCurve(
    new Keyframe(0, 0.0033333333333333f),
    new Keyframe(0.15f, 0.0033333333333333f),
    new Keyframe(0.35f, 0.33333333333333f),
    new Keyframe(0.65f, 0.33333333333333f),
    new Keyframe(0.85f, 0.0033333333333333f),
    new Keyframe(1, 0.0033333333333333f)
    );
    public Gradient sunColor = new Gradient()
    {
        colorKeys = new GradientColorKey[3]{
        new GradientColorKey(new Color(1, 0.75f, 0.3f), 0),
        new GradientColorKey(new Color(0.95f, 0.95f, 1), 0.5f),
        new GradientColorKey(new Color(1, 0.75f, 0.3f), 1),
        },
        alphaKeys = new GradientAlphaKey[2]{
        new GradientAlphaKey(1, 0),
        new GradientAlphaKey(1, 1)
        }
    };

    [Header("Moon")]
    public Transform moon1;
    public AnimationCurve moonBrightness = new AnimationCurve(
    new Keyframe(0, 0.033333333333333f),
    new Keyframe(0.15f, 0.033333333333333f),
    new Keyframe(0.35f, 0.00033333333333333f),
    new Keyframe(0.65f, 0.00033333333333333f),
    new Keyframe(0.85f, 0.033333333333333f),
    new Keyframe(1, 0.033333333333333f)
    );

    [Header("Sky")]
    [GradientUsage(true)]
    public Gradient skyColorDay = new Gradient()
    {
        colorKeys = new GradientColorKey[3]{
        new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 0),
        new GradientColorKey(new Color(0.7f, 1.4f, 3), 0.5f),
        new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 1),
        },
        alphaKeys = new GradientAlphaKey[2]{
        new GradientAlphaKey(1, 0),
        new GradientAlphaKey(1, 1)
        }
    };

    [GradientUsage(true)]
    public Gradient skyColorNight = new Gradient()
    {
        colorKeys = new GradientColorKey[3]{
        new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 0),
        new GradientColorKey(new Color(0.44f, 1, 1), 0.5f),
        new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 1),
        },
        alphaKeys = new GradientAlphaKey[2]{
        new GradientAlphaKey(1, 0),
        new GradientAlphaKey(1, 1)
        }
    };

    [Header("Fog")]
    public Gradient fogColor = new Gradient()
    {
        colorKeys = new GradientColorKey[5]{
        new GradientColorKey(new Color(0.66f, 1, 1), 0),
        new GradientColorKey(new Color(0.88f, 0.62f, 0.43f), 0.25f),
        new GradientColorKey(new Color(0.88f, 0.88f, 1), 0.5f),
        new GradientColorKey(new Color(0.88f, 0.62f, 0.43f), 0.75f),
        new GradientColorKey(new Color(0.66f, 1, 1), 1),
        },
        alphaKeys = new GradientAlphaKey[2]{
        new GradientAlphaKey(1, 0),
        new GradientAlphaKey(1, 1)
        }
    };


    private Light sunLight1;
    private float sunAngle;
    private Light moonLight1;
    private float moonAngle;
    private float accelTime = 1f;

    void Awake()
    {
        if (DayNightCycle.instance == null) instance = this;
        else Debug.Log("Warning; Multiples instances found of {0}, only one instance of {0} allowed.", this);
    }

    void Start()
    {
        sun1.rotation = Quaternion.Euler(0, 0, 0);
        moon1.rotation = Quaternion.Euler(180, 0, 0);
        sunLight1 = sun1.GetComponent<Light>();
        moonLight1 = moon1.GetComponent<Light>();
    }

    void Update()
    {
        UpdateSunAngle();
        UpdateMoonAngle();

        if (Application.isPlaying)
        {
            RotateSun();
            RotateMoon();
        }

        SetSunBrightness();
        SetMoonBrightness();
        SetSunColor();
        SetSkyColor();
        SetFogColor();
    }


    void RotateSun()
    {
        // Rotate 360 degrees every cycleInMinutes minutes.
        sun1.Rotate(Vector3.right * Time.deltaTime * 6 * accelTime / cycleInMinutes);
    }

    void RotateMoon()
    {
        // Rotate 360 degrees every cycleInMinutes minutes.
        moon1.Rotate(Vector3.right * Time.deltaTime * 6 * accelTime / cycleInMinutes);
    }

    void SetSunBrightness()
    {
        // angle = Vector3.Dot(Vector3.down,sun.forward); // range -1 <> 1 but with non-linear progression, meaning it will go up and down between -1 and 1. Not very usefull because then we don't know the difference between sunrise and sunset.
        sunAngle = Vector3.SignedAngle(Vector3.down, sun1.forward, sun1.right); // range -180 <> 180 with linear progression, meaning -180 is midnight -90 is morning 0 is midday and 90 is sunset.
        sunAngle = sunAngle / 360 + 0.5f;

        // Adjust sun brightness by the angle at which the sun is rotated
        sunLight1.intensity = sunBrightness.Evaluate(sunAngle);
    }

    void SetMoonBrightness()
    {
        // angle = Vector3.Dot(Vector3.down,sun.forward); // range -1 <> 1 but with non-linear progression, meaning it will go up and down between -1 and 1. Not very usefull because then we don't know the difference between sunrise and sunset.
        moonAngle = Vector3.SignedAngle(Vector3.down, moon1.forward, moon1.right); // range -180 <> 180 with linear progression, meaning -180 is midnight -90 is morning 0 is midday and 90 is sunset.
        moonAngle = moonAngle / 360 + 0.5f;

        // Adjust moon brightness by the angle at which the moon is rotated
        moonLight1.intensity = moonBrightness.Evaluate(sunAngle);
    }

    void SetSunColor()
    {
        sunLight1.color = sunColor.Evaluate(sunAngle);
    }

    void UpdateSunAngle()
    {
        sunAngle = Vector3.SignedAngle(Vector3.down, sun1.forward, sun1.right);
        sunAngle = sunAngle / 360 + 0.5f;
    }

    void UpdateMoonAngle()
    {
        moonAngle = Vector3.SignedAngle(Vector3.down, moon1.forward, moon1.right);
        moonAngle = moonAngle / 360 + 0.5f;
    }

    void SetSkyColor()
    {
        if (sunAngle >= 0.25f && sunAngle < 0.75f)
        {
            RenderSettings.skybox.SetColor("_SkyColor2", skyColorDay.Evaluate(sunAngle * 2f - 0.5f));
            accelTime = 1f;
        }
        else if (sunAngle > 0.75f)
        {
            RenderSettings.skybox.SetColor("_SkyColorNight2", skyColorNight.Evaluate(sunAngle * 2f - 1.5f));
            accelTime = 2f;
        }
        else
        {
            RenderSettings.skybox.SetColor("_SkyColorNight2", skyColorNight.Evaluate(sunAngle * 2f + 0.5f));
            accelTime = 2f;
        }
    }

    void SetFogColor()
    {
        RenderSettings.fogColor = fogColor.Evaluate(sunAngle);
    }
}