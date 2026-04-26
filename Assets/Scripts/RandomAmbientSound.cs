using Mirror;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class RandomAmbientSound : NetworkBehaviour
{
    [Header("Sound Selection")]
    [SerializeField] private AudioClip[] ambientClips; // Tu wrzuć np. kilka różnych ćwierkań
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Timing (Seconds)")]
    [SerializeField] private float minWaitTime = 10f; // Minimalna przerwa
    [SerializeField] private float maxWaitTime = 30f; // Maksymalna przerwa

    [Header("Settings")]
    [Range(0, 1)] [SerializeField] private float volume = 0.4f;
    [SerializeField] private float soundRadius = 20f;
    [SerializeField] private bool playOnStart = true;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (sfxGroup != null) audioSource.outputAudioMixerGroup = sfxGroup;

        // Ustawienia 3D
        audioSource.spatialBlend = 1.0f; 
        audioSource.maxDistance = soundRadius;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.playOnAwake = false;

        // Tylko serwer zarządza czasem, żeby wszyscy słyszeli to samo jednocześnie
        if (isServer)
        {
            StartCoroutine(SoundRoutine());
        }
    }

    private IEnumerator SoundRoutine()
    {
        if (playOnStart) yield return new WaitForSeconds(Random.Range(1f, 5f));

        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // Serwer wysyła sygnał do wszystkich klientów
            RpcPlayAmbientSound();
        }
    }

    [ClientRpc]
    private void RpcPlayAmbientSound()
    {
        if (ambientClips.Length > 0)
        {
            AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
            audioSource.PlayOneShot(clip, volume);
        }
    }
}