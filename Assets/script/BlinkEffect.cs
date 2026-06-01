using UnityEngine;
using UnityEngine.UI;
using UnitEye;
using UnityEngine.InputSystem;

public class BlinkEffect : MonoBehaviour
{
    public Image blinkOverlay;

    // Delay anti false trigger
    public float blinkDelay = 0.08f;

    // Speed buka/tutup kelopak
    public float blinkSpeed = 20f;

    public Gaze gaze;
    public float blinkTimer = 0f;

    void Start()
    {
        Color c = blinkOverlay.color;
        c.a = 0f;
        blinkOverlay.color = c;
    }

    void Update()
    {
        if (gaze == null || blinkOverlay == null)
            return;

        // Blink dari UnitEye
        if (gaze.Blinking)
        {
            blinkTimer += Time.deltaTime;
        }
        else
        {
            blinkTimer = 0f;
        }

        bool realEyeBlink = blinkTimer >= blinkDelay;

        // Blink dari tombol Space
        bool keyboardBlink = Keyboard.current != null &&
                             Keyboard.current.spaceKey.isPressed;

        // Kalau salah satu aktif → blink
        bool doBlink = realEyeBlink || keyboardBlink;

        Color c = blinkOverlay.color;

        float targetAlpha = doBlink ? 1f : 0f;

        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * blinkSpeed);

        blinkOverlay.color = c;
    }
}