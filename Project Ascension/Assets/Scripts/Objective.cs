using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    [Header("Script Comms")]

    private Player PlayerScript;
    private AudioManager AudioManagerScript;

    [Header("Player Detection")]
    //the ability to recognize the player's presence,
    //and the points where they will be detected.
    [SerializeField] private GameObject Player;
    [SerializeField] private Collider PlayerCollider;

    [SerializeField] private Collider StartPoint;
    [SerializeField] private Collider EndPoint;

    [Header("Objective Information")]

    [SerializeField] private bool ObjectiveComplete;

    [Header("Indicators")]
    //the indicator to show where an objective is, or what its goal is.
    [SerializeField] public GameObject ObjectiveIndicator;

    [Header("Objective Options")]

    //this can be an object to activate, a door to remove, etc.
    //attach other desired actions/behaviors to an individual object script
    [SerializeField] private GameObject Door; 
    //[SerializeField] private GameObject Tool; 
    //[SerializeField] private GameObject Note; 


    void Start()
    {

        PlayerScript = Player.GetComponent<Player>(); //gets player script from player game object
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //**Objective Activator Detection**\\

    private void OnTriggerEnter(Collider other) //if the collider is marked as a trigger, use trigger methods
    {
        Debug.Log("Collision detected with: " + other.gameObject.name);

        if (other.gameObject.tag == "Player") { //if the collided object's tag is player,

            Debug.Log("Player collision detected.");
            ObjectiveIndicator.SetActive(true); //activate the indicator object
        }
        
    }

    //**Objective Goal Detection**\\

    public void ObjectiveReached()
    {
        ObjectiveComplete = true;
        Debug.Log("Objective Complete.");
        AudioManagerScript.PlayerAudio.PlayOneShot(AudioManagerScript.ObjectiveCompleteSound, 1f); 
        DeactivateDoor();
        
    }

    public void ActivateDoor() 
    {
        
        Door.SetActive(true);

    }
    public void DeactivateDoor() {
        
        Door.SetActive(false);

    }


}
