using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [Header("Ustawienia Muzyki")]
    public AudioSource audioSource;
    public AudioClip[] musicTracks; // Tutaj wrzucisz swoje 3 utwory

    private int currentTrackIndex = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (musicTracks.Length > 0)
        {
            PlayTrack(currentTrackIndex);
        }
    }

    private void Update()
    {
        // Sprawdzamy, czy muzyka przesta³a graæ i czy mamy jakieœ utwory na liœcie
        if (!audioSource.isPlaying && musicTracks.Length > 0)
        {
            PlayNextTrack();
        }
    }

    private void PlayNextTrack()
    {
        // Zwiêkszamy indeks o 1. Reszta z dzielenia (%) sprawia, ¿e po 3 utworze wrócimy do 0.
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        PlayTrack(currentTrackIndex);
    }

    private void PlayTrack(int index)
    {
        audioSource.clip = musicTracks[index];
        audioSource.Play();
    }
}