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
    private GameObject botRover;
    [Header("Other")]
    public int botCount = 0;
    private float minDrag = 2f;
    [SerializeField]
    private float maxDrag = 6f;
    private float currentDrag = 6f;
    private float dragT = 0.5f;
    private float xInput;
    private float yInput;
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
        this.xInput = Input.GetAxisRaw("Horizontal");
        this.yInput = Input.GetAxisRaw("Vertical");
        this.RotateAnim();

        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, transform.eulerAngles.y, rotation.eulerAngles.z);
        if (this.yInput != 0f)
            this.dragT += this.yInput * this.acceleration;
        else if (this.dragT > 0.5f)
            this.dragT -= this.acceleration;
        else if (this.dragT < 0.5f)
            this.dragT += this.acceleration;

        this.dragT = Mathf.Clamp01(this.dragT);
        this.motor.speed = Mathf.Lerp(this.motor.startSpeed / 2f, this.motor.startSpeed * 1.5f, this.dragT);

        elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);
        boatCamFollow.position = transform.position;

        if (botCount > 0 && PlayerInputManager.instance.spawnBot)
        {
            Instantiate(botRover, PlayerManager.instance.dropPoint.position, Quaternion.identity);
            botCount--;
        }
    }

    private void RotateAnim()
    {
        float interpolationRatio = ((float)elapsedFrames / interpolationFramesCount) * 0.025f;
        this.zRot = Mathf.Clamp(this.xInput * this.angularSpeed * Time.deltaTime, -25f, 25f);
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
        float d = (Mathf.Abs(this.xInput) > 0f) ? (this.speed * this.turningResponsiveness) : this.speed;
        this.rb.AddForce(this.rotation.forward * d * this.currentDrag, ForceMode.VelocityChange);
    }

    private void Rotation()
    {
        if (Mathf.Sqrt((this.rb.velocity.x * this.rb.velocity.x) + (this.rb.velocity.z * this.rb.velocity.z)) > 20.0f)
            this.rb.AddTorque(base.transform.up * this.angularSpeed * this.xInput);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PlayerInputManager.instance.pickUp)
            return;

        if (other.CompareTag("Rover") && PlayerManager.instance.playerState == PlayerState.BOAT)
        {
            GameObject rover = other.transform.gameObject;
            PlayerManager.instance.currentRover = rover;
            PlayerManager.instance.currentRover.transform.position = PlayerManager.instance.dropPoint.position;
            rover.transform.SetParent(PlayerManager.instance.dropPoint);
            rover.SetActive(false);
        }

        if (other.CompareTag("Bot") && PlayerManager.instance.playerState == PlayerState.BOAT)
        {
            Destroy(other.gameObject);
            botCount++;
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