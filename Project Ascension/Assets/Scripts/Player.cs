using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;

public class Player : MonoBehaviour
{
    [Header("Script Comms")]

    public Objective ObjectiveScript;
    public AudioManager AudioManagerScript;


    [Header("Game Information")]

    //Game State
    public bool IsPaused = false;

    [Header("Player Information")]

    //Movement
    public float NormalSpeed;
    public float WalkSpeed;
    public float SprintSpeed;

    [SerializeField] private bool Sprinting;
    
    //Orientation and Input
    public Transform PlayerOrientation;

    float horizontalInput;
    float verticalInput;


    Vector3 MoveDirection;
    Rigidbody rb; 

    //Animations
    private Animator PlayerAnimator;

    //Audio

    public AudioSource PlayerAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerAnimator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update() //used for general/game-state updates like input or score, etc.
    {        
        MyInput();
        SpeedLimit();
        AnimatePlayer();
    }

    void FixedUpdate() //used for physics-based updates like movement
    {
        Move();
        Sprint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EndPoint")
        {
            ObjectiveScript.ObjectiveIndicator.SetActive(false);
            AudioManagerScript.ObjectiveReached(); //play completion sound
        }
    }

    private void AnimatePlayer()
    {
        //Idle
        if (MoveDirection == Vector3.zero)
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

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

    }

    private void Move()
    {
        if (!IsPaused)
        {
            
            //calculate movement direction
            MoveDirection = PlayerOrientation.forward * verticalInput + PlayerOrientation.right * horizontalInput;
            rb.AddForce(MoveDirection.normalized * NormalSpeed, ForceMode.Force);

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

    private void SpeedLimit()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > NormalSpeed) //if current velocity is higher han move speed, limit velocity to the MoveSpeed
        {
            Vector3 limitedVel = flatVel.normalized * NormalSpeed; //calculate what your max velocity would be with this limit applied 
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); //rigid body's velocity is set to the limited velocity value
        }
    }
}
