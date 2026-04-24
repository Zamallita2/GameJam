using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;

    void Awake()
    {
        Instance = this;

        gameObject.SetActive(true);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.enabled = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.mute = false;
        audioSource.loop = false;

        AudioListener.volume = 1f;
        AudioListener.pause = false;

        Debug.Log("[VoiceManager] Awake ejecutado");
    }

    public void PlayVoice(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[VoiceManager] Clip vacío");
            return;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        gameObject.SetActive(true);
        audioSource.enabled = true;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();

        Debug.Log("[VoiceManager] Reproduciendo: " + clip.name + " / AudioSource enabled: " + audioSource.enabled);
    }
}