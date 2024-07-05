using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    [Header("Script Comms")]

    private Player PlayerScript;
    private AudioManager AudioManagerScript;

    [Header("Player Detection")]

    [SerializeField] private GameObject Player;
    [SerializeField] private Collider PlayerCollider;

    [SerializeField] private Collider StartPoint;
    [SerializeField] private Collider EndPoint;

    [Header("Indicators")]

    [SerializeField] public GameObject ObjectiveIndicator;

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
        AudioManagerScript.PlayerAudio.PlayOneShot(AudioManagerScript.ObjectiveComplete, 1f);
    }


}
