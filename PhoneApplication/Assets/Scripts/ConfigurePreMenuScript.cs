// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the configuration menu (previous to the configuration window).
//               Contains all the button listeners.
// SPECIAL NOTES:
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ConfigurePreMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Dropdown dropdown;
    public InputField inputField;
    public Button okButton;
    public Button clearButton;
    public Button configureButton;
    public Button backButton;
    public Text numberText;
    public Text clearedText;
    public Toggle usabilityTestsToggle;

    /// <summary>
    /// Canvas for scaling purposes.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Connection to the main script (gameplay).
    /// </summary>
    public Gameplay mainScript;

    /// <summary>
    /// Path for IO purposes.
    /// </summary>
    private string fileCoordsAnchorPath;

    /// <summary>
    /// Connection to the settings menu.
    /// </summary>
    public SettingsMenuScript settingsScript;

    /// <summary>
    /// Connection to the usability test script.
    /// </summary>
    public UsabilityTestScript usabilityTestScript;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        //Initializes the dropdown ui.
        dropdown.onValueChanged.AddListener(delegate { dropdownValueChanged(); changeNumberOfAnchors(); });
        dropdown.options.Clear();
        foreach (var option in mainScript.LoadDropdownHistory())
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = option });
        }
        dropdown.options.Add(new Dropdown.OptionData() { text = "Add" });
        dropdown.options.Add(new Dropdown.OptionData() { text = "Remove" });
        dropdown.value = 0;
        dropdownValueChanged();
        dropdown.RefreshShownValue();
        changeNumberOfAnchors();

        // Connect listeners to buttons
        okButton.onClick.AddListener(addOption);
        clearButton.onClick.AddListener(clearAnchors);
        configureButton.onClick.AddListener(changeToConfMenu);
        backButton.onClick.AddListener(back);

        // Path to save files.
        fileCoordsAnchorPath = Application.persistentDataPath;

        // Canvas scaling
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Behaviour when the dropdown value is changed.
    /// </summary>
    void dropdownValueChanged()
    {
        string optionText = dropdown.options[dropdown.value].text;
        if (optionText == "Add" || optionText == "Remove")
        {
            show_input(true);
        }
        else
        {
            show_input(false);
        }
    }

    /// <summary>
    /// [Deprecated] Changes the number of anchors saved.
    /// </summary>
    public void changeNumberOfAnchors()
    {
        int num_of_anchors = 0;
        foreach (var cloud in mainScript.LoadCloudAnchorHistory().Collection)
        {
            if (cloud.Scenario == dropdown.options[dropdown.value].text)
            {
                num_of_anchors += 1;
            }
        }
        numberText.text = "Number of hosted anchors in \"" + dropdown.options[dropdown.value].text + "\" scenario: " + num_of_anchors.ToString();
    }

    /// <summary>
    /// Adds or removes a new scenario to the dropdown.
    /// </summary>
    void addOption()
    {
        string optionText = dropdown.options[dropdown.value].text;
        if (optionText == "Add")
        {
            for (int i = dropdown.options.Count - 1; i >= 0; i--)
            {
                if (dropdown.options[i].text == "Add") dropdown.options.RemoveAt(i);
                else if (dropdown.options[i].text == "Remove") dropdown.options.RemoveAt(i);
            }
            dropdown.options.Add(new Dropdown.OptionData() { text = inputField.text });
            dropdown.options.Add(new Dropdown.OptionData() { text = "Add" });
            dropdown.options.Add(new Dropdown.OptionData() { text = "Remove" });
            mainScript.SaveDropdownHistory(inputField.text);

            dropdown.value = dropdown.options.Count - 3;

            File.Create(fileCoordsAnchorPath + "/" + inputField.text + ".json");
        }
        else if (optionText == "Remove")
        {
            for (int i = dropdown.options.Count - 1; i >= 0; i--)
            {
                if (dropdown.options[i].text == "Add") dropdown.options.RemoveAt(i);
                else if (dropdown.options[i].text == "Remove") dropdown.options.RemoveAt(i);
                else if (dropdown.options[i].text == inputField.text) dropdown.options.RemoveAt(i);
            }
            dropdown.options.Add(new Dropdown.OptionData() { text = "Add" });
            dropdown.options.Add(new Dropdown.OptionData() { text = "Remove" });
            mainScript.RemoveDropdownHistory(inputField.text);
            dropdown.value = 0;

            File.Delete(fileCoordsAnchorPath + "/" + inputField.text + ".json");
        }
        show_input(false);
        dropdown.RefreshShownValue();
        dropdownValueChanged();
        changeNumberOfAnchors();
    }

    /// <summary>
    /// Show the input field for adding/removing a new scenario.
    /// </summary>
    /// <param name="show">Show or hide this add/remove scenario menu.</param>
    void show_input(bool show)
    {
        inputField.gameObject.SetActive(show);
        okButton.gameObject.SetActive(show);
        clearButton.gameObject.SetActive(!show);
        configureButton.gameObject.SetActive(!show);
        //numberText.gameObject.SetActive(!show);
    }

    /// <summary>
    /// Clear the anchors saved on the selected scenario.
    /// </summary>
    void clearAnchors()
    {
        mainScript.ClearAnchors(dropdown.options[dropdown.value].text);
        changeNumberOfAnchors();
        clearedText.gameObject.SetActive(true);
        Invoke("cleared_off", 2.5f);
    }

    /// <summary>
    /// Hides the feedback clear text.
    /// </summary>
    void cleared_off()
    {
        clearedText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Changes to the configuration window.
    /// </summary>
    void changeToConfMenu()
    {
        if (settingsScript.cloud_enabled)
        {
            mainScript.changeMenu("cloud_conf");
        }
        else
        {
            mainScript.changeMenu("file_conf");
        }
        mainScript.setScenario(dropdown.options[dropdown.value].text);
        if (usabilityTestsToggle.isOn)
        {
            usabilityTestScript.enableTest();
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
}