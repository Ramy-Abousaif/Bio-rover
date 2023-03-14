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
    public GameObject inGame;
    public GameObject energyUsage;
    public TMP_Text creditsText;

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
        creditsText.text = GameManager.instance.credits.ToString() + " Credits";
    }
}
