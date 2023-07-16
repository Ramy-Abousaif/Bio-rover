using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class ResearchBaseManager : MonoBehaviour
{

    private PlayerManager playerManager;
    private CinemachineVirtualCamera uiVCam;
    private bool instore;
    private int roverType;
    public int playerRoverType;

    public Canvas storeCanvas;
    public TMP_Text creditText;
    private GameObject HUD;

    [Header("Main Menu")]
    public GameObject mainMenuHolder;
    public Button upgradeButton;
    public Button missionButton;
    public Button roverButton;
    public Button mainBack;

    [Header("Upgrade Screen")]
    public GameObject upgradeMenuHolder;
    public Button Basic;
    public Button Explosive;
    public Button Sonar;
    public Button Fast;
    public Button Light;
    public Button Treaded;
    public Button Confirm;
    public Button upgradeBack;
    public GameObject exampleFrame;
    private MeshRenderer frame;
    public Material exampleMatt;
    public Material basicMatt;
    public Material explosiveMatt;
    public Material sonarMatt;
    public Material fastMatt;
    public Material lightMatt;
    public Material treadedMatt;

    [Header("Mission Screen")]
    public GameObject missionMenuHolder;
    public Button M1;
    public Button M2;
    public Button M3;
    public Button M4;
    public Button M5;
    public Button M6;
    public TMP_Text missionDescription;
    public Button MissionBack;

    [Header("Rover Screen")]
    public GameObject roverMenuHolder;
    public Button Buy;
    public Button Recall;
    public Button Recycle;
    public Button RoverBack;

    // Start is called before the first frame update
    void Start()
    {

        //variable setup
        roverType = 0;
        playerRoverType = 0;
        instore = false;
        GameObject player = GameObject.Find("Player");
        playerManager = player.GetComponent<PlayerManager>();
        GameObject cam = GameObject.Find("MainBaseVCam");
        uiVCam = cam.gameObject.GetComponent<CinemachineVirtualCamera>();
        frame = exampleFrame.gameObject.GetComponent<MeshRenderer>();
        HUD = GameObject.Find("In-Game");

        //button setup
        //mainmenu
        upgradeButton.onClick.AddListener(Upgrade);
        missionButton.onClick.AddListener(MissionScreen);
        roverButton.onClick.AddListener(RoverManagement);
        mainBack.onClick.AddListener(ReturnToGame);
        RoverBack.onClick.AddListener(returnToMain);

        //Customization menu
        Basic.onClick.AddListener(SelectBasic);
        Explosive.onClick.AddListener(SelectExplosive);
        Sonar.onClick.AddListener(SelectSonar);
        Fast.onClick.AddListener(SelectFast);
        Light.onClick.AddListener(SelectLight);
        Treaded.onClick.AddListener(SelectTreaded);
        upgradeBack.onClick.AddListener(returnToMain);
        Confirm.onClick.AddListener(ConfirmSale);

        //Mission menu
        MissionBack.onClick.AddListener(returnToMain);

        //Rovermenu
        RoverBack.onClick.AddListener(returnToMain);
        Buy.onClick.AddListener(BuyRover);
        Recall.onClick.AddListener(RecallRover);
        Recycle.onClick.AddListener(RecycleRover);
}

    // Update is called once per frame
    void Update()
    {

    }


    public void enterResearchBase()
    {
        instore = true;
        //make canvas visible & change camera angle
        storeCanvas.gameObject.SetActive(true);
        PlayerManager.instance.boatVCam.gameObject.SetActive(false);
        uiVCam.gameObject.SetActive(true);
        mainMenuHolder.gameObject.SetActive(true);
        PlayerManager.instance.bc.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        creditText.text = "Credits: " + GameManager.instance.credits.ToString();
        HUD.SetActive(false);

    }

    private void returnToMain()
    {
        upgradeMenuHolder.gameObject.SetActive(false);
        roverMenuHolder.gameObject.SetActive(false);
        missionMenuHolder.gameObject.SetActive(false);
        mainMenuHolder.gameObject.SetActive(true);
    }


    public void newPage(GameObject newpg)
    {
        newpg.gameObject.SetActive(true);
        mainMenuHolder.gameObject.SetActive(false);
    }

    //Main Menu Button Events
    public void Upgrade()
    {
        SelectRoverType(playerRoverType);
        newPage(upgradeMenuHolder);
    }

    public void RoverManagement()
    {
        newPage(roverMenuHolder);
    }

    public void MissionScreen()
    {
        newPage(missionMenuHolder);
    }

    //Customize Rover

    public void SelectBasic()
    {
        roverType = 0;
        SelectRoverType(roverType);
    }

    public void SelectExplosive()
    {
        roverType = 1;
        SelectRoverType(roverType);
    }

    public void SelectSonar()
    {
        roverType = 2;
        SelectRoverType(roverType);
    }

    public void SelectFast()
    {
        roverType = 3;
        SelectRoverType(roverType);
    }

    public void SelectLight()
    {
        roverType = 4;
        SelectRoverType(roverType);
    }

    public void SelectTreaded()
    {
        roverType = 5;
        SelectRoverType(roverType);
    }

    public void SelectRoverType(int Roverindex)
    {
        switch (Roverindex)
        {
            case 1:
                exampleMatt = explosiveMatt;
                ExampleDisplay(exampleMatt);
                break;
            case 2:
                exampleMatt = sonarMatt;
                ExampleDisplay(exampleMatt);
                break;
            case 3:
                exampleMatt = fastMatt;
                ExampleDisplay(exampleMatt);
                break;
            case 4:
                exampleMatt = lightMatt;
                ExampleDisplay(exampleMatt);
                break;
            case 5:
                exampleMatt = treadedMatt;
                ExampleDisplay(exampleMatt);
                break;
            case 0:
                exampleMatt = basicMatt;
                ExampleDisplay(exampleMatt);
                break;
            default:
                Debug.Log("Invalid function index");
                break;
        }
    }


    public void ExampleDisplay(Material matt)
    {
        frame.material = matt;
    }
    public void ConfirmSale()
    {
        GameObject[] AllRovers = GameObject.FindGameObjectsWithTag("Rover");

        switch (roverType)
        {
            case 1:
                if (GameManager.instance.credits >= 100)
                {
                    playerRoverType = roverType;
                    GameManager.instance.rovertype = playerRoverType;
                    GameManager.instance.typematt = exampleMatt;
                    GameManager.instance.credits = (GameManager.instance.credits - 100);
                    creditText.text = "Credits: " + GameManager.instance.credits.ToString();
                    foreach (GameObject rover in AllRovers)
                    {
                        RoverController cont = (RoverController)rover.GetComponent("RoverController");
                        cont.ChangeRoverType(playerRoverType);
                    }
                }
                break;
            case 2:
                if (GameManager.instance.credits >= 100)
                {
                    playerRoverType = roverType;
                    GameManager.instance.rovertype = playerRoverType;
                    GameManager.instance.typematt = exampleMatt;
                    GameManager.instance.credits = (GameManager.instance.credits - 100);
                    creditText.text = "Credits: " + GameManager.instance.credits.ToString();
                    foreach (GameObject rover in AllRovers)
                    {
                        RoverController cont = (RoverController)rover.GetComponent("RoverController");
                        cont.ChangeRoverType(playerRoverType);
                    }
                }
                break;
            case 3:
                if (GameManager.instance.credits >= 100)
                {
                    playerRoverType = roverType;
                    GameManager.instance.rovertype = playerRoverType;
                    GameManager.instance.typematt = exampleMatt;
                    GameManager.instance.credits = (GameManager.instance.credits - 100);
                    creditText.text = "Credits: " + GameManager.instance.credits.ToString();
                    foreach (GameObject rover in AllRovers)
                    {
                        RoverController cont = (RoverController)rover.GetComponent("RoverController");
                        cont.ChangeRoverType(playerRoverType);
                    }
                }
                break;
            case 4:
                if (GameManager.instance.credits >= 100)
                {
                    playerRoverType = roverType;
                    GameManager.instance.rovertype = playerRoverType;
                    GameManager.instance.typematt = exampleMatt;
                    GameManager.instance.credits = (GameManager.instance.credits - 100);
                    creditText.text = "Credits: " + GameManager.instance.credits.ToString();
                    foreach (GameObject rover in AllRovers)
                    {
                        RoverController cont = (RoverController)rover.GetComponent("RoverController");
                        cont.ChangeRoverType(playerRoverType);
                    }
                }
                break;
            case 5:
                if (GameManager.instance.credits >= 100)
                {
                    playerRoverType = roverType;
                    GameManager.instance.rovertype = playerRoverType;
                    GameManager.instance.typematt = exampleMatt;
                    GameManager.instance.credits = (GameManager.instance.credits - 100);
                    creditText.text = "Credits: " + GameManager.instance.credits.ToString();
                    foreach (GameObject rover in AllRovers)
                    {
                        RoverController cont = (RoverController)rover.GetComponent("RoverController");
                        cont.ChangeRoverType(playerRoverType);
                    }
                }
                break;
            case 0:
                if (GameManager.instance.credits >= 100)
                {
                    playerRoverType = roverType;
                    GameManager.instance.rovertype = playerRoverType;
                    GameManager.instance.typematt = exampleMatt;
                    GameManager.instance.credits = (GameManager.instance.credits - 100);
                    creditText.text = "Credits: " + GameManager.instance.credits.ToString();
                    foreach (GameObject rover in AllRovers)
                    {
                        RoverController cont = (RoverController)rover.GetComponent("RoverController");
                        cont.ChangeRoverType(playerRoverType);
                    }
                }
                break;
            default:
                Debug.Log("Invalid function index");
                break;
        }
    }


    // Rover Management
    public void BuyRover()
    {
        if (GameManager.instance.credits >= 50);
        {
            creditText.text = "Credits: " + GameManager.instance.credits.ToString();
            GameObject roverInstance = Instantiate(PlayerManager.instance.roverPrefab, transform.position, Quaternion.identity);
            PlayerManager.instance.bc.TakeRover(roverInstance.transform.GetChild(0).gameObject);
            UIManager.instance.activeRoversText.text = PlayerManager.instance.bc.activeRovers.Count + " Active Rovers";
            UIManager.instance.storedRoversText.text = PlayerManager.instance.bc.storedRovers.Count + " Stored Rovers";
            GameManager.instance.credits = (GameManager.instance.credits - 50);
            creditText.text = "Credits: " + GameManager.instance.credits.ToString();
        }
    }

    public void RecallRover()
    {
        GameObject[] RecallArray = GameObject.FindGameObjectsWithTag("Rover");

        for (int i = 0; i < RecallArray.Length; i++)
        {
            if (RecallArray[i].activeSelf == true && GameManager.instance.credits >= 300)
            {
                PlayerManager.instance.bc.TakeRover(RecallArray[i]);
                GameManager.instance.credits = (GameManager.instance.credits - 300);
                creditText.text = "Credits: " + GameManager.instance.credits.ToString();
            }
        }
    }

    public void RecycleRover()
    {
        GameObject[] RecycleArray = GameObject.FindGameObjectsWithTag("Rover");

        for (int i = 0; i < RecycleArray.Length; i++)
        {
            if (RecycleArray[i].activeSelf == false)
            {
                PlayerManager.instance.bc.storedRovers.Remove(RecycleArray[i]);
                Destroy(RecycleArray[i]);
                GameManager.instance.credits = (GameManager.instance.credits + 25);
                creditText.text = "Credits: " + GameManager.instance.credits.ToString();
            }
        }
    }

    //LeaveStore
    public void ReturnToGame()
    {
        instore = false;
        //make canvas visible & change camera angle
        storeCanvas.gameObject.SetActive(false);
        PlayerManager.instance.boatVCam.gameObject.SetActive(true);
        uiVCam.gameObject.SetActive(false);
        mainMenuHolder.gameObject.SetActive(false);
        PlayerManager.instance.bc.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        HUD.SetActive(true);
    }

}
