// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the initial menu (When the application is launched).
// SPECIAL NOTES:
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button checkPerfomanceButton;
    public Button desktopConfButton;
    public Button userManualButton;
    public Button userManualPerfButton;

    /// <summary>
    /// Next navegation menus.
    /// </summary>
    public GameObject performanceMainMenu;
    public GameObject userManualMenu;
    public GameObject userManualPerfMenu;
    public GameObject desktopConfMenu;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        checkPerfomanceButton.onClick.AddListener(checkPerformance);
        desktopConfButton.onClick.AddListener(desktopConf);
        userManualButton.onClick.AddListener(userManual);
        userManualPerfButton.onClick.AddListener(userManualPerf);
    }

    /// <summary>
    /// Changes to the performance visualization menu.
    /// </summary>
    private void checkPerformance()
    {
        this.gameObject.SetActive(false);
        performanceMainMenu.SetActive(true);
    }

    /// <summary>
    /// Changes to the desktop configuration menu.
    /// </summary>
    private void desktopConf()
    {
        this.gameObject.SetActive(false);
        desktopConfMenu.SetActive(true);
    }

    /// <summary>
    /// Changes to the configuration user manual menu.
    /// </summary>
    private void userManual()
    {
        this.gameObject.SetActive(false);
        userManualMenu.SetActive(true);
    }

    /// <summary>
    /// Changes to the performance visualization user manual menu.
    /// </summary>
    private void userManualPerf()
    {
        this.gameObject.SetActive(false);
        userManualPerfMenu.SetActive(true);
    }
}