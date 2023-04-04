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
    public TMP_Text factText;
    public Image progressBar;
    public GameObject pauseMenu;
    public GameObject inGame;
    public GameObject miniMap;
    public GameObject energyUsage;
    public TMP_Text creditsText;
    public TMP_Text storedRoversText;
    public TMP_Text activeRoversText;
    public GameObject helpSprite;
    public Image blackScreenSprite;
    private Coroutine prevCoroutine = null;
    public GameObject scanTextPrefab;
    public Transform scanTextPos;
    public GameObject creditAddTextPrefab;
    public Transform creditAddTextPos;
    private int screenWidth = 1920;
    private int screenHeight = 1080;

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

    public void Pause()
    {
        AudioManager.instance.Pause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ChangeMenu(GameObject _newTitle, GameObject _newButtons, ref GameObject _activeTitle, ref GameObject _activeButtons)
    {
        GameObject prevTitle = _activeTitle;
        GameObject prevButtons = _activeButtons;
        prevTitle.SetActive(false);
        prevButtons.SetActive(false);
        _activeTitle = _newTitle;
        _activeButtons = _newButtons;
        _activeTitle.SetActive(true);
        _activeButtons.SetActive(true);
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
        Screen.SetResolution(screenWidth, screenHeight, true);
    }

    void Setup()
    {
        creditsText.text = GameManager.instance.credits.ToString() + " Credits";
    }
}
