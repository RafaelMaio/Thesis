// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the configuration window.
//               Contains all the button listeners.
// SPECIAL NOTES:
// ===============================


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureMenuScript : MonoBehaviour
{
    /// <summary>
    /// Tools menu.
    /// </summary>
    public Button hostButton;
    public Button rightArrowButton;
    public Button leftArrowButton;
    public Button backButton;
    public Button addNewAnchorButton;
    public Button attachNewObjectButton;
    public Button removeButton;
    public Slider sliderScaleRot;

    /// <summary>
    /// Usability Texts
    /// </summary>
    public Text endText;
    public Text numObjectsText;

    /// <summary>
    /// Help Menu.
    /// </summary>
    public Text instructionText;
    public Button helpButton;
    public GameObject instructionPanel;

    /// <summary>
    /// Manual Mode UI with behaviour.
    /// </summary>
    public Button manualButton;
    public InputField manualInputField;
    public Button okButton;
    public Text idText;
    public Text xText;
    public Text yText;
    public Text zText;
    public Text rotText;
    public Text scaleText;
    public Image manualArrow;
    public Button manualButtonMore;
    public Button manualButtonLess;
    public Button manualCircleButtonId;
    public Button manualCircleButtonX;
    public Button manualCircleButtonY;
    public Button manualCircleButtonZ;
    public Button manualCircleButtonRot;
    public Button manualCircleButtonScale;
    public GameObject manualMenu;

    /// <summary>
    /// Canvas for scaling purposes.
    /// </summary>
    public Canvas canvas;
    public GameObject tools;

    /// <summary>
    /// Connection to the main script (gameplay).
    /// </summary>
    public Gameplay mainScript;

    /// <summary>
    /// Saves the last instruction text.
    /// </summary>
    private string last_text;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        // Connect listeners to buttons
        hostButton.onClick.AddListener(host);
        rightArrowButton.onClick.AddListener(changePrefabRight);
        leftArrowButton.onClick.AddListener(changePrefabLeft);
        backButton.onClick.AddListener(back);
        helpButton.onClick.AddListener(show_help);
        addNewAnchorButton.onClick.AddListener(addNewAnchor);
        attachNewObjectButton.onClick.AddListener(attachNewObject);
        removeButton.onClick.AddListener(remove);
        sliderScaleRot.onValueChanged.AddListener(delegate { changeBetweenScaleRot(); });

        addNewAnchorButton.interactable = true;
        attachNewObjectButton.interactable = false;

        // Manual at start 
        manualButton.onClick.AddListener(show_menu);
        manualCircleButtonX.onClick.AddListener(delegate { selectText(0); });
        manualCircleButtonY.onClick.AddListener(delegate { selectText(1); });
        manualCircleButtonZ.onClick.AddListener(delegate { selectText(2); });
        manualCircleButtonRot.onClick.AddListener(delegate { selectText(3); });
        manualCircleButtonScale.onClick.AddListener(delegate { selectText(4); });
        manualCircleButtonId.onClick.AddListener(delegate { selectText(5); });
        manualButtonMore.onClick.AddListener(delegate { changeManualMoreOrLess(true); });
        manualButtonLess.onClick.AddListener(delegate { changeManualMoreOrLess(false); });
        okButton.onClick.AddListener(changeByInput);

        // Starts with the manual menu hidden 
        changeManualInteractable(false);

        // Canvas scaling
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        tools.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Change between scaling and rotation a virtual object.
    /// </summary>
    private void changeBetweenScaleRot()
    {
        mainScript.changeBetweenSlideRot((int)sliderScaleRot.value);
    }


    /// <summary>
    /// Remove a virtual object from the scene.
    /// </summary>
    private void remove()
    {
        mainScript.remove();
    }

    /// <summary>
    /// Unity update function.
    /// </summary>
    private void Update()
    {
        // If the text changes, the help button blinks 
        if (instructionText.text != last_text)
        {
            last_text = instructionText.text;
            if (!IsInvoking("blinkHelpButton"))
            {
                Invoke("blinkHelpButton", 0.9f);
            }
        }
    }

    /// <summary>
    /// Show or hide the instruction text when the help button is clicked.
    /// </summary>
    public void show_help()
    {
        if (instructionText.gameObject.activeSelf)
        {
            if (!IsInvoking("blinkHelpButton"))
            {
                instructionPanel.SetActive(!instructionText.gameObject.activeSelf);
                instructionText.gameObject.SetActive(!instructionText.gameObject.activeSelf);
            }
        }
        else
        {
            instructionPanel.SetActive(!instructionText.gameObject.activeSelf);
            instructionText.gameObject.SetActive(!instructionText.gameObject.activeSelf);
        }
        helpButton.GetComponent<Image>().color = new Color32(47, 200, 255, 255);
        CancelInvoke("blinkHelpButton");
    }

    /// <summary>
    /// Changes the color for the help button when a new instruction text appears.
    /// </summary>
    private void blinkHelpButton()
    {
        // Changes to blue when red or white
        if (helpButton.GetComponent<Image>().color == new Color32(255, 255, 255, 255) || helpButton.GetComponent<Image>().color == Color.red)
        {
            helpButton.GetComponent<Image>().color = new Color32(47, 200, 255, 255);
        }
        // Changes to red or white when blue
        else
        {
            if (instructionText.color == Color.red)
            {
                helpButton.GetComponent<Image>().color = Color.red;
            }
            else
            {
                helpButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
        Invoke("blinkHelpButton", 0.9f);
    }

    /// <summary>
    /// Changes the virtual object position, rotation or scale for the new value inserted on the input field.
    /// </summary>
    void changeByInput()
    {
        if (manualArrow.transform.position.y == manualCircleButtonX.transform.position.y)
        {
            mainScript.changeManualByInput(0, (float)Convert.ToDouble(manualInputField.text));
        }
        else if (manualArrow.transform.position.y == manualCircleButtonY.transform.position.y)
        {
            mainScript.changeManualByInput(1, (float)Convert.ToDouble(manualInputField.text));
        }
        else if (manualArrow.transform.position.y == manualCircleButtonZ.transform.position.y)
        {
            mainScript.changeManualByInput(2, (float)Convert.ToDouble(manualInputField.text));
        }
        else if (manualArrow.transform.position.y == manualCircleButtonRot.transform.position.y)
        {
            mainScript.changeManualByInput(3, (float)Convert.ToDouble(manualInputField.text));
        }
        else if (manualArrow.transform.position.y == manualCircleButtonScale.transform.position.y)
        {
            mainScript.changeManualByInput(4, (float)Convert.ToDouble(manualInputField.text));
        }
        else if (manualArrow.transform.position.y == manualCircleButtonId.transform.position.y)
        {
            mainScript.changeManualByInput(5, (float)Convert.ToDouble(manualInputField.text));
        }
    }

    /// <summary>
    /// Changes the values on the manual menu UI texts.
    /// </summary>
    /// <param name="x">the virtual object x-coordinate.</param>
    /// <param name="y">the virtual object y-coordinate.</param>
    /// <param name="z">the virtual object z-coordinate.</param>
    /// <param name="rot">the virtual object yy rotation.</param>
    /// <param name="scale">the virtual object scale.</param>
    /// <param name="id">the virtual object id in the path.</param>
    public void changeManualTexts(float x, float y, float z, float rot, float scale, int id)
    {
        xText.text = "X: " + Math.Round(x, 1).ToString("0.0");
        yText.text = "Y: " + Math.Round(y, 1).ToString("0.0");
        zText.text = "Z: " + Math.Round(z, 1).ToString("0.0");
        rotText.text = "Rotation: " + ((int)rot).ToString("0");
        scaleText.text = "Scale: " + Math.Round(scale, 1).ToString("0.0");
        idText.text = "Id: " + id.ToString();
    }

    /// <summary>
    /// Veirfy if the manual menu is intectable.
    /// </summary>
    /// <returns>The manual menu interactivity.</returns>
    public bool getManualInteractable()
    {
        return manualButtonMore.IsInteractable();
    }

    /// <summary>
    /// Changes the manual menu interactability if a virtual object is selected or not.
    /// </summary>
    /// <param name="interactable">Boolean controlling the manual menu interactability</param>
    public void changeManualInteractable(bool interactable)
    {
        manualInputField.interactable = interactable;
        manualButtonMore.interactable = interactable;
        manualButtonLess.interactable = interactable;
        manualCircleButtonX.interactable = interactable;
        manualCircleButtonY.interactable = interactable;
        manualCircleButtonZ.interactable = interactable;
        manualCircleButtonRot.interactable = interactable;
        manualCircleButtonId.interactable = interactable;
        manualCircleButtonScale.interactable = interactable;
        okButton.interactable = interactable;
    }

    /// <summary>
    /// Changes the manual menu interactability if a virtual object is selected or not.
    /// </summary>
    /// <param name="interactable">Boolean controlling the manual menu interactability</param>
    public void changeManualInteractableAux(bool interactable)
    {
        manualInputField.interactable = interactable;
        manualButtonMore.interactable = interactable;
        manualButtonLess.interactable = interactable;
        manualCircleButtonX.interactable = interactable;
        manualCircleButtonY.interactable = interactable;
        manualCircleButtonZ.interactable = interactable;
        manualCircleButtonRot.interactable = interactable;
        manualCircleButtonId.interactable = false;
        manualCircleButtonScale.interactable = interactable;
        okButton.interactable = interactable;
    }

    /// <summary>
    /// Translate, rotate or scale the virtual object for the next position, rotation or scale. (0.1f, 1f, 0.1f)
    /// </summary>
    /// <param name="moreOrLess">Negative or position increment.</param>
    void changeManualMoreOrLess(bool moreOrLess)
    {
        if (manualArrow.transform.position.y == manualCircleButtonX.transform.position.y)
        {
            mainScript.changeManualMoreOrLess(0, moreOrLess);
        }
        else if (manualArrow.transform.position.y == manualCircleButtonY.transform.position.y)
        {
            mainScript.changeManualMoreOrLess(1, moreOrLess);
        }
        else if (manualArrow.transform.position.y == manualCircleButtonZ.transform.position.y)
        {
            mainScript.changeManualMoreOrLess(2, moreOrLess);
        }
        else if (manualArrow.transform.position.y == manualCircleButtonRot.transform.position.y)
        {
            mainScript.changeManualMoreOrLess(3, moreOrLess);
        }
        else if (manualArrow.transform.position.y == manualCircleButtonScale.transform.position.y)
        {
            mainScript.changeManualMoreOrLess(4, moreOrLess);
        }
        else if (manualArrow.transform.position.y == manualCircleButtonId.transform.position.y)
        {
            mainScript.changeManualMoreOrLess(5, moreOrLess);
        }
    }

    /// <summary>
    /// Position the arrow at the button clicked.
    /// </summary>
    /// <param name="index">Button clicked index. (Higher yy, lower index)</param>
    void selectText(int index)
    {
        switch (index)
        {
            case 0:
                manualArrow.transform.position = new Vector3(manualArrow.transform.position.x, manualCircleButtonX.transform.position.y, manualArrow.transform.position.z);
                break;
            case 1:
                manualArrow.transform.position = new Vector3(manualArrow.transform.position.x, manualCircleButtonY.transform.position.y, manualArrow.transform.position.z);
                break;
            case 2:
                manualArrow.transform.position = new Vector3(manualArrow.transform.position.x, manualCircleButtonZ.transform.position.y, manualArrow.transform.position.z);
                break;
            case 3:
                manualArrow.transform.position = new Vector3(manualArrow.transform.position.x, manualCircleButtonRot.transform.position.y, manualArrow.transform.position.z);
                break;
            case 4:
                manualArrow.transform.position = new Vector3(manualArrow.transform.position.x, manualCircleButtonScale.transform.position.y, manualArrow.transform.position.z);
                break;
            case 5:
                manualArrow.transform.position = new Vector3(manualArrow.transform.position.x, manualCircleButtonId.transform.position.y, manualArrow.transform.position.z);
                break;
        }
    }

    /// <summary>
    /// Destroy the drawn virtual object world coordinate system.
    /// </summary>
    /// <param name="go">Corresponding virtual object.</param>
    public void destroyAxis(GameObject go)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            if (go.transform.GetChild(i).name.Contains("Axis"))
            {
                go.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Draws the virtual object world coordinate system.
    /// </summary>
    /// <param name="go">Corresponding virtual object.</param>
    public void drawAxis(GameObject go)
    {
        if (manualButton.GetComponentInChildren<Text>().text.Contains("Hide"))
        {
            for(int i = 0; i < go.transform.childCount; i++)
            {
                if (go.transform.GetChild(i).name.Contains("Axis"))
                {
                    go.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Shows the manual menu and changes the corresponding button text.
    /// </summary>
    void show_menu()
    {
        if (manualButton.GetComponentInChildren<Text>().text.Contains("Show"))
        {
            manualMenu.SetActive(true);
            manualButton.GetComponentInChildren<Text>().text = "Hide Manual";
            mainScript.drawSelectedObjectAxis();
        }
        else
        {
            manualMenu.SetActive(false);
            manualButton.GetComponentInChildren<Text>().text = "Show Manual";
            mainScript.destroySelectedObjectAxis();
        }
    }

    /// <summary>
    /// Returns to the previous menu.
    /// </summary>
    void back()
    {
        mainScript.changeMenu("configure_pre_menu");
        mainScript.eraseOnBack();
        handleShowText(false);
    }

    /// <summary>
    /// Saves the anchor.
    /// </summary>
    void host()
    {
        mainScript.HostCloudAnchor();
    }

    /// <summary>
    /// Change to the next virtual object.
    /// </summary>
    void changePrefabRight()
    {
        mainScript.ChangePrefab(1);
    }

    /// <summary>
    /// Change to the previous virtual object.
    /// </summary>
    void changePrefabLeft()
    {
        mainScript.ChangePrefab(-1);
    }

    /// <summary>
    /// Change the instruction text and its color.
    /// </summary>
    /// <param name="text">The substitute text.</param>
    /// <param name="textColor">The substitute text color.</param>
    public void changeText(string text, Color textColor)
    {
        instructionText.text = text;
        instructionText.color = textColor;
    }

    /// <summary>
    /// Change the hosting UI buttons interactability.
    /// </summary>
    /// <param name="permission">Is the hosting possible?</param>
    public void enableHosting(bool permission, bool all = true)
    {
        hostButton.interactable = permission;
        rightArrowButton.interactable = permission;
        leftArrowButton.interactable = permission;
        if(all)
            attachNewObjectButton.interactable = permission;
        removeButton.interactable = permission;
        sliderScaleRot.interactable = permission;
    }

    /// <summary>
    /// Enables the placement of a new anchor.
    /// </summary>
    void addNewAnchor()
    {
        mainScript.EnableNewAnchor();
    }

    /// <summary>
    /// Enables the attachment of a new object to the corresponding anchor.
    /// </summary>
    void attachNewObject()
    {
        mainScript.EnableNewObject();
    }

    /// <summary>
    /// Change the UI button interactability for adding a new anchor.
    /// </summary>
    /// <param name="interact">Is it possible to add a new anchor?</param>
    public void changeAnchorButtonInteract(bool interact)
    {
        addNewAnchorButton.interactable = interact;
    }

    /// <summary>
    /// Gets the instruction text color.
    /// </summary>
    /// <returns>The instruction text color.</returns>
    public Color getTextColor()
    {
        return instructionText.color;
    }

    /// <summary>
    /// Change the UI button interactability for adding a new object.
    /// Used for usability tests.
    /// </summary>
    /// <param name="interact">Is it possible to add a new object?</param>
    public void changeAttachInteractable(bool permission)
    {
        attachNewObjectButton.interactable = permission;
    }

    /// <summary>
    /// Usability ending text.
    /// </summary>
    /// <param name="show">Enable usability ending text.</param>
    public void handleShowText(bool show)
    {
        endText.gameObject.SetActive(show);
        if (!show)
        {
            numObjectsText.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// Usability experience - Show the number of object placed and remaining.
    /// </summary>
    /// <param name="numObjs">Number of placed objects.</param>
    public void changeNumObjectsText(int numObjs)
    {
        if (!numObjectsText.gameObject.activeSelf)
        {
            numObjectsText.gameObject.SetActive(true);
        }
        numObjectsText.text = numObjs.ToString() + "/4";
    }
}