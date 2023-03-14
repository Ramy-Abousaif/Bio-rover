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
    private int screenWidth = 1920;
    private int screenHeight = 1080;

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

    public void SetWidth(int newWidth)
    {
        screenWidth = newWidth;
    }

    public void SetHeight(int newHeight)
    {
        screenHeight = newHeight;
    }

    public void SetResolution()
    {
        Screen.SetResolution(screenWidth, screenHeight, false);
    }

    public void Settings()
    {
        StartCoroutine(ChangeMenu(settingsTitle, settingsButtons));
    }

    public void Back()
    {
        StartCoroutine(ChangeMenu(mainTitle, mainButtons));
    }

    IEnumerator ChangeMenu(GameObject newTitle, GameObject newButtons)
    {
        GameObject prevTitle = activeTitle;
        GameObject prevButtons = activeButtons;
        yield return new WaitForSeconds(0.8f);
        prevTitle.SetActive(false);
        prevButtons.SetActive(false);
        yield return new WaitForSeconds(0.02f);
        activeTitle = newTitle;
        activeButtons = newButtons;
        activeTitle.SetActive(true);
        activeButtons.SetActive(true);
        yield return null;
    }
}
