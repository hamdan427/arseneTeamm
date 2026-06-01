using UnityEngine;

/// <summary>
/// Manages SFX playback for the level.
/// - clock: loops automatically from game start.
/// - blink: plays once per blink event (triggered externally by BlinkEffect).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [Tooltip("Looping clock sound played throughout the entire game session.")]
    public AudioClip clockClip;

    [Tooltip("One-shot sound played once each time the player blinks.")]
    public AudioClip blinkClip;

    [Header("Volume")]
    [Range(0f, 1f)] public float clockVolume = 0.6f;
    [Range(0f, 1f)] public float blinkVolume = 1f;

    private AudioSource clockSource;
    private AudioSource blinkSource;

    private void Awake()
    {
        // Use the required AudioSource for clock.
        clockSource = GetComponent<AudioSource>();
        clockSource.clip = clockClip;
        clockSource.loop = true;
        clockSource.volume = clockVolume;
        clockSource.playOnAwake = false;

        // Add a second AudioSource for blink (one-shot, no loop).
        blinkSource = gameObject.AddComponent<AudioSource>();
        blinkSource.loop = false;
        blinkSource.playOnAwake = false;
        blinkSource.volume = blinkVolume;
    }

    private void Start()
    {
        if (clockClip != null)
            clockSource.Play();
        else
            Debug.LogWarning("[SFXManager] clockClip is not assigned.");

        if (blinkClip == null)
            Debug.LogWarning("[SFXManager] blinkClip is not assigned.");
    }

    /// <summary>
    /// Plays the blink SFX once. Call this on the rising edge of a blink event.
    /// </summary>
    public void PlayBlink()
    {
        if (blinkClip == null)
            return;

        // PlayOneShot allows overlapping if needed, but for a single blink it's clean.
        blinkSource.PlayOneShot(blinkClip, blinkVolume);
    }

    /// <summary>Pauses the clock sound.</summary>
    public void PauseClock() => clockSource.Pause();

    /// <summary>Resumes the clock sound.</summary>
    public void ResumeClock() => clockSource.UnPause();

    /// <summary>Stops the clock sound.</summary>
    public void StopClock() => clockSource.Stop();
}
