using UnityEngine;
using UnitEye;

/// <summary>
/// Temporary diagnostic script to identify why face detection is failing.
/// Attach to any active GameObject in the scene, assign references in Inspector, then Play.
/// Remove this component once the issue is resolved.
/// </summary>
public class EyeTrackingDiagnostic : MonoBehaviour
{
    public WebCamInput webCamInput;
    public Gaze gaze;

    private const float LogInterval = 2f;
    private float logTimer = 0f;

    [Header("Detection Tuning")]
    [Tooltip("Lower this if face is not detected. Default in UnitEye is 0.5. Try 0.1–0.3.")]
    [Range(0.05f, 0.9f)]
    public float faceDetectionThreshold = 0.2f;

    private void Start()
    {
        LogAvailableCameras();
        ApplyDetectionThreshold();
    }

    private void ApplyDetectionThreshold()
    {
        if (gaze == null || gaze.ModelRunner == null)
        {
            Debug.LogWarning("[Diagnostic] Cannot set threshold — Gaze or ModelRunner is null.");
            return;
        }

        gaze.ModelRunner.DetectionThreshold = faceDetectionThreshold;
        Debug.Log($"[Diagnostic] Face detection threshold set to {faceDetectionThreshold}");
    }

    private void Update()
    {
        logTimer += Time.deltaTime;
        if (logTimer < LogInterval)
            return;

        logTimer = 0f;
        LogWebCamState();
        LogGazeState();
    }

    private void LogAvailableCameras()
    {
        var devices = WebCamTexture.devices;
        Debug.Log($"[Diagnostic] Total webcam devices found: {devices.Length}");

        for (int i = 0; i < devices.Length; i++)
            Debug.Log($"[Diagnostic] Camera [{i}]: name='{devices[i].name}' | frontFacing={devices[i].isFrontFacing}");

        if (devices.Length == 0)
            Debug.LogError("[Diagnostic] No webcam devices detected. Check camera permissions or device connection.");
    }

    private void LogWebCamState()
    {
        if (webCamInput == null)
        {
            Debug.LogError("[Diagnostic] WebCamInput reference is not assigned.");
            return;
        }

        var wct = webCamInput.webCamTexture;
        if (wct == null)
        {
            Debug.LogError("[Diagnostic] webCamTexture is null — webcam failed to initialize.");
            return;
        }

        Debug.Log($"[Diagnostic] WebCam: isPlaying={wct.isPlaying} | resolution={wct.width}x{wct.height} | didUpdate={wct.didUpdateThisFrame} | device='{wct.deviceName}'");

        var rt = webCamInput.inputRT;
        Debug.Log($"[Diagnostic] InputRT: {(rt != null ? $"{rt.width}x{rt.height} | created={rt.IsCreated()}" : "null")}");

        float threshold = gaze?.ModelRunner?.DetectionThreshold ?? -1f;
        Debug.Log($"[Diagnostic] Face detection threshold (active): {threshold}");
    }

    private void LogGazeState()
    {
        if (gaze == null)
        {
            Debug.LogError("[Diagnostic] Gaze reference is not assigned.");
            return;
        }

        bool helperReady = gaze.EyeHelper != null;
        float ear = helperReady ? gaze.EyeHelper.EyeFeature() : -1f;

        Debug.Log($"[Diagnostic] Gaze: Blinking={gaze.Blinking} | Drowsy={gaze.Drowsy} | EyeHelperReady={helperReady} | EAR={ear:F4}");
    }
}
