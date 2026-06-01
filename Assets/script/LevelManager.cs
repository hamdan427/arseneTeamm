using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public Gaze gaze;
    public AnomalyManager anomaly;
    public CameraRotateSmooth cam;
    public float blinkTimer;
    private int sceneIndex = 1;
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
                if (!anomaly.hasAnomaly && (cam.CurrentPosition == 0 || cam.CurrentPosition == 2))
                    StartCoroutine(SwitchScene());
                else if (anomaly.hasAnomaly && cam.CurrentPosition == 1) StartCoroutine(SwitchScene());
                else SceneManager.LoadScene(1);
            }
        }
        else blinkTimer = 0f;
    }

    private IEnumerator SwitchScene()
    {
        sceneIndex++;
        SceneManager.LoadScene(sceneIndex);
        yield return new WaitForSeconds(1f);
    }
}
