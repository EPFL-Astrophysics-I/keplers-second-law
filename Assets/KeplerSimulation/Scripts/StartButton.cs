using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject pauseText;
    [SerializeField] private GameObject resumeText;

    private List<GameObject> texts = new List<GameObject>();

    private void Awake()
    {
        if (startText != null)
        {
            startText.SetActive(false);
            texts.Add(startText);
        }

        if (pauseText != null)
        {
            pauseText.SetActive(false);
            texts.Add(pauseText);
        }

        if (resumeText != null)
        {
            resumeText.SetActive(false);
            texts.Add(resumeText);
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
        if (startText != null)
        {
            ShowText(startText);
        }
    }

    public void ShowPauseText()
    {
        if (pauseText != null)
        {
            ShowText(pauseText);
        }
    }

    public void ShowResumeText()
    {
        if (resumeText != null)
        {
            ShowText(resumeText);
        }
    }
}
