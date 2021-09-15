// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the settings menu.
//               Contains all the button listeners.
// SPECIAL NOTES:
// ===============================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button backButton;
    public Button depthButton;
    public Button serverButton;
    public InputField ifAddress;
    public InputField ifPort;
    public Button cloudButton;
    public Button fileButton;
    public Button phoneButton;
    public Button desktopButton;

    /// <summary>
    /// Connection to the main script (gameplay).
    /// </summary>
    public Gameplay mainScript;

    /// <summary>
    /// Canvas for scaling purposes.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Booleans for saving the changes.
    /// </summary>
    public bool depth_enabled = false;
    public bool server_enabled = false;
    public bool cloud_enabled = true;
    public bool phone_enabled = true;

    /// <summary>
    /// Depth sub menu.
    /// </summary>
    public GameObject depthSettings;

    /// <summary>
    /// Network sub menu.
    /// </summary>
    public GameObject networkSettings;

    /// <summary>
    /// Operating mode sub menu.
    /// </summary>
    public GameObject modeSettings;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        backButton.onClick.AddListener(back);

        depthButton.onClick.AddListener(depthHandler);
        serverButton.onClick.AddListener(serverHandler);
        cloudButton.onClick.AddListener(cloudHandler);
        fileButton.onClick.AddListener(fileHandler);
        phoneButton.onClick.AddListener(phoneHandler);
        desktopButton.onClick.AddListener(desktopHandler);

        changeServerInteractable(false);
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        depthSettings.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        networkSettings.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        modeSettings.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Handles the depth buttons.
    /// </summary>
    void depthHandler()
    {
        if (depthButton.transform.GetChild(1).gameObject.activeSelf)
        {
            depthButton.transform.GetChild(1).gameObject.SetActive(false);
            depth_enabled = false;
        }
        else
        {
            depthButton.transform.GetChild(1).gameObject.SetActive(true);
            depth_enabled = true;
        }
    }

    /// <summary>
    /// Handles the server buttons.
    /// </summary>
    void serverHandler()
    {
        if (serverButton.transform.GetChild(1).gameObject.activeSelf)
        {
            serverButton.transform.GetChild(1).gameObject.SetActive(false);
            server_enabled = false;
            changeServerInteractable(false);
        }
        else
        {
            serverButton.transform.GetChild(1).gameObject.SetActive(true);
            server_enabled = true;
            changeServerInteractable(true);
        }
    }

    /// <summary>
    /// Handles the cloud button.
    /// </summary>
    void cloudHandler()
    {
        if (cloudButton.transform.GetChild(1).gameObject.activeSelf)
        {
            cloudButton.transform.GetChild(1).gameObject.SetActive(false);
            fileButton.transform.GetChild(1).gameObject.SetActive(true);
            cloud_enabled = false;
        }
        else
        {
            cloudButton.transform.GetChild(1).gameObject.SetActive(true);
            fileButton.transform.GetChild(1).gameObject.SetActive(false);
            cloud_enabled = true;
        }
    }

    /// <summary>
    /// Handles the camera coordinates button.
    /// </summary>
    void fileHandler()
    {
        if (cloudButton.transform.GetChild(1).gameObject.activeSelf)
        {
            cloudButton.transform.GetChild(1).gameObject.SetActive(false);
            fileButton.transform.GetChild(1).gameObject.SetActive(true);
            cloud_enabled = false;
        }
        else
        {
            cloudButton.transform.GetChild(1).gameObject.SetActive(true);
            fileButton.transform.GetChild(1).gameObject.SetActive(false);
            cloud_enabled = true;
        }
    }

    /// <summary>
    /// Handles the phone menu.
    /// </summary>
    void phoneHandler()
    {
        if (phoneButton.transform.GetChild(1).gameObject.activeSelf)
        {
            phoneButton.transform.GetChild(1).gameObject.SetActive(false);
            desktopButton.transform.GetChild(1).gameObject.SetActive(true);
            phone_enabled = false;
        }
        else
        {
            phoneButton.transform.GetChild(1).gameObject.SetActive(true);
            desktopButton.transform.GetChild(1).gameObject.SetActive(false);
            phone_enabled = true;
        }
    }

    /// <summary>
    /// Handles the desktop button.
    /// </summary>
    void desktopHandler()
    {
        if (phoneButton.transform.GetChild(1).gameObject.activeSelf)
        {
            phoneButton.transform.GetChild(1).gameObject.SetActive(false);
            desktopButton.transform.GetChild(1).gameObject.SetActive(true);
            phone_enabled = false;
        }
        else
        {
            phoneButton.transform.GetChild(1).gameObject.SetActive(true);
            desktopButton.transform.GetChild(1).gameObject.SetActive(false);
            phone_enabled = true;
        }
    }

    /// <summary>
    /// Returns to the previous menu.
    /// </summary>
    void back()
    {
        mainScript.changeMenu("initial");
        mainScript.eraseOnBack();
    }

    /// <summary>
    /// Change the server sub menu options interactivity.
    /// </summary>
    void changeServerInteractable(bool interact)
    {
        ifAddress.interactable = interact;
        ifPort.interactable = interact;
    }

    /// <summary>
    /// Get the server address at the address input field.
    /// </summary>
    public string getAddressField()
    {
        if (!string.IsNullOrEmpty(ifAddress.text))
        {
            return ifAddress.text;
        }
        return "";
    }

    /// <summary>
    /// Get the server port at the port input field.
    /// </summary>
    public int getPortField()
    {
        if (!string.IsNullOrEmpty(ifPort.text))
        {
            return Int32.Parse(ifPort.text);
        }
        return -1;
    }

    /// <summary>
    /// Change the depth button interactivity if the device supports or not the ARCore Depth API.
    /// </summary>
    public void depthNotAvailable()
    {
        depthButton.interactable = false;
    }
}