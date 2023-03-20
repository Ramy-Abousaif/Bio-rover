using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Serializable]
    public class sfxLib
    {
        public string Name;
        public EventReference sfxPath;
    }

    [Header("Volume Sliders")]
    public float masterVolume = 0.5f;
    public float bgVolume = 0.5f;
    public float sfxVolume = 0.5f;
    //public int testIndex = 2;

    //FMOD Variables
    Bus masterBus;
    Bus backgroundBus;
    Bus sfxBus;
    EventInstance ambienceInstance;
    EventInstance musicInstance;
    [Header("Events Selector")]
    [Space(20)]
    public EventReference ambienceEvent;
    public EventReference musicEvent;
    private static string sfxDir = "event:/SFX/";
    public List<sfxLib> sfxObjectsList;

    void Awake()
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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/Master");
        backgroundBus = RuntimeManager.GetBus("bus:/Master/Background");
        sfxBus = RuntimeManager.GetBus("bus:/Master/SFX");
        masterBus.setVolume(this.masterVolume);
        backgroundBus.setVolume(this.bgVolume);
        sfxBus.setVolume(this.sfxVolume);
        StartAmbience();
        //StartMusic();
    }

    private void Update()
    {
        ambienceInstance.setParameterByName("Underwater", (Camera.main.transform.position.y >
            WaveManager.instance.getHeight(Camera.main.transform.position.x, Camera.main.transform.position.z)) ? 0f : 1f);
        //musicInstance.setParameterByName("Scene", SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeSFXVolume(float newSFXVolume)
    {
        this.sfxVolume = newSFXVolume;
        sfxBus.setVolume(this.sfxVolume);
    }

    public void ChangeBGVolume(float newBGVolume)
    {
        this.bgVolume = newBGVolume;
        backgroundBus.setVolume(this.bgVolume);
    }

    public void ChangeMasterVolume(float newMasterVolume)
    {
        this.masterVolume = newMasterVolume;
        masterBus.setVolume(this.masterVolume);
    }

    public void PlayOneShotWithParameters(string fmodEvent, Transform t, params (string name, float value)[] parameters)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sfxDir + fmodEvent);

        foreach (var (name, value) in parameters)
        {
            instance.setParameterByName(name, value);
        }

        if (t.GetComponent<Rigidbody>() != null)
            RuntimeManager.AttachInstanceToGameObject(instance, t, t.GetComponent<Rigidbody>());

        instance.set3DAttributes(t.position.To3DAttributes());
        instance.start();
        instance.release();
    }

    public void StartAmbience()
    {
        ambienceInstance = RuntimeManager.CreateInstance(ambienceEvent);
        ambienceInstance.start();
        //ambienceInstance.release();
    }

    public void StartMusic()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
        musicInstance.release();
    }

    public void ReleaseAmbience()
    {
        ambienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ReleaseMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void Resume()
    {
        masterBus.setPaused(false);
    }

    public void Pause()
    {
        masterBus.setPaused(true);
    }

    public void OnDestroy()
    {
        ReleaseAmbience();
        ReleaseMusic();
    }
}