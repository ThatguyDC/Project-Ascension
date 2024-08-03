using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    [Header("Script Comms")]

    private PlayerTestScript PlayerScript;
    [SerializeField] private GameObject Player;

    [Header("Object Information")]



    //Since this script will be applied to any pickup object,
    //these gameObjects will apply to each object that use the script.
    //For example, a cube would need a ref point, and a dupe cube to spawn. 
    //A sphere could use the same ref point, and have a dupe sphere replace the dupe cube instead. 

    public GameObject PickupSourceObj; //The original Pickup object to be destroyed once interacted with
    public GameObject PickupRefPoint; //spawn point for picked up object
    public GameObject ObjectToSpawn; //Object to instantiate in hand on pickup



    void Start()
    {
        PlayerScript = Player.GetComponent<PlayerTestScript>(); //gets player script from player game object
    }

   
    
}

