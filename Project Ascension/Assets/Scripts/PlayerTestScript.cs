using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
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
    public GameObject Player;

    

    public float NormalSpeed;
    public float WalkSpeed = 50f;
    public float SprintSpeed = 65f;

    public bool Sprinting;

    public float TurnSmoothing = 0.1f;
    public float TurnSmoothVelocity;
    public Vector3 Direction;

    //Grounding Info

    public LayerMask GroundMask; // Layer mask to specify what layers are considered ground
    public bool IsGrounded; // Check if the player is on the ground

    private Vector3 Velocity; // Current velocity of the player
    private float Gravity = -9.81f;
    public float GroundCheckDistance = 0.2f; // Distance to check if the player is grounded


    //Animations
    private Animator PlayerAnimator;

    //Audio

    public AudioSource PlayerAudioSource;

    [Header("Camera Information")]

    public Transform CameraTransform; //assign main camera to this, not PlayerCam



    //Pickups and Carrying

    private GameObject SpawnedObj;
    private GameObject ObjectToPickUp;
    private GameObject SpawnObject;
    private GameObject RefPoint;
    private GameObject SourceObj;

    [Header("Pickup Information")]
    /*
    public GameObject SpawnedObj;
    public GameObject ObjectToPickUp;
    public GameObject SpawnObject;
    public GameObject RefPoint;
    public GameObject SourceObj;
    */


    [SerializeField] private float SpawnOffsetX = 4f;
    [SerializeField] private float SpawnOffsetY = 0f;
    [SerializeField] private float SpawnOffsetZ = 4f;

    [SerializeField] private bool CarryingObject;


    private void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        
        AnimatePlayer();
        DropObject();


    }

    private void OnDrawGizmos()
    {
        DebugCollider();

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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6 && CarryingObject == false) //if object is on layer 6, aka PickupLayer, then do stuff
        {
            Debug.Log("Object on PickupLayer detected");

            //show interaction keybind to pick up object 
            //pick up object
            //deactivate trigger on pickup


            SpawnObject = other.GetComponent<Pickup>().ObjectToSpawn;
            RefPoint = other.gameObject.GetComponent<Pickup>().PickupRefPoint;
            SourceObj = other.gameObject.GetComponent<Pickup>().PickupSourceObj;
            PickUpObject(SpawnObject, RefPoint, SourceObj); //passes Pickup spawn object and ref point, spawns given object in player's hand at ref point

        }

        
    }

    private void DebugCollider()
    {
        CharacterController characterController = Player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            // Draw a wireframe of the collider bounds
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(characterController.bounds.center, characterController.bounds.size);
        }
    }

    private void Move()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Ground check
        IsGrounded = Physics.CheckSphere(transform.position, GroundCheckDistance, GroundMask);
        if (IsGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f; // Ensures the player sticks to the ground
        }

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

        // Apply gravity
        Velocity.y += Gravity * Time.deltaTime;
        PlayerController.Move(Velocity * Time.deltaTime);
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

    public void PickUpObject(GameObject ObjectToSpawn, GameObject RefPoint, GameObject SourceObj) //typeOf GameObject to indicate what is being passed through
    {
        if (CarryingObject == false && ObjectToSpawn != null && RefPoint != null && SourceObj != null && Input.GetKeyDown(KeyCode.E)) //if the params are filled and E is pressed, 
        {
            SpawnedObj = Instantiate(ObjectToSpawn, RefPoint.transform.position, RefPoint.transform.rotation); //spawn passed obj at passed ref point's position and rotation
            SpawnedObj.transform.SetParent(RefPoint.transform); //set spawned object's parent as RefPoint
            CarryingObject = true;
            SourceObj.SetActive(false);
            
        }

        else if (CarryingObject == true)
        {
            Debug.Log("Already Holding Object");

        }
        else if (ObjectToSpawn == null)
        {
            Debug.Log("Null Spawn Object");

        }
        else if (RefPoint == null)
        {
            Debug.Log("Null Spawn RefPoint");

        }
        else
        {
            Debug.Log("Missing ObjectToSpawn/RefPoint");
        }

    }

    public void DropObject()
    {
        if (CarryingObject == true && SourceObj != null && Input.GetKeyDown(KeyCode.Q))
        {
            SourceObj.SetActive(true); //set active near player
            SourceObj.transform.position = Player.transform.position + new Vector3(SpawnOffsetX, SpawnOffsetY, SpawnOffsetZ); //move hidden Pickup to player with offsets
            CarryingObject = false;

            //Deactivate carried object in player's hand
            Destroy(SpawnedObj);


        }
        else if (SourceObj == null) 
        {
            //Debug.Log("No SourceObj");

        }
        else if (CarryingObject == false)
        {
            //Debug.Log("Carrying is false");

        }

        else
        {
            Debug.Log("No Input / Unknown Error");

        }
    }

}
