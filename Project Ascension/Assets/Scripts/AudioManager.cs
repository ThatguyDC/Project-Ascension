using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    [Header("Script Comms")]

    [SerializeField] private Player PlayerScript;


    [Header("Audio Sources")]

    [SerializeField] public AudioSource PlayerAudio;


    [Header("Audio Clips")]

    [Header("Sound Effects")]

    public AudioClip ObjectiveCompleteSound;

    [Header("Music")]

    public AudioClip LevelAmbience;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ObjectiveReached()
    {
        PlayerAudio.PlayOneShot(ObjectiveCompleteSound, 1f);
    }
}
