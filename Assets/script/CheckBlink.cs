using UnityEngine;

public class CheckBlink : MonoBehaviour
{
    public Gaze gaze;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gaze.Blinking);
    }
}
