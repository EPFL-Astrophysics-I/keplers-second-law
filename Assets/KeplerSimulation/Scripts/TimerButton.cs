using System.Collections.Generic;
using UnityEngine;

public class TimerButton : MonoBehaviour
{
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject stopText;

    private List<GameObject> texts = new List<GameObject>();

    private void Awake()
    {
        if (startText)
        {
            texts.Add(startText);
            //startText.SetActive(false);
        }

        if (stopText)
        {
            stopText.SetActive(false);
            texts.Add(stopText);
        }
    }

    private void ShowText(GameObject text)
    {
        foreach (GameObject textGO in texts)
        {
            textGO.SetActive(false);
        }

        text.SetActive(true);
    }

    public void ShowStartText()
    {
        if (startText)
        {
            ShowText(startText);
        }
    }

    public void ShowStopText()
    {
        if (stopText)
        {
            ShowText(stopText);
        }
    }
}
