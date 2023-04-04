using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Multipliers")]
    public float speed;
    public float angularSpeed;
    [SerializeField]
    private float turningResponsiveness;
    [SerializeField]
    private float acceleration;
    [Header("References")]
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform rotation;
    [SerializeField]
    private Transform mesh;
    [SerializeField]
    private Transform boatCamFollow;
    [SerializeField]
    private Transform dropPoint;
    [SerializeField]
    private ResetChecker resetChecker;
    [Header("Other")]
    private int selection = 0;
    public List<GameObject> storedRovers = new List<GameObject>();
    public List<GameObject> activeRovers = new List<GameObject>();
    private float minDrag = 2f;
    [SerializeField]
    private float maxDrag = 6f;
    private float currentDrag = 6f;
    private float dragT = 0.5f;
    private float zRot;
    private bool stopPressed = false;
    private bool stopped = false;
    [SerializeField]
    private Motor motor;
    private Vector3 lastSavedPos;
    private Vector3 lastSavedRot;

    private int interpolationFramesCount = 45;
    int elapsedFrames = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(SavePosAndRot(10.0f));
        UIManager.instance.activeRoversText.text = activeRovers.Count + " Active Rovers";
        UIManager.instance.storedRoversText.text = storedRovers.Count + " Stored Rovers";
    }

    private void Update()
    {
        RotateAnim();

        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, transform.eulerAngles.y, rotation.eulerAngles.z);
        if (PlayerInputManager.instance.inputY > 0f)
        {
            dragT += PlayerInputManager.instance.inputY * acceleration;
            ResumeMovement();
        }
        else if (PlayerInputManager.instance.inputY == 0f)
        {
            stopPressed = false;
        }
        else if (dragT > 0.5f)
        {
            dragT -= acceleration;
        }

        if(PlayerInputManager.instance.inputX != 0)
        {
            ResumeMovement();
        }

        if (PlayerInputManager.instance.inputY < 0f)
        {
            if (!stopPressed)
            {
                stopPressed = true;
                stopped = !stopped;
                motor.isSpinning = !stopped;
                MotorEffect();
            }
        }

        dragT = Mathf.Clamp01(dragT);
        motor.speed = Mathf.Lerp(motor.startSpeed / 2f, motor.startSpeed * 1.5f, dragT);

        elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);
        boatCamFollow.position = transform.position;

        // Spawn rover
        if (storedRovers.Count > 0 && PlayerInputManager.instance.releaseRover)
        {
            storedRovers[selection].transform.GetChild(0).position = dropPoint.position;
            storedRovers[selection].SetActive(true);
            storedRovers[selection].transform.GetChild(0).GetComponent<AIController>().enabled = true;
            activeRovers.Add(storedRovers[selection]);
            storedRovers.Remove(storedRovers[selection]);
            UIManager.instance.activeRoversText.text = activeRovers.Count + " Active Rovers";
            UIManager.instance.storedRoversText.text = storedRovers.Count + " Stored Rovers";

            if (selection > 0)
                selection--;
        }
    }

    private void RotateAnim()
    {
        float interpolationRatio = ((float)elapsedFrames / interpolationFramesCount) * 0.025f;
        zRot = Mathf.Clamp(PlayerInputManager.instance.inputX * angularSpeed * Time.deltaTime, -25f, 25f);
        mesh.transform.localRotation = Quaternion.Slerp(mesh.transform.localRotation,
            Quaternion.Euler(mesh.transform.localRotation.x, mesh.transform.localRotation.y, zRot), interpolationRatio);
    }

    private void LateUpdate()
    {
        currentDrag = Mathf.Lerp(minDrag, maxDrag, dragT);
    }

    private void FixedUpdate()
    {
        Rotation();
        Movement();
    }

    private void Movement()
    {
        rb.velocity = Vector3.zero;
        float d = (Mathf.Abs(PlayerInputManager.instance.inputX) > 0f) ? (speed * turningResponsiveness) : speed;
        if(!stopped)
            rb.AddForce(rotation.forward * d * currentDrag, ForceMode.VelocityChange);
    }

    private void Rotation()
    {
        if (Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z)) > 20.0f)
            rb.AddTorque(transform.up * angularSpeed * PlayerInputManager.instance.inputX);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PlayerInputManager.instance.pickUp)
            return;

        if (other.CompareTag("Rover"))
            TakeRover(other.gameObject);
    }

    public void TakeRover(GameObject rover)
    {
        storedRovers.Add(rover.transform.parent.gameObject);
        activeRovers.Remove(rover.transform.parent.gameObject);
        rover.GetComponent<AIController>().FullRecharge();
        rover.transform.parent.gameObject.SetActive(false);
        UIManager.instance.activeRoversText.text = activeRovers.Count + " Active Rovers";
        UIManager.instance.storedRoversText.text = storedRovers.Count + " Stored Rovers";
    }

    private void ResumeMovement()
    {
        stopPressed = false;
        if (stopped)
        {
            stopped = false;
            motor.isSpinning = true;
            MotorEffect();
        }
    }

    private IEnumerator SavePosAndRot(float duration)
    {
        while(true)
        {
            resetChecker.transform.position = transform.position;
            resetChecker.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            if (resetChecker.noBlocks)
            {
                lastSavedPos = transform.position;
                lastSavedRot = transform.eulerAngles;
            }
            yield return new WaitForSeconds(0.5f);
            resetChecker.gameObject.SetActive(false);
            yield return new WaitForSeconds(duration);
        }
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Default") || collision.gameObject.layer == LayerMask.NameToLayer("Homebase"))
        {
            PoolManager.instance.SpawnPoof(transform.position, Quaternion.identity);
            transform.position = lastSavedPos;
            transform.eulerAngles = lastSavedRot;
            PoolManager.instance.SpawnPoof(transform.position, Quaternion.identity);
        }
    }

    private void MotorEffect()
    {
        if (motor.isSpinning)
        {
            motor.splash.Play();
            motor.foam.Play();
        }
        else
        {
            motor.splash.Stop();
            motor.foam.Stop();
        }
    }

    private void OnEnable()
    {
        motor.isSpinning = !stopped;
        MotorEffect();
    }

    private void OnDisable()
    {
        motor.isSpinning = false;
        MotorEffect();
    }
}