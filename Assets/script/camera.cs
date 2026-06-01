using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Smoothly rotates the camera between three horizontal positions: left, center, and right.
/// Input can be disabled externally (e.g., by LevelManager during EyesClosed or Result phases).
/// </summary>
public class CameraRotateSmooth : MonoBehaviour
{
    // --- Rotation targets (Euler angles) ---
    private readonly Vector3 LeftRotation   = new Vector3(-6.202f,  144.142f,  8.795f);
    private readonly Vector3 MiddleRotation = new Vector3(-8f,      185f,      0f);
    private readonly Vector3 RightRotation  = new Vector3(-9.268f,  229.94f,  -5.467f);

    [Tooltip("Degrees per second for smooth rotation.")]
    public float rotationSpeed = 2.5f;

    private Quaternion targetRotation;

    /// <summary>0 = left, 1 = center, 2 = right.</summary>
    private int currentPosition = 1;

    private bool inputEnabled = true;

    /// <summary>Read-only access to the current camera direction. 0 = left, 1 = center, 2 = right.</summary>
    public int CurrentPosition => currentPosition;

    void Start()
    {
        targetRotation = Quaternion.Euler(MiddleRotation);
        transform.rotation = targetRotation;
    }

    void Update()
    {
        if (inputEnabled && Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame && currentPosition > 0)
            {
                currentPosition--;
                UpdateTargetRotation();
            }

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame && currentPosition < 2)
            {
                currentPosition++;
                UpdateTargetRotation();
            }
        }

        // Always smooth-rotate toward the target regardless of input state.
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * 100f * Time.deltaTime
        );
    }

    /// <summary>Enables or disables arrow-key input for camera rotation.</summary>
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    private void UpdateTargetRotation()
    {
        switch (currentPosition)
        {
            case 0:
                targetRotation = Quaternion.Euler(LeftRotation);
                break;
            case 1:
                targetRotation = Quaternion.Euler(MiddleRotation);
                break;
            case 2:
                targetRotation = Quaternion.Euler(RightRotation);
                break;
        }
    }
}
