using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;

public class Player : MonoBehaviour
{

    [Header("Game Information")]

    public bool IsPaused = false;

    [Header("Player Information")]

    public float NormalSpeed;

    public float WalkSpeed;
    public float SprintSpeed;
    

    public Transform PlayerOrientation;

    float horizontalInput;
    float verticalInput;


    Vector3 MoveDirection;
    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //Sprint();
        MyInput();
        SpeedLimit();

        
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
            Debug.Log("Left Shift key is being pressed");
            NormalSpeed = SprintSpeed; //changes player speed to sprinting
        }
        else
        {
            NormalSpeed = WalkSpeed; //sets player speed back to walking
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
