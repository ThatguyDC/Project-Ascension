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
    public uiManager uiManagerScript;
    public Ladder LadderScript;


    [Header("Game Information")]
    //Game State
    public bool IsPaused = false;

    

    [Header("Player Information")]

    public CharacterController PlayerController;
    public GameObject Player;
    public Rigidbody rb;

    

    

    public float NormalSpeed;
    public float WalkSpeed = 50f;
    public float SprintSpeed = 65f;
    public float ClimbSpeed = 3f; //Initial climbing speed
    public float TargetClimbSpeed = 0f; //Speed to reduce to 
    private float SpeedReductionTime = 5f;  // Time over which to reduce the speed

 
    private float StopMovementDelay = 5f; //Time that speed is set to zero in StopMove function

    public bool Sprinting;
    public bool Climbing;
    

    public float TurnSmoothing = 0.1f;
    public float TurnSmoothVelocity;
    public Vector3 Direction;

    //Grounding Info

    public LayerMask GroundMask; // Layer mask to specify what layers are considered ground
    public bool IsGrounded; // Check if the player is on the ground
    public GameObject GroundCheckObj;
   

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
    


    [SerializeField] private float SpawnOffsetX = 4f;
    [SerializeField] private float SpawnOffsetY = 0f;
    [SerializeField] private float SpawnOffsetZ = 4f;

    [SerializeField] private bool CarryingObject;


    private void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        
        AnimatePlayer();
        DropObject();
        GroundCheck();




    }

    void FixedUpdate()
    {
        Move();
        Sprint();
        
    }

    private void OnDrawGizmos()
    {
        DebugCollider();

    }

    #region Colliders
    

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

    

    #endregion

    #region Triggers
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

        if(other.tag == "Ladder")
        {
            Debug.Log("Hitting Ladder");
            Climbing = true;
            Debug.Log(Climbing);
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Ladder")
        {
            Climbing = false;
            Move();
        }
    }

    #endregion

    private void GroundCheck()
    {

        IsGrounded = Physics.CheckSphere(GroundCheckObj.transform.position, GroundCheckDistance, GroundMask);

    }
   



    private void Move()
    {
        Cursor.lockState = CursorLockMode.Locked;


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Direction = new Vector3 (horizontal, 0f, vertical).normalized;


        if (!Climbing)
        {

            if (Direction.magnitude >= 0.1f)
            {
                float TargetAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + CameraTransform.eulerAngles.y; //returns an angle amt to turn player clockwise from player forward direction
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref TurnSmoothVelocity, TurnSmoothing);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 MoveDirection = Quaternion.Euler(0f, TargetAngle, 0f) * Vector3.forward;
                PlayerController.Move(MoveDirection.normalized * NormalSpeed * Time.fixedDeltaTime);


            }

            // Apply gravity
            Velocity.y = Gravity * Time.deltaTime;
            PlayerController.Move(Velocity * Time.deltaTime);

        }
        
        else
        {
            Climb();
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

    #region Climbing
    private void Climb()
    {
        // When climbing, halt forward movement
        Debug.Log("Player is climbing, halting forward movement");


        Vector3 ClimbDirection = Vector3.up; // Move upwards
        PlayerController.Move(ClimbDirection.normalized * ClimbSpeed * Time.fixedDeltaTime);

         

        // Log the climbing event (optional for debugging)
        Debug.Log("Player is climbing...");
        
    }

    // Coroutine to smoothly transition back to normal gravity
    private IEnumerator SmoothExitFromClimb()
    {
        float transitionTime = 0.5f;  // Time to transition back to normal state
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;

            // Gradually reduce climbing velocity to a normal fall
            ClimbSpeed = Mathf.Lerp(ClimbSpeed, TargetClimbSpeed, elapsedTime);

            yield return null; // Wait for the next frame
        }

        // Re-enable gravity and reset velocity
        rb.useGravity = true;
    }

    #endregion

    #region Player Animation
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
         if (Sprinting)
        {
            PlayerAnimator.SetFloat("Speed", 1);
            //Debug.Log("Sprint");
        }

         if (!IsGrounded)
        {
            PlayerAnimator.SetBool("Falling", true); //Idle falling anim plays
        }

         if (IsGrounded)
        {
            PlayerAnimator.SetBool("Falling", false); //hard landing anim plays
            
            


        }





    }
    #endregion


    #region Object Interaction
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

    #endregion

}
