using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 25f;
    [SerializeField] private ConfigurableJoint hipJoint;
    [SerializeField] private Rigidbody hip;
    [SerializeField] private float jumpforce = 200f;
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private float fallMultiplier = 20f;
    [SerializeField] private float distanceBeforeFall = 1f;
    [SerializeField] private int fallBeforeRagdoll = 15;
    [SerializeField] private int ragdollfallMultiplier = 2;
    [SerializeField] private float jumpCD = 1f;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private float resetCD = 1.5f;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource grabSound;
    [SerializeField] private AudioSource resetSound;
    [SerializeField] private AudioSource checkpointSound;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private Text jumpText;
    [SerializeField] private Text grabText;
    [SerializeField] private Text resetText;
    [SerializeField] private Text fallText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text maxHeightText;

    private bool walk = false;
    public bool isRagdoll = false;
    private int ragdollCount = 0;
    private bool isJumping = false;
    private Camera cam;
    private int fallTime = 0;
    private float timeSinceJump = 0;
    private float currentX = 0f;
    private float currentY = 0f;
    public List<bool> isGrabbing;
    private float holdResetTime;
    private Vector3 originalLocation;
    public bool isBeaming = false;
    private bool showGameMenu = false;
    private float timer = 0;
    private float jumpCount = 0;
    private float grabCount = 0;
    private float resetCount = 0;
    private float fallCount = 0;
    private float maxHeight = 0;

    private List<Rigidbody> playerRB = new List<Rigidbody>();
    private List<ConfigurableJoint> playerJoints = new List<ConfigurableJoint>();

    void Start()
    {
        originalLocation = hip.transform.position;
        isGrabbing = new List<bool>() { false, false };
        cam = Camera.main;

        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (gameObject.GetComponent<Rigidbody>() != null)
            {
                playerRB.Add(gameObject.GetComponent<Rigidbody>());
                playerJoints.Add(gameObject.GetComponent<ConfigurableJoint>());
            }
        }
    }

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X");
        currentY -= Input.GetAxis("Mouse Y");
        currentY = Mathf.Clamp(currentY, 20, 55);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (showGameMenu)
            {
                HideGameMenu();
            }
            else
            {
                ShowGameMenu();
            }
        }

        if (isBeaming || showGameMenu)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isRagdoll)
            {
                if (ragdollCount > 0)
                    ragdollCount--;
                else
                    UnRagdoll();
            }
            else if (!isJumping && timeSinceJump > jumpCD)
            {
                Jump();
            }
        }

        if (Input.GetKey(KeyCode.E))
            Ragdoll();
    }

    private void FixedUpdate()
    {
        UpdateText();

        if (isBeaming)
        {
            hip.AddForce(Physics.gravity * -1 * fallMultiplier, ForceMode.Acceleration);
            return;
        }

        if (showGameMenu)
            return;

        timer += Time.fixedDeltaTime;
        timeSinceJump += Time.fixedDeltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 camVertical = cam.transform.forward;
        Vector3 camHorizontal = cam.transform.right;
        camVertical.y = 0f;
        camHorizontal.y = 0f;
        float curHeight = hip.position.y;
        Vector3 direction = (camVertical * vertical + camHorizontal * horizontal).normalized;

        if (curHeight > maxHeight)
            maxHeight = curHeight;

        if (direction.magnitude >= 0.1f && !isRagdoll)
        {
            float targetAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            this.hipJoint.targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

            this.hip.AddForce(direction * this.speed);

            this.walk = true;
        }
        else
        {
            this.walk = false;
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    checkpointSound.Play();
                    originalLocation = hip.transform.position;
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
            holdResetTime += Time.fixedDeltaTime;
        else
            holdResetTime = 0;

        if (holdResetTime > resetCD)
        {
            holdResetTime = 0;
            hip.transform.position = originalLocation;
            resetSound.Play();
            resetCount++;
        }

        this.targetAnimator.SetBool("Walk", this.walk);

        UpdateText();

        hip.AddForce(Physics.gravity * fallMultiplier * (isRagdoll ? ragdollfallMultiplier : 1), ForceMode.Acceleration);
        if (hip.velocity.y < 0)
        {
            fallTime++;
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(hip.position, -Vector3.up, out hit))
            {
                float distanceToGround = hit.distance;

                if (distanceToGround > distanceBeforeFall)
                {
                    if (!isJumping && fallTime >= fallBeforeRagdoll)
                        Ragdoll();
                }
            }
        }
        else
        {
            fallTime = 0;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Vector3 dir = new Vector3(0, 3, -5);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

            if (Input.GetMouseButton(1))
                leftArm.transform.LookAt(leftArm.transform.position + rotation * dir);

            if (Input.GetMouseButton(0))
                rightArm.transform.LookAt(rightArm.transform.position + rotation * dir);
        }
    }

    public void SetGrounded(GameObject groundedObject)
    {
        if (isJumping)
            timeSinceJump = 0;

        isJumping = false;
    }

    private void ShowGameMenu()
    {
        gameMenu.SetActive(true);
        showGameMenu = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideGameMenu()
    {
        gameMenu.SetActive(false);
        showGameMenu = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetTripping(GameObject gameObject)
    {
        if (!isJumping)
        {
            Ragdoll();
        }
    }

    private void Jump()
    {
        isJumping = true;
        this.hip.AddForce(new Vector3(0, jumpforce, 0), ForceMode.Impulse);
        jumpSound.Play();
        jumpCount++;
    }

    public void SetGrabbing()
    {
        grabCount++;
        grabSound.Play();
    }

    private void Ragdoll()
    {
        if (!isRagdoll && !isGrabbing[0] && !isGrabbing[1])
        {
            isJumping = false;
            isRagdoll = true;
            ragdollCount = UnityEngine.Random.Range(2, 6);
            fallCount++;

            playerRB.ForEach(r => r.mass = 10);
            foreach (ConfigurableJoint joint in playerJoints)
            {
                JointDrive limpDrive = new JointDrive() { positionSpring = joint.angularXDrive.positionSpring / 8, maximumForce = joint.angularXDrive.maximumForce, positionDamper = 0 };
                joint.angularXDrive = limpDrive;
                joint.angularYZDrive = limpDrive;
            }
        }
    }

    private void UnRagdoll()
    {
        if (isRagdoll)
        {
            isRagdoll = false;
            playerRB.ForEach(r => r.mass = 1);

            foreach (ConfigurableJoint joint in playerJoints)
            {
                JointDrive limpDrive = new JointDrive() { positionSpring = joint.angularXDrive.positionSpring * 8, maximumForce = joint.angularXDrive.maximumForce, positionDamper = 0 };
                joint.angularXDrive = limpDrive;
                joint.angularYZDrive = limpDrive;
            }

            Jump();
        }
    }

    private void UpdateText()
    {
        TimeSpan ts = TimeSpan.FromSeconds(timer);
        jumpText.text = $"Jumps: {jumpCount}";
        grabText.text = $"Grabs: {grabCount}";
        resetText.text = $"Resets: {resetCount}";
        fallText.text = $"Falls: {fallCount}";
        maxHeightText.text = $"Max Height: {(int)maxHeight}ft";
        timerText.text = $"Timer: {ts.ToString(@"hh\:mm\:ss")}";
    }
}
