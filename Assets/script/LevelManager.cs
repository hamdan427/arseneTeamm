using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public Gaze gaze;
    public AnomalyManager anomaly;
    public CameraRotateSmooth cam;
    public float blinkTimer;
    public int Clock = 0;
    void Start()
    {
        
    }
    void Update()
    {
        if (gaze.Blinking)
        {
            blinkTimer += Time.deltaTime;
            //Debug.Log(gaze.Blinking+" | "+blinkTimer);
            if (blinkTimer >= 3f)
            {
                if (anomaly.hasAnomaly && (cam.CurrentPosition == 0 || cam.CurrentPosition == 2))
                    Clock++;
                else if (!anomaly.hasAnomaly && cam.CurrentPosition == 2) Clock++;
                else Clock = 0;
            }
        }
        else blinkTimer = 0f;
    }
}
