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
    public GameObject HTP1;
    public GameObject HTP2;
    public GameObject HTP3;
    public GameObject HTP4;
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
