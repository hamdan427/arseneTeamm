using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Central state machine for the anomaly-detection level loop.
/// Phases: Observing → EyesClosed → Deciding → Result
///
/// Controls:
///   - Hold Space for >= eyeCloseThreshold seconds to "close eyes"
///   - After releasing Space, face center (position 1) = claiming anomaly exists
///   - Face left (0) or right (2) = claiming no anomaly
///   - Press E to confirm the guess
/// </summary>
public class LevelManager : MonoBehaviour
{
    // --- References ---
    [Header("References")]
    public BlinkEffect blinkEffect;
    public AnomalyManager anomalyManager;
    public CameraRotateSmooth cameraController;

    [Header("UI")]
    public TMP_Text statusText;
    public TMP_Text timerText;
    [Tooltip("Optional panel shown on result. Can be null.")]
    public GameObject resultPanel;
    [Tooltip("Optional text inside resultPanel. Can be null.")]
    public TMP_Text resultText;

    // --- Settings ---
    [Header("Settings")]
    [Tooltip("Minimum seconds Space must be held continuously for eye-close to count.")]
    public float eyeCloseThreshold = 3f;
    [Tooltip("Seconds to display the result screen before transitioning.")]
    public float resultDisplayDuration = 2f;

    // --- State ---
    public enum Phase { Observing, EyesClosed, Deciding, Result }
    [HideInInspector] public Phase currentPhase = Phase.Observing;

    private float spaceHoldTimer = 0f;
    private bool eyeCloseCompleted = false;
    private float resultTimer = 0f;
    private bool guessCorrect = false;

    private void Start()
    {
        if (anomalyManager == null)
            Debug.LogError("[LevelManager] AnomalyManager reference is not assigned.");
        if (cameraController == null)
            Debug.LogError("[LevelManager] CameraRotateSmooth reference is not assigned.");

        SetPhase(Phase.Observing);
    }

    private void Update()
    {
        switch (currentPhase)
        {
            case Phase.Observing:
                UpdateObserving();
                break;
            case Phase.EyesClosed:
                UpdateEyesClosed();
                break;
            case Phase.Deciding:
                UpdateDeciding();
                break;
            case Phase.Result:
                UpdateResult();
                break;
        }
    }

    // ---------------------------------------------------------------
    // Phase: Observing
    // ---------------------------------------------------------------
    private void UpdateObserving()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetPhase(Phase.EyesClosed);
    }

    // ---------------------------------------------------------------
    // Phase: EyesClosed
    // ---------------------------------------------------------------
    private void UpdateEyesClosed()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            spaceHoldTimer += Time.deltaTime;
            UpdateTimerDisplay(spaceHoldTimer);

            // Trigger anomaly exactly once when threshold is reached.
            if (!eyeCloseCompleted && spaceHoldTimer >= eyeCloseThreshold)
            {
                eyeCloseCompleted = true;
                anomalyManager?.TriggerAnomaly();
            }
        }
        else
        {
            // Space released early — reset and go back to observing.
            if (!eyeCloseCompleted)
            {
                spaceHoldTimer = 0f;
                SetPhase(Phase.Observing);
                return;
            }

            // Space released after threshold — proceed to deciding.
            SetPhase(Phase.Deciding);
        }
    }

    // ---------------------------------------------------------------
    // Phase: Deciding
    // ---------------------------------------------------------------
    private void UpdateDeciding()
    {
        if (Input.GetKeyDown(KeyCode.E))
            EvaluateGuess();
    }

    private void EvaluateGuess()
    {
        if (cameraController == null)
        {
            Debug.LogError("[LevelManager] Cannot evaluate guess — CameraRotateSmooth is null.");
            return;
        }

        // Position 1 (center) = player claims anomaly exists.
        bool playerSaysAnomaly = (cameraController.CurrentPosition == 1);
        guessCorrect = (playerSaysAnomaly == anomalyManager.HasAnomaly);

        Debug.Log($"[LevelManager] Guess: playerSaysAnomaly={playerSaysAnomaly} | hasAnomaly={anomalyManager.HasAnomaly} | correct={guessCorrect}");

        SetPhase(Phase.Result);
    }

    // ---------------------------------------------------------------
    // Phase: Result
    // ---------------------------------------------------------------
    private void UpdateResult()
    {
        resultTimer += Time.deltaTime;
        if (resultTimer >= resultDisplayDuration)
        {
            if (guessCorrect)
                LoadNextScene();
            else
                RestartScene();
        }
    }

    // ---------------------------------------------------------------
    // Phase Transitions
    // ---------------------------------------------------------------
    private void SetPhase(Phase phase)
    {
        currentPhase = phase;

        switch (phase)
        {
            case Phase.Observing:
                spaceHoldTimer = 0f;
                eyeCloseCompleted = false;
                anomalyManager?.ResetAnomaly();
                UpdateTimerDisplay(0f);
                SetResultPanel(false);
                cameraController?.SetInputEnabled(true);
                UpdateStatus("Amati ruangan. Tahan Space untuk menutup mata.");
                break;

            case Phase.EyesClosed:
                spaceHoldTimer = 0f;
                eyeCloseCompleted = false;
                cameraController?.SetInputEnabled(false);
                UpdateStatus($"Mata tertutup... tahan selama {eyeCloseThreshold:F0} detik.");
                break;

            case Phase.Deciding:
                cameraController?.SetInputEnabled(true);
                UpdateTimerDisplay(spaceHoldTimer);
                UpdateStatus("Lihat TENGAH = ada anomali | KIRI/KANAN = tidak ada. Tekan E untuk konfirmasi.");
                break;

            case Phase.Result:
                resultTimer = 0f;
                cameraController?.SetInputEnabled(false);
                string resultMsg = guessCorrect ? "BENAR! Melanjutkan..." : "SALAH! Mengulang...";
                UpdateStatus(resultMsg);
                SetResultPanel(true, guessCorrect ? "BENAR!" : "SALAH!");
                break;
        }
    }

    // ---------------------------------------------------------------
    // Scene Navigation
    // ---------------------------------------------------------------
    private void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene(0); // Loop back to first scene / main menu.
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ---------------------------------------------------------------
    // UI Helpers
    // ---------------------------------------------------------------
    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private void UpdateTimerDisplay(float seconds)
    {
        if (timerText != null)
            timerText.text = $"{seconds:F1}s";
    }

    private void SetResultPanel(bool visible, string message = "")
    {
        if (resultPanel != null)
            resultPanel.SetActive(visible);
        if (resultText != null)
            resultText.text = message;
    }
}
