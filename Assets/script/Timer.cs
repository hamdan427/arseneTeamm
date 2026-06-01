using UnityEngine;
using TMPro;
using UnitEye;

/// <summary>
/// Tracks eye-closed duration using UnitEye's Gaze.Blinking signal.
/// The timer increments whenever eyes are confirmed closed past the confirm delay.
/// </summary>
public class Timer : MonoBehaviour
{
    [Header("References")]
    public Gaze gaze;

    [Header("Detection Settings")]
    [Tooltip("Eyes must stay closed for this many seconds before the timer starts counting. Prevents false triggers from normal blinks.")]
    public float confirmDelay = 0.15f;

    [Header("Timer Behavior")]
    [Tooltip("When true, only the current continuous closed-eye streak is displayed and resets on eye-open. When false, total accumulated closed time is displayed.")]
    public bool trackContinuousOnly = false;

    [Header("Debug")]
    [Tooltip("Log blink state and EAR value every frame to Console.")]
    public bool debugLog = false;

    // Internal state
    private TMP_Text timerText;
    private float confirmTimer = 0f;
    private float continuousClosedTime = 0f;
    private float totalClosedTime = 0f;
    private bool isConfirmedClosed = false;

    private void Start()
    {
        timerText = GetComponent<TMP_Text>();
        UpdateDisplay();
    }

    private void Update()
    {
        if (gaze == null)
            return;

        // gaze.Blinking is set by Gaze.LateUpdate() after a successful inference — safe to read here
        bool rawClosed = gaze.Blinking;

        if (debugLog)
        {
            float ear = (gaze.EyeHelper != null) ? gaze.EyeHelper.EyeFeature() : -1f;
            Debug.Log($"[Timer] Blinking={rawClosed} | EAR={ear:F4} | ConfirmTimer={confirmTimer:F2}s | Total={totalClosedTime:F2}s");
        }

        // Debounce: only confirm closure after eyes stay closed past confirmDelay
        if (rawClosed)
            confirmTimer += Time.deltaTime;
        else
            confirmTimer = 0f;

        isConfirmedClosed = confirmTimer >= confirmDelay;

        if (isConfirmedClosed)
        {
            float delta = Time.deltaTime;
            continuousClosedTime += delta;
            totalClosedTime += delta;
        }
        else if (!rawClosed)
        {
            continuousClosedTime = 0f;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (timerText == null)
            return;

        float displayTime = trackContinuousOnly ? continuousClosedTime : totalClosedTime;
        int minutes = (int)(displayTime / 60f);
        float seconds = displayTime % 60f;
        timerText.text = $"{minutes:00}:{seconds:00.00}";
    }

    /// <summary>Returns whether eyes are currently confirmed as closed.</summary>
    public bool IsEyeClosed => isConfirmedClosed;

    /// <summary>Returns total accumulated eye-closed duration in seconds.</summary>
    public float TotalClosedTime => totalClosedTime;

    /// <summary>Returns the current continuous eye-closed streak duration in seconds.</summary>
    public float ContinuousClosedTime => continuousClosedTime;

    /// <summary>Resets all timers and state back to zero.</summary>
    public void ResetTimer()
    {
        totalClosedTime = 0f;
        continuousClosedTime = 0f;
        confirmTimer = 0f;
        isConfirmedClosed = false;
        UpdateDisplay();
    }
}
