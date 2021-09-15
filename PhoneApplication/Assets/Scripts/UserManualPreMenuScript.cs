// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the menu for choosing the user manual.
// SPECIAL NOTES: 
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserManualPreMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button backButton;
    public Button umWheelChairButton;
    public Button umConfButton;
    public Button umPlayButton;

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
        backButton.onClick.AddListener(back);
        umWheelChairButton.onClick.AddListener(show_wheelchair_um_menu);
        umConfButton.onClick.AddListener(show_conf_um_menu);
        umPlayButton.onClick.AddListener(show_play_um_menu);
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Returns to the previous menu.
    /// </summary>
    private void back()
    {
        mainScript.eraseOnBack();
        mainScript.changeMenu("initial");
    }

    /// <summary>
    /// Change to the wheelchair user manual window.
    /// </summary>
    private void show_wheelchair_um_menu()
    {
        mainScript.eraseOnBack();
        mainScript.changeMenu("um_wheelchair");
    }

    /// <summary>
    /// Change to the configuration user manual window.
    /// </summary>
    private void show_conf_um_menu()
    {
        mainScript.eraseOnBack();
        mainScript.changeMenu("um_conf");
    }

    /// <summary>
    /// Change to the play user manual window.
    /// </summary>
    private void show_play_um_menu()
    {
        mainScript.eraseOnBack();
        mainScript.changeMenu("um_play");
    }
}