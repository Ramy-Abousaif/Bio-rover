using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Gamestate
{
    MENU,
    LOADING,
    IN_GAME
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public Gamestate gameState = Gamestate.MENU;

    public int credits = 0;
    public int sonarUpgrade = 0;
    public int explosionUpgrade = 0;
    public int expandUpgrade = 0;

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

    void Setup()
    {
        switch (gameState)
        {
            case Gamestate.MENU:
                UIManager.instance.mainMenu.SetActive(true);
                UIManager.instance.inGame.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case Gamestate.LOADING:
                break;
            case Gamestate.IN_GAME:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                UIManager.instance.mainMenu.SetActive(false);
                UIManager.instance.inGame.SetActive(true);
                credits = 0;
                sonarUpgrade = 0;
                explosionUpgrade = 0;
                expandUpgrade = 0;
                StoreManager.instance.AddCredit(50);
                MissionsManager.instance.Setup();
                break;
        }
    }

    float currentProgress = 0;
    IEnumerator LoadSceneAsync(string sceneIndex, Gamestate _gameState)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        UIManager.instance.progressBar.fillAmount = 0;
        gameState = Gamestate.LOADING;

        UIManager.instance.loadingScreen.SetActive(true);
        var scene = SceneManager.LoadSceneAsync(sceneIndex);

        while (!scene.isDone)
        {
            float progress = Mathf.Clamp01(scene.progress / 0.9f);
            currentProgress = progress;
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);
        UIManager.instance.loadingScreen.SetActive(false);
        gameState = _gameState;
        Setup();
    }

    private void Update()
    {
        switch (gameState)
        {
            case Gamestate.MENU:
                break;
            case Gamestate.LOADING:
                UIManager.instance.progressBar.fillAmount = currentProgress;
                break;
            case Gamestate.IN_GAME:
                if (PlayerInputManager.instance.pause)
                    UIManager.instance.Pause();
                break;
        }
    }

    public void PlayGame()
    {
        StartCoroutine(LoadSceneAsync("GameScene", Gamestate.IN_GAME));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        StartCoroutine(LoadSceneAsync("MainMenu", Gamestate.MENU));
        UIManager.instance.pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }
}
