using Mirror;
using UnityEngine;
using UnityEngine.Audio; // Dodajemy to, aby obsługiwać Mixer
using System.Collections;

public class InteractSound : NetworkBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip[] rustleSounds; 
    [Range(0, 1)] [SerializeField] private float volume = 0.5f;
    [SerializeField] private float soundRadius = 15f;
    [SerializeField] private AudioMixerGroup sfxGroup; // Pole na Twój mixer SFX

    [Header("Visual Juice")]
    [SerializeField] private bool shakeOnEnter = true;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;

    private AudioSource audioSource;
    private Vector3 originalPosition;
    private bool isShaking = false;

    void Start()
    {
        originalPosition = transform.position;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // PRZYPISANIE DO MIXERA
        if (sfxGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxGroup;
        }

        audioSource.spatialBlend = 1.0f; 
        audioSource.maxDistance = soundRadius;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkIdentity ni = other.GetComponent<NetworkIdentity>();
            if (ni != null && ni.isLocalPlayer)
            {
                CmdRequestEffects();
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdRequestEffects()
    {
        RpcPlayEffects();
    }

    [ClientRpc]
    private void RpcPlayEffects()
    {
        if (rustleSounds.Length > 0)
        {
            AudioClip randomClip = rustleSounds[Random.Range(0, rustleSounds.Length)];
            audioSource.PlayOneShot(randomClip, volume);
        }

        if (shakeOnEnter && !isShaking)
        {
            StartCoroutine(ShakeObject());
        }
    }

    private IEnumerator ShakeObject()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float z = Random.Range(-1f, 1f) * shakeIntensity;

            transform.position = originalPosition + new Vector3(x, 0, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        isShaking = false;
    }
}