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
    public bool leftArrow;
    public bool rightArrow;
    public bool upArrow;
    public bool downArrow;
    public bool jump;
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
        leftArrow = Input.GetKeyDown(KeyCode.LeftArrow);
        rightArrow = Input.GetKeyDown(KeyCode.RightArrow);
        upArrow = Input.GetKeyDown(KeyCode.UpArrow);
        downArrow = Input.GetKeyDown(KeyCode.DownArrow);
        jump = Input.GetKey(KeyCode.Space);
        scan = Input.GetKeyDown(KeyCode.Q);
        switchMode = Input.GetKeyDown(KeyCode.T);
        releaseRover = Input.GetKeyDown(KeyCode.Z);
        pickUp = Input.GetKeyDown(KeyCode.R);
        goToStore = Input.GetKey(KeyCode.F);
        changeSize = Input.GetKeyDown(KeyCode.E);
        pause = Input.GetKeyDown(KeyCode.Escape);
    }
}
