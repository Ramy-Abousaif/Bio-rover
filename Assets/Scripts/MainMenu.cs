using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainTitle;
    public GameObject mainButtons;
    public GameObject settingsTitle;
    public GameObject settingsButtons;
    private GameObject activeTitle;
    private GameObject activeButtons;

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
        UIManager.instance.ChangeMenu(settingsTitle, settingsButtons, ref activeTitle, ref activeButtons);
    }

    public void About()
    {
        Application.OpenURL("https://jbioleng.biomedcentral.com/articles/10.1186/s13036-021-00279-0");
    }

    public void Back()
    {
        UIManager.instance.ChangeMenu(mainTitle, mainButtons, ref activeTitle, ref activeButtons);
    }
}
