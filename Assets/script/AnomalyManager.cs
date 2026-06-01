using UnityEngine;

/// <summary>
/// Manages anomaly state for a level.
/// Set hasAnomaly in the Inspector to define whether this level has an anomaly.
/// Supports three anomaly types: PositionChange, ObjectSwap, and VisibilityToggle.
/// </summary>
public class AnomalyManager : MonoBehaviour
{
    [Header("Anomaly Config")]
    [Tooltip("Whether this level contains an anomaly. Configure in Inspector per level.")]
    public bool hasAnomaly = false;

    [Tooltip("All anomaly entries to apply when TriggerAnomaly is called.")]
    public AnomalyEntry[] anomalies;

    private bool anomalyTriggered = false;

    /// <summary>Whether this level has an anomaly defined.</summary>
    public bool HasAnomaly => hasAnomaly;

    private void Start()
    {
        // Initialize all entries so original state is captured before any trigger.
        foreach (var entry in anomalies)
            entry.Initialize();
    }

    /// <summary>Activates all anomaly entries if hasAnomaly is true.</summary>
    public void TriggerAnomaly()
    {
        if (!hasAnomaly || anomalyTriggered)
            return;

        anomalyTriggered = true;

        foreach (var entry in anomalies)
            entry.Apply();

        Debug.Log("[AnomalyManager] Anomaly triggered.");
    }

    /// <summary>Reverts all anomaly entries to their original state and resets the trigger flag.</summary>
    public void ResetAnomaly()
    {
        anomalyTriggered = false;

        foreach (var entry in anomalies)
            entry.Revert();

        Debug.Log("[AnomalyManager] Anomaly reset.");
    }
}

/// <summary>
/// A single anomaly change. Supports position change, object swap, and visibility toggle.
/// </summary>
[System.Serializable]
public class AnomalyEntry
{
    public enum AnomalyType
    {
        /// <summary>Moves target to a new world position.</summary>
        PositionChange,
        /// <summary>Hides target and shows swapTarget instead.</summary>
        ObjectSwap,
        /// <summary>Toggles target's active state.</summary>
        VisibilityToggle
    }

    [Tooltip("What kind of change this anomaly entry applies.")]
    public AnomalyType type;

    [Header("Target Object")]
    [Tooltip("The primary object affected by the anomaly.")]
    public GameObject target;

    [Header("Position Change Settings")]
    [Tooltip("World-space position to move the target to when anomaly activates.")]
    public Vector3 newPosition;

    [Header("Object Swap Settings")]
    [Tooltip("Object to reveal when anomaly is active. Target will be hidden.")]
    public GameObject swapTarget;

    [Header("Visibility Toggle Settings")]
    [Tooltip("If true, target starts visible and hides on anomaly. If false, reversed.")]
    public bool startsVisible = true;

    private Vector3 originalPosition;
    private bool initialized = false;

    /// <summary>Captures the original state. Must be called once before Apply/Revert.</summary>
    public void Initialize()
    {
        if (initialized || target == null)
            return;

        initialized = true;
        originalPosition = target.transform.position;

        // Ensure swap target starts hidden.
        if (type == AnomalyType.ObjectSwap && swapTarget != null)
            swapTarget.SetActive(false);
    }

    /// <summary>Applies the anomaly change.</summary>
    public void Apply()
    {
        if (target == null)
        {
            Debug.LogWarning("[AnomalyEntry] Target is null, skipping Apply.");
            return;
        }

        Initialize();

        switch (type)
        {
            case AnomalyType.PositionChange:
                target.transform.position = newPosition;
                break;

            case AnomalyType.ObjectSwap:
                target.SetActive(false);
                if (swapTarget != null)
                    swapTarget.SetActive(true);
                break;

            case AnomalyType.VisibilityToggle:
                target.SetActive(!startsVisible);
                break;
        }
    }

    /// <summary>Reverts the anomaly change back to the original state.</summary>
    public void Revert()
    {
        if (target == null)
            return;

        Initialize();

        switch (type)
        {
            case AnomalyType.PositionChange:
                target.transform.position = originalPosition;
                break;

            case AnomalyType.ObjectSwap:
                target.SetActive(true);
                if (swapTarget != null)
                    swapTarget.SetActive(false);
                break;

            case AnomalyType.VisibilityToggle:
                target.SetActive(startsVisible);
                break;
        }
    }
}
