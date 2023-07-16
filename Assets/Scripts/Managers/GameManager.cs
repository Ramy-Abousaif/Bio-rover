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
    public int rovertype = 0;
    public Material typematt;

    private string[] facts = {
        "Nature has evolved photosynthesis to harness solar energy, which is utilised by nearly all organisms",
        "The maximum theoretical efficiency of photosynthesis is 36%",
        "PV panels typically have a conversion efficiency of around 15%",
        "Combining photosynthesis with human engineering is biomimicry which enables novel devices to be prototyped",
        "Marimo are balls of intertwined algal filaments",
        "Using photosynthesis to directly power movement has several advantages",
        "Bio-rover can be  integrated with conventional sensors and cameras",
        "Bio-rover is electromagnetically silent",
        "Bio-rover can automatically become positively buoyant, and rise, to bypass obstacles",
        "The spherical shape of the bio-rover aids it ability to traverse challenging environments"
    };

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
                UIManager.instance.helpSprite.SetActive(false);
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
                expandUpgrade = 1;
                MissionsManager.instance.Setup();
                StoreManager.instance.AddCredit(50);
                PlayerManager.instance.bc.transform.localPosition = Vector3.zero;
                PlayerManager.instance.bc.transform.localEulerAngles = Vector3.zero;
                break;
        }
    }

    float currentProgress = 0;
    IEnumerator LoadSceneAsync(string sceneIndex, Gamestate _gameState)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        UIManager.instance.progressBar.fillAmount = 0;
        UIManager.instance.factText.text = facts[Random.Range(0, facts.Length)].ToUpper();
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

                if (PlayerInputManager.instance.help)
                    UIManager.instance.helpSprite.SetActive(!UIManager.instance.helpSprite.activeSelf);
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
        AudioManager.instance.Resume();
        UIManager.instance.pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }
}
