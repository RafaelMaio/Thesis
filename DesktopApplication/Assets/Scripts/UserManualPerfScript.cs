// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the user manual for the performance visualization.
// SPECIAL NOTES: 
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserManualPerfScript : MonoBehaviour
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
        "Instruction 1: Use the mouse and the keyboard to interact with the starting object and the camera.",
        "Instruction 2: Place the starting object in the starting game position and rotation.",
        "Instruction 3: Visualize your gaming performance. Move, rotate and zoom the camera to enhance the visualization.",
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