// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Dinamically sets the lean component properties and configuration properties.
// SPECIAL NOTES: Camera and slider properties.
// ===============================

using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanCameraHandler : MonoBehaviour
{
    /// <summary>
    /// Device camera.
    /// </summary>
    private GameObject camera;

    /// <summary>
    /// Configuration menu script.
    /// </summary>
    private ConfigureMenuScript configureMenuScript;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        try
        {
            camera = GameObject.Find("Main Camera");
            configureMenuScript = GameObject.Find("ConfigureMenu").GetComponent<ConfigureMenuScript>();
        }
        catch { }
        try
        {
            this.GetComponent<LeanTranslate>().Camera = camera.GetComponent<Camera>();
        }
        catch { }
        try
        {
            this.GetComponent<LeanRotate>().Camera = camera.GetComponent<Camera>();
            this.GetComponent<LeanRotate>().enabled = configureMenuScript.scaleRotSlider.value == 0;
        }
        catch { }
        try
        {
            this.GetComponent<LeanScale>().Camera = camera.GetComponent<Camera>();
            this.GetComponent<LeanScale>().enabled = configureMenuScript.scaleRotSlider.value == 1;
        }
        catch { }
    }
}