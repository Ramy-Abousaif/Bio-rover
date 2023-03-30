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
    public GameObject roverPrefab;
    public GameObject arrow;
    public Camera playerCam;
    public CinemachineVirtualCamera boatVCam;
    public CinemachineFreeLook roverVCam;
    private int roverIndex = 0;
    public GameObject currentRover;
    public GameObject boat;
    public RoverController rc;
    public BoatController bc;

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
        switch(playerState)
        {
            case PlayerState.BOAT:

                ChangeRover();

                if (PlayerInputManager.instance.switchMode && bc.activeRovers.Count > 0 && !StoreManager.instance.inStore)
                    ChangePlayerState(PlayerState.ROVER);

                break;
            case PlayerState.ROVER:

                if(PlayerInputManager.instance.switchMode)
                    ChangePlayerState(PlayerState.BOAT);

                break;
        }
    }

    void ChangeRover()
    {
        if(bc.activeRovers.Count > 0)
        {
            if (PlayerInputManager.instance.scroll > 0f)
                roverIndex = (roverIndex - 1 + bc.activeRovers.Count) % bc.activeRovers.Count;
            else if (PlayerInputManager.instance.scroll < 0f)
                roverIndex = (roverIndex + 1) % bc.activeRovers.Count;
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

        // Change to boat
        if (state == PlayerState.BOAT)
        {
            UIManager.instance.energyUsage.SetActive(false);
            UIManager.instance.miniMap.SetActive(true);
            bc.enabled = true;

            if(currentRover != null)
                currentRover.GetComponent<AIController>().enabled = true;

            if (rc != null)
            {
                rc.UIBall.SetActive(false);
                rc.enabled = false;
            }

            arrow.SetActive(false);
            boatVCam.gameObject.SetActive(true);
            roverVCam.gameObject.SetActive(false);
        }

        // Change to rover
        if (state == PlayerState.ROVER && bc.activeRovers.Count > 0)
        {
            UIManager.instance.energyUsage.SetActive(true);
            UIManager.instance.miniMap.SetActive(false);
            currentRover = bc.activeRovers[roverIndex].transform.GetChild(0).gameObject;
            rc = currentRover.GetComponent<RoverController>();
            currentRover.GetComponent<AIController>().enabled = false;
            bc.enabled = false;
            rc.enabled = true;
            rc.UIBall.SetActive(true);
            boatVCam.gameObject.SetActive(false);
            roverVCam.gameObject.SetActive(true);
        }
    }
}
