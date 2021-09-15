// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the user manual for playing.
// SPECIAL NOTES: 
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserManualPlayMenuScript : MonoBehaviour
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
    /// Connection to the main script (gameplay).
    /// </summary>
    public Gameplay mainScript;

    /// <summary>
    /// Canvas for scaling purposes.
    /// </summary>
    public Canvas canvas;

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
        "Instruction 1: Catch the green cubes in the shortest time possible.",
        "Instruction 2 (Static Objects): Follow the direction of the arrows as close as possible to the white line.",
        "Instruction 3 (Moving Object): Follow the car as close to it as possible.",
        "Instruction 4: Don't leave the road, unless you need to dodge something.",
        "Instruction 5: Dodge the road obstacles or they will explode.",
        "Instruction 6: Look away from the spotlight or you will get blinded.",
        "Instruction 7: Stop! Immobilize the wheelchair.",
        "Instruction 8: At the end turn around and look to the path taken (red line)",
        "Instruction 9 (Server enabled): Verify your performance on the desktop machine.",
    };

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        backButton.onClick.AddListener(back);
        nextButton.onClick.AddListener(next);
        previousButton.onClick.AddListener(previous);
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