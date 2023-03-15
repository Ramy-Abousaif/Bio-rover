using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    public GameObject mainMenu;
    public GameObject loadingScreen;
    public Image progressBar;
    public GameObject pauseMenu;
    public GameObject inGame;
    public GameObject miniMap;
    public GameObject energyUsage;
    public TMP_Text creditsText;

    public GameObject floatingTextPrefab;
    public Transform floatTextPos;

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
        Setup();
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
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    void Setup()
    {
        creditsText.text = GameManager.instance.credits.ToString() + " Credits";
    }
}
