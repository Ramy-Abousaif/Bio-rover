using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    public GameObject inGame;
    public GameObject energyUsage;
    public GameObject money;
    public TMP_Text moneyText;

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
        moneyText.text = GameManager.instance.money.ToString() + " Credits";
    }
}
