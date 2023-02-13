using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    ROVER,
    BOAT
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance { get; private set; }

    [HideInInspector]
    public PlayerState playerState;
    public GameObject rover;
    public GameObject boat;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            if (playerState == PlayerState.BOAT)
                ChangePlayerState(PlayerState.ROVER);
            else
                ChangePlayerState(PlayerState.BOAT);
        }
    }

    void SetUp()
    {
        ChangePlayerState(PlayerState.BOAT);
    }

    void ChangePlayerState(PlayerState state)
    {
        playerState = state;

        if (state == PlayerState.BOAT)
        {
            UIManager.instance.energyUsage.SetActive(false);
            boat.SetActive(true);
            rover.SetActive(false);
        }

        if (state == PlayerState.ROVER)
        {
            UIManager.instance.energyUsage.SetActive(true);
            boat.SetActive(false);
            rover.SetActive(true);
        }
    }
}
