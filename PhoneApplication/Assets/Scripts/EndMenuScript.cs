// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the end menu (When the game is finished).
// SPECIAL NOTES:
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button backButton;
    public Button retryButton;
    public Text timeText;
    public Text scoreText;

    /// <summary>
    /// Connection to the main script (gameplay).
    /// </summary>
    public Gameplay mainScript;

    // <summary>
    // Save the played game relevant info for repeating the game - Variables.
    // </summary>
    string last_mode;
    string last_scenario;
    string last_username;

    /// <summary>
    /// Canvas for scaling purposes.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        backButton.onClick.AddListener(back);
        retryButton.onClick.AddListener(retry);
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Save the played game relevant info for repeating the game.
    /// </summary>
    /// <param name="scenario">Last scenario.</param>
    /// <param name="mode">Last mode.</param>
    /// <param name="username">Last username.</param>
    public void change_last_mode(string scenario, string mode, string username)
    {
        last_mode = mode;
        last_scenario = scenario;
        last_username = username;

    }

    /// <summary>
    /// Change the time text - 00:00 format.
    /// </summary>
    /// <param name="seconds">Seconds to print.</param>
    /// <param name="minutes">Minutes to print.</param>
    public void changeTimeText(float minutes, float seconds)
    {
        timeText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    /// <summary>
    /// Change the score text.
    /// </summary>
    /// <param name="score">Score to print.</param>
    public void changeScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    /// <summary>
    /// Repeat the game.
    /// </summary>
    void retry()
    {
        mainScript.eraseOnBack();
        mainScript.chosenScenario(last_scenario, last_mode, last_username);
    }

    /// <summary>
    /// Returns to the previous menu.
    /// </summary>
    void back()
    {
        mainScript.eraseOnBack();
        mainScript.changeMenu("play_pre_menu");
    }
}