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
    [Header("Other")]
    public int selection = 0;
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

    private int interpolationFramesCount = 45;
    int elapsedFrames = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        ChangeSelection();

        // Spawn rover
        if (storedRovers.Count > 0 && PlayerInputManager.instance.releaseRover)
        {
            storedRovers[selection].transform.GetChild(0).position = dropPoint.position;
            storedRovers[selection].SetActive(true);
            storedRovers[selection].transform.GetChild(0).GetComponent<AIController>().enabled = true;
            activeRovers.Add(storedRovers[selection]);
            storedRovers.Remove(storedRovers[selection]);

            if(selection > 0)
                selection--;
        }
    }

    void ChangeSelection()
    {
        if (PlayerInputManager.instance.scroll > 0f)
        {
            selection--;
            if (selection < 0)
                selection = storedRovers.Count - 1;
        }
        else if (PlayerInputManager.instance.scroll < 0f)
        {
            selection++;
            if (selection >= storedRovers.Count)
                selection = 0;
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

        if (other.CompareTag("Rover") && PlayerManager.instance.playerState == PlayerState.BOAT)
        {
            storedRovers.Add(other.transform.parent.gameObject);
            activeRovers.Remove(other.transform.gameObject);
            other.transform.parent.gameObject.SetActive(false);
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