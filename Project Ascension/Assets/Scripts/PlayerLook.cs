using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Camera Sensitivity")]


    public float sensX;
    public float sensY;

    [Header("Rotation Speeds")]

    float xRotation;
    float yRotation;

    [Header("Game Objects")]

    public Transform FollowTarget;
    public Transform Player;


    [Header("Script Comms")]

    public Player PlayerScript;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        MouseLook();
    }

    private void MouseLook()
    {
        if (!PlayerScript.IsPaused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -33f, 70f); //clamps rotation of camera when looking up and down

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            FollowTarget.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            Player.rotation = Quaternion.Euler(0, yRotation, 0);

        }
        else
        {

        }

    }

    private void CameraShake()
    {
        //Use Cinemachine's Impulse Listeners and Sources for this concept, it integrates way easier into the camera's motion.
    }
}
