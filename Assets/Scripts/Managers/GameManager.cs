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

    public int money = 0;
    public int scanned_objs = 0;
    public int bot_count = 0;

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
        money = 0;
        bot_count = 0;
    }
}
