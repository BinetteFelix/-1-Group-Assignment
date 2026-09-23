using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAudio : MonoBehaviour
{
    #region variables
    private TestPlayerMovement playerMovement;                              
    [SerializeField] private AudioSource sfxAudioSource;                    // Made into SerializeField so the two Audio Sources don't get mixed up     
    [SerializeField] private AudioSource bgmAudioSource;
    public AudioClip jumpSound;                                             // Making a public field so we can add the actual audio file through Inspector.
    public AudioClip landingSound;                                          // Making a public field so we can add the actual audio file through Inspector.
    public AudioClip hardLandingSound;                                      // Public field for another landing sound (when you land with higher downward velocity). 
    public AudioClip[] footstepSounds;                                      // Public field for footstep sounds. Made it into an array so that we can have different footstep sounds playing in randomized order (makes it sound less flat and boring).
    public AudioClip[] gruntSounds;                                         // Public field for grunt sounds. Made as array to play random grunt sound out of three on jump.
    private bool wasGroundedLastFrame;                                      // Making a bool variable that will check for us if the player was grounded last frame or not. We need to know this so we can identify when the player lands after a jump.
    private float lastLandingSoundTime;                                     // Variable for last time a landing sound played so we can have a cooldown check on it.
    private float landingSoundCooldown = 0.2f;                              // The actual cooldown time on the landing time.
    public float hardLandingThreshold = -15f;                               // The value of how much velocity the player must have at the very least to play the hard landing sound.
    private float footstepSoundCd = 0.45f;                                   // Cooldown on when next footstep sound can play so it doesn't play continously without no pause in between actual footsteps. More realistic.
    private float lastFootstepSound;                                        // Variable to check when the last footstep sound was played so that we can actually use the cooldown we created for the footsteps.
    private int lastFootstepIndex = -1;                                     // Variable for storing the last index that was used that plays one of the audio files for footstep sound. Starts as -1 so that we don't accidentally play the index with value 0 twice in the beginning.
    private int lastGruntIndex = -1;                                        // Same idea as the starting value of index for footstep sounds.
    private float peakFallVelocity;
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       playerMovement = GetComponent<TestPlayerMovement>();
    }


    private void FixedUpdate()                                                                             // Fix for hard landing sound not playing at high speeds.
    {                                                                                                      // New check of velocity in FixedUpdate() to have ground check in sync with velocity check (both are in FixedUpdate now) 
        if (!playerMovement.IsGrounded && playerMovement.VerticalVelocity < peakFallVelocity)
        {
            peakFallVelocity = playerMovement.VerticalVelocity;
        }
    }
    // Update is called once per frame
    void Update()                                       
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && playerMovement.IsGrounded)                    // Gets the key press of spacebar in the first frame is was pressed. Even if spacebar is held down after the key press, it won't register the press multiple times.              
        {
            sfxAudioSource.PlayOneShot(jumpSound);                                                         // Plays the sound of the jump when the spacebar key is pressed down without it being interrupted by new spacebar inputs or interuppting previous spacebar input.
                                                                                                           // Sound basically doesn't restart on a new spacebar input.
            int randomGrunt = Random.Range(0, gruntSounds.Length);                                         // Getting random index to play of three grunt sounds when jumping.               
            while (randomGrunt == lastGruntIndex)                                                          // While loop to make sure that the same grunt sounds doesn't play twice in a row.
            {
                randomGrunt = Random.Range(0, gruntSounds.Length);                                         // If the same index gets picked again, then it picks another index again randomly. Continues looking for different index so same grunt sound is not played twice. 
            }
            sfxAudioSource.PlayOneShot(gruntSounds[randomGrunt], 0.6f);                                    // Playing grunt sounds through Audio Source.
            lastGruntIndex = randomGrunt;                                                                  // Remembering which grunt sound was played last by equaling it to the value of the grunt sound that was just played.
            
        }                                                                     
        
        float timeSinceLastLanding = Time.time - lastLandingSoundTime;
        if (!wasGroundedLastFrame && playerMovement.IsGrounded && timeSinceLastLanding >= landingSoundCooldown)         /* The variable wasGroundedLastFrame is by default a false since I didn't explicitly say if it was false or true when creating it. */
        {                                                                                                               /* We check if it stays false and also if the isGrounded variable turns to true. As soon as it matches up it plays the sound in that frame. */
                                                                                                                        /* Added a cooldown for landing sound so it does trigger multiple times when ground check flickers (raycasting being unreliable). */
        if (peakFallVelocity < hardLandingThreshold)                                                     /* Wanted to add a "harder landing sound" when landing from a specific height (or specific velocity).*/
            {                                                                                                           /* Checks how fast the player is going vertically and plays a harder land sound if player is falling from a higher place.*/
                sfxAudioSource.PlayOneShot(hardLandingSound);                                                           // Plays hard landing sound if conditions are met.
            }
            else
            {
                sfxAudioSource.PlayOneShot(landingSound);               // We call on PlayOneShot to play the sound once using the Audio Source reference and playing the audio file that is in the Audio Clip field.
            }
            lastLandingSoundTime = Time.time;                                 // Change the variable lastLandingSoundTime to equal the current time that has passed so we can correctly check how much time has passed since the last time the landing sound played.
            peakFallVelocity = 0;
                
        }
        wasGroundedLastFrame = playerMovement.IsGrounded;                     // We change the value of wasGroundedLastFrame to the same value as the player isGrounded check to not play the landing sound outside of cases where the player isn't landing from a jump.

        float timeSinceLastFootstep = Time.time - lastFootstepSound;
        if (playerMovement.IsMoving && playerMovement.IsGrounded && timeSinceLastFootstep >= footstepSoundCd)           // Checks if player is moving, is grounded and if enough time has elapsed since last time the footstep sound was played (so it doesn't spam the sound).
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);           // Made a dynamic array that can check how many indeces we have within the array dependent on how many audio files we input through the Inspector. Stores the index value in randomIndex.
            while (randomIndex == lastFootstepIndex)                            // Created a while loop so we can check which sound out of the 4 elements in the array was played last. Doing this to we don't get any repeat sounds.
            {
                 randomIndex = Random.Range(0, footstepSounds.Length);          // If the statement in the while loop stays true than we continue looking for another random index until it's not the same.
            }

            sfxAudioSource.PlayOneShot(footstepSounds[randomIndex]);      // Plays the audio file of the random index that was selected.
            lastFootstepSound = Time.time;                                      // Resets the timer for the last played footstep variable. Basically makes out cooldown always valid since it now counts from when last the sound was played and not from 0 like in the beginning.
            lastFootstepIndex = randomIndex;                                    // Setting the value of the randomIndex (that just played) to the last played index variable. Next time it checks for repeat sounds it will know which one was played last
        }
    }
    
}
