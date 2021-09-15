// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the user manual for the configuration.
// SPECIAL NOTES: 
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserManualConfigureMenuScript : MonoBehaviour
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
        "Instruction 1: Create or choose the desired scenario.",
        "Instruction 2 (Cloud): Choose a place with plenty of texture to add a new anchor.",
        "Instruction 3: Press the \"Add a new anchor\" button and place it on a surface.",
        "Instruction 4 (Cloud): Point the device to the anchor and move around it, until all the walls turn yellow and then green.",
        "Instruction 5: Choose the desired virtual object using the arrow buttons.",
        "Instruction 6: Drag the virtual object to the desired position.",
        "Instruction 7: Rotate/scale the virtual object using two fingers. Use the slider to switch between rotation and scaling.",
        "Instruction 8: Use the manual menu granting more precision.",
        "Instruction 9: Attach more objects to the created anchor, if its locations is close to it.",
        "Instruction 10: Save the anchor and the objects associated with it.",
        "Instruction 11: Repeat until you finish the desired track.",
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
        
        if(index >= textInstructions.Length) 
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