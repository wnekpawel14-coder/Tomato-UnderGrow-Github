using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    [Header("Playlist")]
    [SerializeField] private List<AudioClip> playlist = new List<AudioClip>();
    [SerializeField] private bool shuffle = true;

    [Header("Settings")]
    [SerializeField] private AudioMixerGroup musicGroup;
    [Range(0, 1)] [SerializeField] private float volume = 0.3f;

    private AudioSource audioSource;
    private int currentTrackIndex = -1;

    void Start()
    {
        // Konfiguracja AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = musicGroup;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; // Muzyka 2D (gra wszędzie tak samo)
        audioSource.volume = volume;

        if (playlist.Count > 0)
        {
            StartCoroutine(PlayMusicRoutine());
        }
    }

    IEnumerator PlayMusicRoutine()
    {
        while (true)
        {
            PlayNextTrack();
            // Czekamy aż piosenka się skończy + sekunda przerwy
            yield return new WaitForSeconds(audioSource.clip.length + 1f);
        }
    }

    void PlayNextTrack()
    {
        if (shuffle)
        {
            currentTrackIndex = Random.Range(0, playlist.Count);
        }
        else
        {
            currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
        }

        audioSource.clip = playlist[currentTrackIndex];
        audioSource.Play();
        Debug.Log("Gra teraz: " + audioSource.clip.name);
    }

    // Funkcja do zmiany głośności (np. z menu pauzy)
    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        audioSource.volume = volume;
    }
}