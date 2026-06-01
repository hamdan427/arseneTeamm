using UnityEngine;
using UnityEngine.UI;
using UnitEye;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the blink overlay effect and exposes the isBlink state.
/// Notifies SFXManager once per blink on the rising edge (false → true transition).
/// </summary>
public class BlinkEffect : MonoBehaviour
{
    [Header("References")]
    public Image blinkOverlay;
    public Gaze gaze;

    [Tooltip("Reference to SFXManager for blink sound. Assign in Inspector.")]
    public SFXManager sfxManager;

    [Header("Settings")]
    [Tooltip("Seconds the eye must stay closed before counting as a real blink.")]
    public float blinkDelay = 0.08f;

    [Tooltip("Speed at which the overlay fades in/out.")]
    public float blinkSpeed = 20f;

    /// <summary>True while the player's eyes are considered closed (Space held or real eye blink).</summary>
    public bool isBlink = false;

    private float blinkTimer = 0f;
    private bool previousBlink = false;

    private void Start()
    {
        if (blinkOverlay != null)
        {
            Color c = blinkOverlay.color;
            c.a = 0f;
            blinkOverlay.color = c;
        }
    }

    private void Update()
    {
        if (blinkOverlay == null)
            return;

        // --- Detect blink from UnitEye ---
        bool realEyeBlink = false;
        if (gaze != null)
        {
            if (gaze.Blinking)
                blinkTimer += Time.deltaTime;
            else
                blinkTimer = 0f;

            realEyeBlink = blinkTimer >= blinkDelay;
        }

        // --- Detect blink from Space key ---
        bool keyboardBlink = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

        // Combined blink state.
        bool doBlink = realEyeBlink || keyboardBlink;

        // isBlink tracks Space key only (used by LevelManager for threshold timing).
        isBlink = Input.GetKey(KeyCode.Space);

        // --- Rising edge: play blink SFX once per blink event ---
        if (doBlink && !previousBlink)
            sfxManager?.PlayBlink();

        previousBlink = doBlink;

        // --- Overlay fade ---
        Color col = blinkOverlay.color;
        float targetAlpha = doBlink ? 1f : 0f;
        col.a = Mathf.Lerp(col.a, targetAlpha, Time.deltaTime * blinkSpeed);
        blinkOverlay.color = col;
    }
}
