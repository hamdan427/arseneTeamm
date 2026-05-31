using UnityEngine;

public class CameraRotateSmooth : MonoBehaviour
{
    // Rotasi target
    private Vector3 leftRotation = new Vector3(-9.268f, 229.94f, -5.467f);
    private Vector3 rightRotation = new Vector3(-6.202f, 144.142f, 8.795f);

    // Speed rotasi
    public float rotationSpeed = 2.5f;

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // LEFT
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetRotation = Quaternion.Euler(leftRotation);
        }

        // RIGHT
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetRotation = Quaternion.Euler(rightRotation);
        }

        // Smooth rotation seperti orang menoleh
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * 100f * Time.deltaTime
        );
    }
}