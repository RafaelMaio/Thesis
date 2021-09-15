// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the initial menu (When the application is launched).
// SPECIAL NOTES:
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class InitialMenuScript_v2 : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button configureButton;
    public Button playButton;
    public Button experienceButton;
    public Button quitButton;
    public Button settingsButton;
    public Button umButton;

    /// <summary>
    /// Connection to the main script (gameplay).
    /// </summary>
    public Gameplay mainScript;

    /// <summary>
    /// Canvas for scaling purposes.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        //Check if the device running the application is a computer device
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            mainScript.gameObject.GetComponent<ARCoreBackgroundRenderer>().enabled = false;
            configureButton.interactable = false;
            playButton.interactable = false;
            experienceButton.interactable = false;
            settingsButton.interactable = false;
        }
        configureButton.onClick.AddListener(changeToConfigureMenu);
        playButton.onClick.AddListener(changeToPlayMenu);
        experienceButton.onClick.AddListener(changeToExperienceMenu);
        quitButton.onClick.AddListener(quit);
        settingsButton.onClick.AddListener(settings);
        umButton.onClick.AddListener(user_manual);
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Changes to the user manual menu.
    /// </summary>
    void user_manual()
    {
        mainScript.changeMenu("user_manual");
    }

    /// <summary>
    /// Changes to the settings menu.
    /// </summary>
    void settings()
    {
        mainScript.changeMenu("settings");
    }

    /// <summary>
    /// Quit application.
    /// </summary>
    void quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Changes to the play menu.
    /// </summary>
    void changeToPlayMenu()
    {
        mainScript.changeMenu("play_pre_menu");
    }

    /// <summary>
    /// Changes to the experience window.
    /// </summary>
    void changeToExperienceMenu()
    {
        mainScript.changeMenu("experience_menu");
    }

    /// <summary>
    /// Changes to the configuration menu.
    /// </summary>
    void changeToConfigureMenu()
    {
        mainScript.changeMenu("configure_pre_menu");
    }

    /// <summary>
    /// Change configuration menu button interactivity.
    /// </summary>
    public void changeConfButtonInterectable(bool interactable)
    {
        if (configureButton.interactable != interactable)
        {
            configureButton.interactable = interactable;
        }
    }
}