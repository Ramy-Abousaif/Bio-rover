using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance { get; private set; }

    // Inputs
    public float inputX;
    public float inputY;
    public float scroll;
    public bool leftMouse;
    public bool rightMouse;
    public bool rightArrow;
    public bool leftArrow;
    public bool upArrow;
    public bool downArrow;
    public bool help;
    public bool floating;
    public bool scan;
    public bool switchMode;
    public bool releaseRover;
    public bool pickUp;
    public bool goToStore;
    public bool changeSize;
    public bool pause;

    private void Awake()
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

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    void UpdateInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        scroll = Input.GetAxis("Mouse ScrollWheel");
        leftMouse = Input.GetButtonDown("Fire1");
        rightMouse = Input.GetButtonDown("Fire2");
        rightArrow = Input.GetKeyDown(KeyCode.RightArrow);
        leftArrow = Input.GetKeyDown(KeyCode.LeftArrow);
        upArrow = Input.GetKeyDown(KeyCode.UpArrow);
        downArrow = Input.GetKeyDown(KeyCode.DownArrow);
        help = Input.GetKeyDown(KeyCode.H);
        floating = Input.GetKey(KeyCode.Space);
        scan = Input.GetKeyDown(KeyCode.Q);
        switchMode = Input.GetKeyDown(KeyCode.T);
        releaseRover = Input.GetKeyDown(KeyCode.Z);
        pickUp = Input.GetKeyDown(KeyCode.R);
        goToStore = Input.GetKey(KeyCode.F);
        changeSize = Input.GetKeyDown(KeyCode.E);
        pause = Input.GetKeyDown(KeyCode.Escape);
    }
}
