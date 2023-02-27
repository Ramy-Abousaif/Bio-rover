using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance { get; private set; }

    // Inputs
    public float inputX;
    public float inputY;
    public bool jump;
    public bool scan;
    public bool switchMode;
    public bool spawnBot;
    public bool pickUp;
    public bool goToStore;
    public bool changeSize;

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
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        jump = Input.GetKey(KeyCode.Space);
        scan = Input.GetKeyDown(KeyCode.Q);
        switchMode = Input.GetKeyDown(KeyCode.T);
        spawnBot = Input.GetKeyDown(KeyCode.Alpha1);
        pickUp = Input.GetKeyDown(KeyCode.R);
        goToStore = Input.GetKey(KeyCode.F);
        changeSize = Input.GetKeyDown(KeyCode.E);
    }
}
