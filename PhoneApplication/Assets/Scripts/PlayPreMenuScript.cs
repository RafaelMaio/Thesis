// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the play menu (previous to the play window).
//               Contains all the button listeners.
// SPECIAL NOTES:
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPreMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Dropdown dropdown;
    public Dropdown velocityDropdown;
    public Text numberText;
    public Button staticButton;
    public Button movingButton;
    public Button backButton;
    public InputField ifUsername;

    /// <summary>
    /// Connection to the main script (gameplay).
    /// </summary>
    public Gameplay mainScript;

    /// <summary>
    /// Canvas for scaling purposes.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Connection to the settings menu.
    /// </summary>
    public SettingsMenuScript settingsScript;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { changeTheNumberOfAnchors(); });
        fillDropdown();
        if (dropdown.options.Count > 0)
        {
            dropdown.value = 0;
            dropdown.RefreshShownValue();
            changeTheNumberOfAnchors();
        }

        staticButton.onClick.AddListener(changeToCloudStaticMenu);
        movingButton.onClick.AddListener(changeToCloudMovingMenu);
        backButton.onClick.AddListener(back);
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Changes to the play window -> Game with static objects.
    /// </summary>
    void changeToCloudStaticMenu()
    {
        string username = "user" + ((int)Random.Range(1, 10000)).ToString();
        if(!string.IsNullOrEmpty(ifUsername.text))
        {
            username = ifUsername.text;
        }
        if (settingsScript.cloud_enabled)
        {
            mainScript.chosenScenario(dropdown.options[dropdown.value].text, "play_cloud_static", username);
        }
        else
        {
            mainScript.chosenScenario(dropdown.options[dropdown.value].text, "play_file_static", username);
        }
    }

    /// <summary>
    /// Fill the dropdown with all the created scenarios.
    /// </summary>
    void fillDropdown()
    {
        dropdown.options.Clear();
        if (mainScript.settingsMenuScript.phone_enabled)
        {
            foreach (var option in mainScript.LoadDropdownHistory())
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = option });
            }
        }
        else
        {
            foreach (var option in mainScript.LoadDesktopScenarios())
            {
                dropdown.options.Add(new Dropdown.OptionData() { text = option });
            }
        }
    }

    /// <summary>
    /// Changes to the play window -> Game with the moving object.
    /// </summary>
    void changeToCloudMovingMenu()
    {
        string username = "user" + ((int)Random.Range(1, 10000)).ToString();
        if (!string.IsNullOrEmpty(ifUsername.text))
        {
            username = ifUsername.text;
        }
        if (settingsScript.cloud_enabled)
        {
            mainScript.chosenScenario(dropdown.options[dropdown.value].text, "play_cloud_moving", username);
            if (velocityDropdown.options[velocityDropdown.value].text.Contains("1"))
                mainScript.setVelocity(1);
            else if (velocityDropdown.options[velocityDropdown.value].text.Contains("2"))
                mainScript.setVelocity(2);
            else if (velocityDropdown.options[velocityDropdown.value].text.Contains("3"))
                mainScript.setVelocity(3);
            else if (velocityDropdown.options[velocityDropdown.value].text.Contains("4"))
                mainScript.setVelocity(4);
        }
        else
        {
            mainScript.chosenScenario(dropdown.options[dropdown.value].text, "play_file_moving", username);
            if (velocityDropdown.options[velocityDropdown.value].text.Contains("1"))
                mainScript.setVelocity(1);
            else if (velocityDropdown.options[velocityDropdown.value].text.Contains("2"))
                mainScript.setVelocity(2);
            else if (velocityDropdown.options[velocityDropdown.value].text.Contains("3"))
                mainScript.setVelocity(3);
            else if (velocityDropdown.options[velocityDropdown.value].text.Contains("4"))
                mainScript.setVelocity(4);
        }
    }

    /// <summary>
    /// Changes the number of saved anchors in the corresponding scenario -> Deprecated.
    /// </summary>
    public void changeTheNumberOfAnchors()
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
    /// Returns to the previous menu.
    /// </summary>
    void back()
    {
        mainScript.changeMenu("initial");
        mainScript.eraseOnBack();
    }

    /// <summary>
    /// Refresh the dropdown option when the selected scenario is changed.
    /// </summary>
    public void refreshDropdown()
    {
        fillDropdown();
        dropdown.RefreshShownValue();
    }
}
