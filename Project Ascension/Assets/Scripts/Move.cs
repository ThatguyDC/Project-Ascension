using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject thing;
    public Rigidbody rb;

    public float speed = 0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveThing();
        SpeedLimit();
    }

    private void MoveThing()
    {
        thing.transform.position = Vector3.zero;
    }

    private void SpeedLimit()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > speed) //if current velocity is higher han move speed, limit velocity to the MoveSpeed
        {
            Vector3 limitedVel = flatVel.normalized * speed; //calculate what your max velocity would be with this limit applied 
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); //rigid body's velocity is set to the limited velocity value
        }
    }

}
