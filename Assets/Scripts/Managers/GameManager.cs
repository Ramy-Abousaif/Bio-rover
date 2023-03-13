using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gamestate
{
    MENU,
    LOADING,
    IN_GAME
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public Gamestate gameState = Gamestate.IN_GAME;

    public int credits = 0;
    public int sonarUpgrade = 0;
    public int explosionUpgrade = 0;
    public int expandUpgrade = 0;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        credits = 0;
        sonarUpgrade = 0;
        explosionUpgrade = 0;
        expandUpgrade = 0;
        StoreManager.instance.AddCredit(50);
    }
}
