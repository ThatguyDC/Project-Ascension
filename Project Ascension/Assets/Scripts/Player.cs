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

    //Pickups and Carrying

    public GameObject ObjectToPickUp;
    public GameObject SpawnObject;
    public GameObject RefPoint;

    [SerializeField] private bool CarryingObject;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerAnimator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update() //used for general/game-state updates like input or score, etc.
    {        
        SpeedLimit();
        AnimatePlayer();
    }

    void FixedUpdate() //used for physics-based updates like movement
    {
        MyInput();
        Move();
        Sprint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EndPoint")
        {
            ObjectiveScript.ObjectiveIndicator.SetActive(false);
            ObjectiveScript.ObjectiveReached();
            AudioManagerScript.ObjectiveReached(); //play completion sound
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6 && CarryingObject == false) //if object is on layer 6, aka PickupLayer, then do stuff
        {
            //show interaction keybind to pick up object 
            //pick up object
            //teleport object to player's hand at the ref point
            //deactivate trigger on pickup

            SpawnObject = other.gameObject.GetComponent<Pickup>().ObjectToSpawn;
            RefPoint = other.gameObject.GetComponent<Pickup>().PickupRefPoint;
            PickUpObject(SpawnObject, RefPoint); //passes Pickup spawn object to function, spawns object in player's hand

        }
    }

    private void AnimatePlayer()
    {
        //Idle
        if (MoveDirection == Vector3.zero && CarryingObject != true)
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

        //Carry Idle
        if (MoveDirection == Vector3.zero && CarryingObject == true)
        {
            PlayerAnimator.SetFloat("Speed", 0); //sets animator's PlayerSpeed parameter to zero, playing idle anim
            //Debug.Log("Idle");

        }
        //Carry Walk

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

    public void PickUpObject(GameObject ObjectToSpawn, GameObject RefPoint) //typeOf GameObject to indicate what is being passed through
    {
        if (ObjectToSpawn != null && RefPoint != null && Input.GetKeyDown(KeyCode.E)) //if the params are filled and E is pressed, 
        {
        Instantiate(ObjectToSpawn, RefPoint.transform.position, RefPoint.transform.rotation); //spawn passed obj at passed ref point's position and rotation
        }
        else
        {
            Debug.Log("No input detected");
        }
    }
}
