using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SceneMusic : MonoBehaviour
{
    [Header("Musique")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioMixerGroup musicMixerGroup; 

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (musicMixerGroup != null)
            audioSource.outputAudioMixerGroup = musicMixerGroup;
    }

    private void Start()
    {
        if (musicClip != null)
        {
            audioSource.Play();
        }
    }
}