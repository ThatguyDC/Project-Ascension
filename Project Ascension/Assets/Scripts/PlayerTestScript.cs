using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTestScript : MonoBehaviour
{

    [Header("Script Comms")]

    public Objective ObjectiveScript;
    public AudioManager AudioManagerScript;


    [Header("Game Information")]
    //Game State
    public bool IsPaused = false;

    [Header("Player Information")]

    public CharacterController PlayerController;

    public float NormalSpeed;
    public float WalkSpeed = 50f;
    public float SprintSpeed = 65f;

    public bool Sprinting;

    public float TurnSmoothing = 0.1f;
    public float TurnSmoothVelocity;
    public Vector3 Direction;


    //Animations
    private Animator PlayerAnimator;

    //Audio

    public AudioSource PlayerAudioSource;

    [Header("Camera Information")]

    public Transform CameraTransform; //assign main camera to this, not PlayerCam


    private void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        
        AnimatePlayer();
    }
    void FixedUpdate()
    {
        Move();
        Sprint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EndPoint")
        {
            ObjectiveScript.ObjectiveIndicator.SetActive(false);
            ObjectiveScript.ObjectiveReached();
            AudioManagerScript.ObjectiveReached(); //play completion sound
        }
    }

    private void Move()
    {
        Cursor.lockState = CursorLockMode.Locked;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Direction = new Vector3 (horizontal, 0f, vertical).normalized;

        if (Direction.magnitude >= 0.1f)
        {
            float TargetAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + CameraTransform.eulerAngles.y; //returns an angle amt to turn player clockwise from player forward direction
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref TurnSmoothVelocity, TurnSmoothing);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 MoveDirection = Quaternion.Euler(0f, TargetAngle, 0f) * Vector3.forward;
            PlayerController.Move(MoveDirection.normalized * NormalSpeed * Time.fixedDeltaTime);
        }
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("Sprinting");
            NormalSpeed = SprintSpeed; //changes player speed to sprinting
            Sprinting = true;
        }
        else
        {
            NormalSpeed = WalkSpeed; //sets player speed back to walking
            Sprinting = false;
        }
    }

    private void AnimatePlayer()
    {
        //Idle
        if (Direction == Vector3.zero)
        {
            PlayerAnimator.SetFloat("Speed", 0); //sets animator's PlayerSpeed parameter to zero, playing idle anim
            //Debug.Log("Idle");

        }
        //Walk
        else if (!Sprinting)
        {
            PlayerAnimator.SetFloat("Speed", 0.4f);
            //Debug.Log("Walk");

        }
        //Sprint
        else
        {
            PlayerAnimator.SetFloat("Speed", 1);
            //Debug.Log("Sprint");
        }

    }
}
