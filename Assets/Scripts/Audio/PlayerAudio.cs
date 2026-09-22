using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    #region variables                            
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;
    #endregion

    public void PlaySFXAudio(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);
    }
}
