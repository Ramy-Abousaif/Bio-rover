using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainTitle;
    public GameObject mainButtons;
    public GameObject settingsTitle;
    public GameObject settingsButtons;
    public GameObject howToPlayScreen;
    public GameObject howToPlayButtons;
    public GameObject aboutTitle;
    public GameObject aboutButtons;
    private GameObject activeTitle;
    private GameObject activeButtons;
    public Slider masterSlider;
    public Slider bgSlider;
    public Slider sfxSlider;

    void Start()
    {
        //Initiating
        mainTitle.SetActive(true);
        mainButtons.SetActive(true);
        settingsTitle.SetActive(false);
        settingsButtons.SetActive(false);
        activeTitle = mainTitle;
        activeButtons = mainButtons;
    }

    public void Settings()
    {
        UpdateSliders();
        UIManager.instance.ChangeMenu(settingsTitle, settingsButtons, ref activeTitle, ref activeButtons);
    }

    public void HowToPlay()
    {
        UIManager.instance.ChangeMenu(howToPlayScreen, howToPlayButtons, ref activeTitle, ref activeButtons);
    }

    public void About()
    {
        UIManager.instance.ChangeMenu(aboutTitle, aboutButtons, ref activeTitle, ref activeButtons);
    }

    private void UpdateSliders()
    {
        masterSlider.value = AudioManager.instance.masterVolume;
        bgSlider.value = AudioManager.instance.bgVolume;
        sfxSlider.value = AudioManager.instance.sfxVolume;
    }

    public void Notes1()
    {
        Application.OpenURL("https://jbioleng.biomedcentral.com/articles/10.1186/s13036-021-00279-0");
    }

    public void Notes2()
    {
        Application.OpenURL("https://jbioleng.biomedcentral.com/articles/10.1186/s13036-019-0200-5");
    }

    public void Back()
    {
        UIManager.instance.ChangeMenu(mainTitle, mainButtons, ref activeTitle, ref activeButtons);
    }
}
