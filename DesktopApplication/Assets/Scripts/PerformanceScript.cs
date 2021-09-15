// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the performance visualization window.
//               Contains all the button listeners.
// SPECIAL NOTES:
// ===============================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceScript : MonoBehaviour
{
    /// <summary>
    /// UI with behaviour.
    /// </summary>
    public Button startButton;
    public Button backButton;
    public Button replayButton;
    public Text infoText;
    public Text scoreText;
    public Text scoreTextFeedback;

    // <summary>
    // Manual Mode UI with behaviour.
    // </summary>
    public Button manualButton;
    public InputField manualInputField;
    public Button okButton;
    public Text xText;
    public Text yText;
    public Text zText;
    public Text rotText;
    public Image manualArrow;
    public Button manualButtonMore;
    public Button manualButtonLess;
    public Button manualCircleButtonX;
    public Button manualCircleButtonY;
    public Button manualCircleButtonZ;
    public Button manualCircleButtonRot;
    public GameObject manualMenu;

    /// <summary>
    /// Starting object.
    /// </summary>
    private GameObject aux_go;

    /// <summary>
    /// Starting object prefab.
    /// </summary>
    public GameObject startingPrefab;

    /// <summary>
    /// Perfomance visualization main menu.
    /// </summary>
    public GameObject performanceMainMenu;

    /// <summary>
    /// Connection to the performance visualization menu script.
    /// </summary>
    public perfMainMenuScript perfomanceMainMenuScript;

    /// <summary>
    /// Connection to the performance visualization reader script.
    /// </summary>
    public PerformanceFileReader performanceFileReader;

    /// <summary>
    /// Unity Start function
    /// </summary>
    void Start()
    {
        backButton.onClick.AddListener(back);
        startButton.onClick.AddListener(start);
        replayButton.onClick.AddListener(replay);

        // Manual at start 
        manualButton.onClick.AddListener(show_menu);
        manualCircleButtonX.onClick.AddListener(delegate { selectText(0); });
        manualCircleButtonY.onClick.AddListener(delegate { selectText(1); });
        manualCircleButtonZ.onClick.AddListener(delegate { selectText(2); });
        manualCircleButtonRot.onClick.AddListener(delegate { selectText(3); });
        manualButtonMore.onClick.AddListener(delegate { changeManualMoreOrLess(true); });
        manualButtonLess.onClick.AddListener(delegate { changeManualMoreOrLess(false); });
        okButton.onClick.AddListener(changeByInput);
    }

    /// <summary>
    /// Unity Update function.
    /// </summary>
    private void Update()
    {
        if(aux_go != null)
        {
            changeManualTexts(aux_go.transform.position.x, aux_go.transform.position.y, aux_go.transform.position.z, aux_go.transform.rotation.eulerAngles.y);
        }
    }

    /// <summary>
    /// Repeat the performance visualization for the chosen file.
    /// </summary>
    private void replay()
    {
        performanceFileReader.eraseSimulation();
        performanceFileReader.replay();
        change_replay_active(false);
    }

    /// <summary>
    /// Returns to the previous menu or the previous mode.
    /// </summary>
    private void back()
    {
        this.gameObject.SetActive(false);
        performanceMainMenu.SetActive(true);
        if (aux_go != null)
        {
            Destroy(aux_go);
        }
        perfomanceMainMenuScript.erase_plan();
        change_replay_active(false);
        performanceFileReader.eraseSimulation();
        changeInfoText(0,0,0,0);
        changeScoreText("", Color.black);
    }

    /// <summary>
    /// Start the performance visualization.
    /// </summary>
    private void start()
    {
        performanceFileReader.setPose(aux_go.transform.position, aux_go.transform.rotation.eulerAngles);
        Destroy(aux_go);
        manualButton.gameObject.SetActive(false);
        manualMenu.SetActive(false);
        startButton.gameObject.SetActive(false);
        string[] file_lines = File.ReadAllLines("Assets/Monitorization/" + perfomanceMainMenuScript.getFileName() + ".txt");
        performanceFileReader.perform(file_lines);
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
            for (int i = 0; i < aux_go.transform.childCount; i++)
            {
                if (aux_go.transform.GetChild(i).name.Contains("Axis"))
                {
                    aux_go.transform.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            manualMenu.SetActive(false);
            manualButton.GetComponentInChildren<Text>().text = "Show Manual";
            for (int i = 0; i < aux_go.transform.childCount; i++)
            {
                if (aux_go.transform.GetChild(i).name.Contains("Axis"))
                {
                    aux_go.transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    // <summary>
    // Position the arrow at the button clicked.
    // </summary>
    // <param name="index">Button clicked index. (Higher yy, lower index)</param>
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
        }
    }

    // <summary>
    // Translate, rotate or scale the virtual object for the next position, rotation or scale. (0.1f, 1f, 0.1f)
    // </summary>
    // <param name="moreOrLess">Negative or position increment.</param>
    void changeManualMoreOrLess(bool moreOrLess)
    {
        float value;
        if (moreOrLess)
        {
            value = 0.1f;
        }
        else
        {
            value = -0.1f;
        }
        if (Mathf.RoundToInt(manualArrow.transform.position.y) == Mathf.RoundToInt(manualCircleButtonX.transform.position.y))
        {
            aux_go.transform.Translate(new Vector3(value, 0, 0));
        }
        else if (Mathf.RoundToInt(manualArrow.transform.position.y) == Mathf.RoundToInt(manualCircleButtonY.transform.position.y))
        {
            aux_go.transform.Translate(new Vector3(0, value, 0));
        }
        else if (Mathf.RoundToInt(manualArrow.transform.position.y) == Mathf.RoundToInt(manualCircleButtonZ.transform.position.y))
        {
            aux_go.transform.Translate(new Vector3(0, 0, value));
        }
        else if (Mathf.RoundToInt(manualArrow.transform.position.y) == Mathf.RoundToInt(manualCircleButtonRot.transform.position.y))
        {
            if(moreOrLess)
            {
                aux_go.transform.Rotate(0, 1, 0);
            }
            else
            {
                aux_go.transform.Rotate(0, -1, 0);
            }
        }
    }

    // <summary>
    // Changes the virtual object position, rotation or scale for the new value inserted on the input field.
    // </summary>
    void changeByInput()
    {
        float value = (float)Convert.ToDouble(manualInputField.text);

        if (Mathf.RoundToInt(manualArrow.transform.position.y) == Mathf.RoundToInt(manualCircleButtonX.transform.position.y))
        {
            aux_go.transform.Translate(new Vector3(value - aux_go.transform.position.x, 0, 0));
        }
        else if (Mathf.RoundToInt(manualArrow.transform.position.y) == Mathf.RoundToInt(manualCircleButtonY.transform.position.y))
        {
            aux_go.transform.Translate(new Vector3(0, value - aux_go.transform.position.y, 0));
        }
        else if (Mathf.RoundToInt(manualArrow.transform.position.y) == Mathf.RoundToInt(manualCircleButtonZ.transform.position.y))
        {
            aux_go.transform.Translate(new Vector3(0, 0, value - aux_go.transform.position.z));
        }
        else if (Mathf.RoundToInt(manualArrow.transform.position.y) == Mathf.RoundToInt(manualCircleButtonRot.transform.position.y))
        {
            aux_go.transform.Rotate(0, value - aux_go.transform.rotation.eulerAngles.y, 0);
        }
    }

    /// <summary>
    /// Unity OnEnable function.
    /// </summary>
    public void OnEnable()
    {
        if (aux_go == null)
        {
            aux_go = Instantiate(startingPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            aux_go.AddComponent<PathHandler>().id = 0;
        }
        if (!manualButton.gameObject.activeSelf)
        {
            manualButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Change game information text: score, time and number of checkpoints caught.
    /// </summary>
    public void changeInfoText(int score, int num_checkpoints, int minutes, int seconds)
    {
        infoText.text = "Score: " + score.ToString() + ", Num_Checkpoints: " + num_checkpoints.ToString() + ", Time: " + minutes.ToString("00") + ":" + seconds.ToString("00"); 
    }

    /// <summary>
    /// Change the numeric score feedback text.
    /// </summary>
    /// <param name="color">Score given/taken.</param>
    /// <param name="score">Feedback text color.</param>
    public void changeScoreText(string score, Color color)
    {
        scoreText.text = score;
        scoreText.color = color;
    }

    /// <summary>
    /// Change the feedback text.
    /// </summary>
    /// <param name="score">Feedback text.</param>
    /// <param name="color">Feedback text color.</param>
    public void changeScoreFeedbackText(string score, Color color)
    {
        scoreTextFeedback.text = score;
        scoreTextFeedback.color = color;
    }

    // <summary>
    // Changes the values on the manual menu UI texts.
    // </summary>
    // <param name="x">the virtual object x-coordinate.</param>
    // <param name="y">the virtual object y-coordinate.</param>
    // <param name="z">the virtual object z-coordinate.</param>
    // <param name="rot">the virtual object yy rotation.</param>
    public void changeManualTexts(float x, float y, float z, float rot)
    {
        xText.text = "X: " + Math.Round(x, 1).ToString("0.0");
        yText.text = "Y: " + Math.Round(y, 1).ToString("0.0");
        zText.text = "Z: " + Math.Round(z, 1).ToString("0.0");
        rotText.text = "Rotation: " + ((int)rot).ToString("0");
    }

    /// <summary>
    /// Change the replay button interactivity. (At the end of the performance visualization.
    /// </summary>
    public void change_replay_active(bool active)
    {
        replayButton.gameObject.SetActive(active);
    }
}