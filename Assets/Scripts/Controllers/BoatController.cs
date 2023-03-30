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
        this.RotateAnim();

        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, transform.eulerAngles.y, rotation.eulerAngles.z);
        if (PlayerInputManager.instance.inputY != 0f)
            this.dragT += PlayerInputManager.instance.inputY * this.acceleration;
        else if (this.dragT > 0.5f)
            this.dragT -= this.acceleration;
        else if (this.dragT < 0.5f)
            this.dragT += this.acceleration;

        this.dragT = Mathf.Clamp01(this.dragT);
        this.motor.speed = Mathf.Lerp(this.motor.startSpeed / 2f, this.motor.startSpeed * 1.5f, this.dragT);

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
        this.zRot = Mathf.Clamp(PlayerInputManager.instance.inputX * this.angularSpeed * Time.deltaTime, -25f, 25f);
        this.mesh.transform.localRotation = Quaternion.Slerp(this.mesh.transform.localRotation,
            Quaternion.Euler(this.mesh.transform.localRotation.x, this.mesh.transform.localRotation.y, this.zRot), interpolationRatio);
    }

    private void LateUpdate()
    {
        this.currentDrag = Mathf.Lerp(this.minDrag, this.maxDrag, this.dragT);
    }

    private void FixedUpdate()
    {
        this.Rotation();
        this.Movement();
    }

    private void Movement()
    {
        this.rb.velocity = Vector3.zero;
        float d = (Mathf.Abs(PlayerInputManager.instance.inputX) > 0f) ? (this.speed * this.turningResponsiveness) : this.speed;
        this.rb.AddForce(this.rotation.forward * d * this.currentDrag, ForceMode.VelocityChange);
    }

    private void Rotation()
    {
        if (Mathf.Sqrt((this.rb.velocity.x * this.rb.velocity.x) + (this.rb.velocity.z * this.rb.velocity.z)) > 20.0f)
            this.rb.AddTorque(base.transform.up * this.angularSpeed * PlayerInputManager.instance.inputX);
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

    private void OnEnable()
    {
        motor.isSpinning = true;
        motor.splash.Play();
        motor.foam.Play();
    }

    private void OnDisable()
    {
        motor.isSpinning = false;
        motor.splash.Stop();
        motor.foam.Stop();
    }
}