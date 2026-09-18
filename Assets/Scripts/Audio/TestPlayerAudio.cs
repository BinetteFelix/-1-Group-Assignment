using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAudio : MonoBehaviour
{
    private TestPlayerMovement playerMovement;                              // Setting variable for Felixs' movement script. Want to reference it in Start() and be able to read the value of isGrounded here so I don't have to rewrite that ground check.
    private AudioSource sfxAudioSource;                                     // Created a variable to reference Audio Source component. Will be used to call on methods in script.
    public AudioClip jumpSound;                                             // Making a public field so we can add the actual audio file through Inspector.
    public AudioClip landingSound;                                          // Making a public field so we can add the actual audio file through Inspector.
    private bool wasGroundedLastFrame;                                      // Making a bool variable that will check for us if the player was grounded last frame or not. We need to know this so we can identify when the player lands after a jump.
    private float lastLandingSoundTime;
    private float landingSoundCooldown = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sfxAudioSource = GetComponent<AudioSource>();                         // Getting the actual reference from components and storing it in the "sfxAudioSource" variable.
        playerMovement = GetComponent<TestPlayerMovement>();                  // Getting the actual reference from compoenents and storing it in the "playerMovement" varibale.
    }

    // Update is called once per frame
    void Update()                                       
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && playerMovement.IsGrounded)                    // Gets the key press of spacebar in the first frame is was pressed. Even if spacebar is held down after the key press, it won't register the press multiple times.              
        {
            sfxAudioSource.PlayOneShot(jumpSound);                            // Plays the sound of the jump when the spacebar key is pressed down without it being interrupted by new spacebar inputs or interuppting previous spacebar input.
        }                                                                     // Sound basically doesn't restart on a new spacebar input.
        
        float timeSinceLastLanding = Time.time - lastLandingSoundTime;
        if (!wasGroundedLastFrame && playerMovement.IsGrounded && timeSinceLastLanding >= landingSoundCooldown)         /* The variable wasGroundedLastFrame is by default a false since I didn't explicitly say if it was false or true when creating it. */
        {                                                                                                               /* We check if it stays false and also if the isGrounded variable turns to true. As soon as it matches up it plays the sound in that frame.
                                                                                                                           Added a cooldown for landing sound so it does trigger multiple times when ground check flickers (raycasting being unreliable). */
            sfxAudioSource.PlayOneShot(landingSound, 0.8f);                   // We call on PlayOneShot to play the sound once using the Audio Source reference and playing the audio file that is in the Audio Clip field.
            lastLandingSoundTime = Time.time;                                 // Change the variable lastLandingSoundTime to equal the current time that has passed so we can correctly check how much time has passed since the last time the landing sound played.
        }
        wasGroundedLastFrame = playerMovement.IsGrounded;                     // We change the value of wasGroundedLastFrame to the same value as the player isGrounded check to not play the landing sound outside of cases where the player isn't landing from a jump.
    }
}
