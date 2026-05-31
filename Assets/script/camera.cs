using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotateSmooth : MonoBehaviour
{
    // Posisi kamera
    private Vector3 leftRotation = new Vector3(-6.202f, 144.142f, 8.795f);
    private Vector3 middleRotation = new Vector3(-8f, 185f, 0f);
    private Vector3 rightRotation = new Vector3(-9.268f, 229.94f, -5.467f);

    public float rotationSpeed = 2.5f;

    private Quaternion targetRotation;

    // 0 = kiri, 1 = tengah, 2 = kanan
    private int currentPosition = 1;

    void Start()
    {
        targetRotation = Quaternion.Euler(middleRotation);
        transform.rotation = targetRotation;
    }

    void Update()
    {
        // ARROW KIRI
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (currentPosition > 0)
            {
                currentPosition--;
                UpdateTargetRotation();
            }
        }

        // ARROW KANAN
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (currentPosition < 2)
            {
                currentPosition++;
                UpdateTargetRotation();
            }
        }

        // Smooth rotation
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * 100f * Time.deltaTime
        );
    }

    void UpdateTargetRotation()
    {
        switch (currentPosition)
        {
            case 0:
                targetRotation = Quaternion.Euler(leftRotation);
                break;

            case 1:
                targetRotation = Quaternion.Euler(middleRotation);
                break;

            case 2:
                targetRotation = Quaternion.Euler(rightRotation);
                break;
        }
    }
}