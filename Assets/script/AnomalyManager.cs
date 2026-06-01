using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public bool hasAnomaly = false;
    public bool createRandom = false;
    public int afterBlink = 3;
    public int anomalyIndex = 0;
    public GameObject[] myAnomalys;

    private int currentBlink = 0;

    void Start()
    {
        if (createRandom) afterBlink = Random.Range(1, 5);
    }

    public void CountBlink()
    {
        currentBlink++;
        if (currentBlink >= afterBlink) CreateAnomaly();
    }

    private void CreateAnomaly()
    {
        if (createRandom) anomalyIndex = Random.Range(0, myAnomalys.Length);
        for (int i = 0; i < myAnomalys.Length; i++)
        {
            if (i == anomalyIndex) myAnomalys[i].SetActive(true);
            else myAnomalys[i].SetActive(false);
        }
        hasAnomaly = true;
    }
}