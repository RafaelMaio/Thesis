// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the user manual for the configuration.
// SPECIAL NOTES: 
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserManualMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button backButton;
    public Button nextButton;
    public Button previousButton;
    public Image helpImage;
    public Text instructionText;

    /// <summary>
    /// Initial menu.
    /// </summary>
    public GameObject initialMenu;

    /// <summary>
    /// Instruction images array.
    /// </summary>
    public Sprite[] spriteInstructions;

    /// <summary>
    /// Instruction current index.
    /// </summary>
    private int index = 0;

    /// <summary>
    /// Instruction texts.
    /// </summary>
    private string[] textInstructions =
    {
        "Instruction 1: Use the help button in order to recall all the interaction keys. Use the manual menu to add more precision.",
        "Instruction 2 (Cloud Anchors): Place the starting object in the started position and rotation.",
        "Instruction 3 (Motion Tracking): Place the starting object in the intended starting pose.",
        "Instruction 4 (Cloud Anchors): Add the anchors using the space key. The anchors will be automatically placed in the corresponding pose. Adjust their pose if necessary.",
        "Instruction 5 (Motion Tracking): Add one anchor, using the space key.",
        "Instruction 6: Add new virtual objects and place them in the intended pose. They will be automattically attached to the closest anchor.",
        "Instruction 7: Use the manual menu granting more precision.",
        "Instruction 8: Save the finished tracked. Transfer the output file to the mobile device application."
    };

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        backButton.onClick.AddListener(back);
        nextButton.onClick.AddListener(next);
        previousButton.onClick.AddListener(previous);
    }

    /// <summary>
    /// Returns to the previous menu.
    /// </summary>
    private void back()
    {
        this.gameObject.SetActive(false);
        initialMenu.SetActive(true);
    }

    /// <summary>
    /// Change to the next instruction.
    /// </summary>
    private void next()
    {
        index += 1;

        if (index >= textInstructions.Length)
        {
            index = 0;
        }

        helpImage.sprite = spriteInstructions[index];
        instructionText.text = textInstructions[index];
    }

    /// <summary>
    /// Change to the previous instruction.
    /// </summary>
    private void previous()
    {
        index -= 1;

        if (index < 0)
        {
            index = textInstructions.Length - 1;
        }

        helpImage.sprite = spriteInstructions[index];
        instructionText.text = textInstructions[index];
    }
}