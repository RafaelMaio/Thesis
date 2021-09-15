// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the anchor stabiliy experience menu.
// SPECIAL NOTES: Still in testing phase.
// ===============================

using UnityEngine;
using UnityEngine.UI;

public class ExperienceMenuScript_2 : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button resultsButton;

    /// <summary>
    /// Debug texts.
    /// </summary>
    public Text debugText1;
    public Text debugText2;
    public Text debugText3;
    public Text debugText4;
    public Text debugText5;
    public Text debugText6;
    public Text debugText7;

    /// <summary>
    /// Connection to the experience main script (EnhanceTrackExperience).
    /// </summary>
    public EnhanceTrackExperience enhanceTrackExperience;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        resultsButton.onClick.AddListener(takeResults);
    }

    /// <summary>
    /// Unity change texts for debug.
    /// </summary>
    /// <param name="text">Debug text.</param>
    public void changeDebugText1(string text)
    {
        debugText1.text = text;
    }
    public void changeDebugText2(string text)
    {
        debugText2.text = text;
    }
    public void changeDebugText3(string text)
    {
        debugText3.text = text;
    }
    public void changeDebugText4(string text)
    {
        debugText4.text = text;
    }
    public void changeDebugText5(string text)
    {
        debugText5.text = text;
    }
    public void changeDebugText6(string text)
    {
        debugText6.text = text;
    }
    public void changeDebugText7(string text)
    {
        debugText7.text = text;
    }
    /// <summary>
    /// Unity change texts for debug.
    /// </summary>
    /// <param name="text">Debug text.</param>
    /// <param name="cont">Debug text index.</param>
    public void changeDebugTexts(string text, int cont)
    {
        if (cont == 1)
        {
            changeDebugText1(text);
        }
        else if (cont == 2)
        {
            changeDebugText2(text);
        }
        else if (cont == 3)
        {
            changeDebugText3(text);
        }
        else if (cont == 4)
        {
            changeDebugText4(text);
        }
        else if (cont == 5)
        {
            changeDebugText5(text);
        }
        else if (cont == 6)
        {
            changeDebugText6(text);
        }
        else if (cont == 7)
        {
            changeDebugText7(text);
        }
    }

    /// <summary>
    /// Take results for the current frame.
    /// </summary>
    public void takeResults()
    {
        enhanceTrackExperience.writeResults();
    }
}
