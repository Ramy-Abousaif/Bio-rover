using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseTitle;
    public GameObject pauseButtons;
    public GameObject settingsTitle;
    public GameObject settingsButtons;
    private GameObject activeTitle;
    private GameObject activeButtons;
    public GameObject howToPlayButtons;
    public GameObject HTP1;
    public GameObject HTP2;
    public GameObject HTP3;
    public GameObject HTP4;
    public Slider masterSlider;
    public Slider bgSlider;
    public Slider sfxSlider;

    private void Start()
    {
        activeTitle = pauseTitle;
        activeButtons = pauseButtons;
    }

    public void Resume()
    {
        AudioManager.instance.Resume();
        if (StoreManager.instance != null)
        {
            if (StoreManager.instance.inStore)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void Settings()
    {
        UpdateSliders();
        UIManager.instance.ChangeMenu(settingsTitle, settingsButtons, ref activeTitle, ref activeButtons);
    }


    public void HowToPlay1()
    {
        UIManager.instance.ChangeMenu(HTP1, howToPlayButtons, ref activeTitle, ref activeButtons);
    }

    public void HowToPlay2()
    {
        UIManager.instance.ChangeMenu(HTP2, howToPlayButtons, ref activeTitle, ref activeButtons);
    }

    public void HowToPlay3()
    {
        UIManager.instance.ChangeMenu(HTP3, howToPlayButtons, ref activeTitle, ref activeButtons);
    }

    public void HowToPlay4()
    {
        UIManager.instance.ChangeMenu(HTP4, howToPlayButtons, ref activeTitle, ref activeButtons);
    }

    private void UpdateSliders()
    {
        masterSlider.value = AudioManager.instance.masterVolume;
        bgSlider.value = AudioManager.instance.bgVolume;
        sfxSlider.value = AudioManager.instance.sfxVolume;
    }

    public void Back()
    {
        UIManager.instance.ChangeMenu(pauseTitle, pauseButtons, ref activeTitle, ref activeButtons);
    }
}
