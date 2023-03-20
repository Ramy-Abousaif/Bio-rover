using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseTitle;
    public GameObject pauseButtons;
    public GameObject settingsTitle;
    public GameObject settingsButtons;
    private GameObject activeTitle;
    private GameObject activeButtons;

    private void Start()
    {
        activeTitle = pauseTitle;
        activeButtons = pauseButtons;
    }

    public void Resume()
    {
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
        UIManager.instance.ChangeMenu(settingsTitle, settingsButtons, ref activeTitle, ref activeButtons);
    }

    public void Back()
    {
        UIManager.instance.ChangeMenu(pauseTitle, pauseButtons, ref activeTitle, ref activeButtons);
    }
}
