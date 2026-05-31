using UnityEngine;
using UnityEngine.UI;
using UnitEye;

public class BlinkEffect : MonoBehaviour
{
    public Image blinkOverlay;

    // Harus dianggap blink selama beberapa waktu dulu
    public float blinkDelay = 0.40f;

    // Kecepatan nutup & buka mata
    public float blinkSpeed = 20f;

    private Gaze gaze;
    private float blinkTimer = 0f;

    void Start()
    {
        gaze = FindObjectOfType<Gaze>();

        Color c = blinkOverlay.color;
        c.a = 0f;
        blinkOverlay.color = c;
    }

    void Update()
    {
        if (gaze == null || blinkOverlay == null)
            return;

        // Kalau UnitEye bilang blink
        if (gaze.Blinking)
        {
            blinkTimer += Time.deltaTime;
        }
        else
        {
            blinkTimer = 0f;
        }

        // Baru trigger kalau blink cukup lama
        bool realBlink = blinkTimer >= blinkDelay;

        Color c = blinkOverlay.color;

        // FULL hitam pas blink
        float targetAlpha = realBlink ? 1f : 0f;

        // Fade cepet biar natural kayak kelopak mata
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * blinkSpeed);

        blinkOverlay.color = c;
    }
}