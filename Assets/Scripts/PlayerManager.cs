using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    public Camera playerCam;
    public CinemachineVirtualCamera boatVCam;
    public CinemachineFreeLook roverVCam;
    public GameObject currentRover;
    public GameObject boat;
    public RoverController rc;
    public BoatController bc;
    public Transform dropPoint;

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
        if(PlayerInputManager.instance.switchMode)
        {
            if (playerState == PlayerState.BOAT && currentRover != null)
                ChangePlayerState(PlayerState.ROVER);
            else
                ChangePlayerState(PlayerState.BOAT);
        }
    }

    void SetUp()
    {
        playerCam = Camera.main;
        ChangePlayerState(PlayerState.BOAT);
    }

    void ChangePlayerState(PlayerState state)
    {
        playerState = state;

        if (state == PlayerState.BOAT)
        {
            UIManager.instance.energyUsage.SetActive(false);
            bc.enabled = true;
            rc.enabled = false;
            boatVCam.gameObject.SetActive(true);
            roverVCam.gameObject.SetActive(false);
        }

        if (state == PlayerState.ROVER && currentRover != null)
        {
            UIManager.instance.energyUsage.SetActive(true);
            bc.enabled = false;
            currentRover.transform.SetParent(transform);
            currentRover.SetActive(true);
            rc.enabled = true;
            boatVCam.gameObject.SetActive(false);
            roverVCam.gameObject.SetActive(true);
        }
    }
}
