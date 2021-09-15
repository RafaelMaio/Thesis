// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the user manual for using the wheelchair.
// SPECIAL NOTES: 
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserManualWheelchairMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button backButton;

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
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Returns to the previous menu.
    /// </summary>
    private void back()
    {
        mainScript.eraseOnBack();
        mainScript.changeMenu("user_manual");
    }
}