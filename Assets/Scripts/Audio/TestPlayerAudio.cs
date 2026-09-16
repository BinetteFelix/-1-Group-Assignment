using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAudio : MonoBehaviour
{

    private AudioSource jumpingAudioSource;                     // Created a variable to store the audio in. Will be used in Start() to store the audio source in this variable to be used in the script.
    public AudioClip jumpSound;                                 // Making a public class so we can add the actual audio file through Inspector.


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jumpingAudioSource = GetComponent<AudioSource>();                   // Getting the audio from components and storing it in the "jumpingAudioSource" variable.
        
    }

    // Update is called once per frame
    void Update()                                       
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)                    // Gets the key press of spacebar in the first frame is was pressed. Even if spacebar is held down after the key press, it won't register the press multiple times.              
        {
            jumpingAudioSource.PlayOneShot(jumpSound);          // Plays the sound of the jump when the spacebar key is pressed down without it being interrupted by new spacebar inputs or interuppting previous spacebar input.
        }                                                       // Sound basically doesn't restart on a new spacebar input.
        
    }
}
