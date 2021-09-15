// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the play window.
//               Contains all the button listeners.
// SPECIAL NOTES:
// ===============================

using UnityEngine;
using UnityEngine.UI;

public class PlayMenuThesisScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public Button calibrateButton;
    public Button backButton;
    public Button helpButton;
    public Button enableMovButton;
    public Text debugText1;
    public Text timeText;
    public Text checkPointsText;
    public GameObject instructionPanel;
    public Text instructionText;
    public Text feedbackText;
    public Text feedbackText2;
    public Text scoreText;

    /// <summary>
    /// Connection to the main script (gameplay).
    /// </summary>
    public Gameplay mainScript;

    /// <summary>
    /// Motivation words.
    /// </summary>
    protected string[] motivation = { "Awesome!", "Well done!", "Excellent!", "Very good!"};

    /// <summary>
    /// Motivation word time counter.
    /// </summary>
    float timer = 0;

    /// <summary>
    /// Saves the last instruction text.
    /// Has the objective to verify if the instruction text was changed.
    /// </summary>
    private string last_text = "";

    /// <summary>
    /// Canvas for scaling purposes.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// White image representing the flash light when the user fails the spotlight.
    /// </summary>
    public Image flashLightImage;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        calibrateButton.onClick.AddListener(calibrate);
        backButton.onClick.AddListener(back);
        helpButton.onClick.AddListener(show_help);
        enableMovButton.onClick.AddListener(enable_without_anyfind);
        this.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        flashLightImage.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Unity Update function.
    /// </summary>
    private void Update()
    {
        //Make the feedback text more transparent over time.
        if(timer < 2.5)
        {
            timer += Time.deltaTime;
            Color32 auxC = feedbackText.color;
            auxC.a = (byte)(auxC.a - (int)(Time.deltaTime * (255/3)));
            feedbackText.color = auxC;
            auxC = feedbackText2.color;
            auxC.a = (byte)(auxC.a - (int)(Time.deltaTime * (255 / 3)));
            feedbackText2.color = auxC;
        }
        else
        {
            if (feedbackText.IsActive())
            {
                feedbackText.gameObject.SetActive(false);
            }
            if (feedbackText2.IsActive())
            {
                feedbackText2.gameObject.SetActive(false);
            }
        }
        // Verify if the the instruction text changed to make the help button blink.
        if(instructionText.text != last_text)
        {
            last_text = instructionText.text;
            if (!IsInvoking("blinkHelpButton"))
            {
                Invoke("blinkHelpButton", 0.9f);
            }
        }
    }

    /// <summary>
    /// Show/Hide the instruction text.
    /// Also stops the blinking on the help button.
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
    /// Make/stop the help button blink.
    /// </summary>
    private void blinkHelpButton()
    {
        if (helpButton.GetComponent<Image>().color == new Color32(255, 255, 255, 255) || helpButton.GetComponent<Image>().color == Color.red)
        {
            helpButton.GetComponent<Image>().color = new Color32(47, 200, 255, 255);
        }
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
    /// Change the calibration button interactivity -> Deprecated.
    /// </summary>
    public void changeCalibrationActive()
    {
        calibrateButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Handles the feedback text.
    /// </summary>
    /// <param name="positive">Is positive or negative feedback</param>
    /// <param name="reduceScorePoints">Score to subtract in a negative feedback.</param>
    public void changeFeedbackText(bool positive, string reduceScorePoints="100")
    {
        if (positive)
        {
            feedbackText.color = Color.green;
            int mIndex = Random.Range(0, motivation.Length);
            feedbackText.text = motivation[mIndex];
            feedbackText.gameObject.SetActive(true);
            timer = 0;
            Color32 auxC = feedbackText.color;
            auxC.a = 255;
            feedbackText.color = auxC;
        }
        else
        {
            feedbackText.color = Color.red;
            feedbackText.text = "-"+ reduceScorePoints +" Points";
            feedbackText.gameObject.SetActive(true);
            timer = 0;
            Color32 auxC = feedbackText.color;
            auxC.a = 255;
            feedbackText.color = auxC;
        }
    }

    /// <summary>
    /// Handles the 2nd feedback text.
    /// Represents the number of checkpoints caught in comparison with the total number of checkpoints.
    /// </summary>
    /// <param name="num_checkpoints_done">Number of checkpoints caught.</param>
    /// <param name="total_num_of_checkpoints">Total number of checkpoints.</param>
    public void changeFeedbackText2(int num_checkpoints_done, int total_num_of_checkpoints)
    {
        if (num_checkpoints_done != total_num_of_checkpoints)
        {
            feedbackText2.text = num_checkpoints_done.ToString() + "/" + total_num_of_checkpoints.ToString();
            feedbackText2.gameObject.SetActive(true);
            Color32 auxC = feedbackText2.color;
            auxC.a = 255;
            feedbackText2.color = auxC;
            changeFeedbackText(true);
        }
    }

    /// <summary>
    /// Calls the main script calibrate function -> Deprecated.
    /// </summary>
    public void calibrate()
    {
        mainScript.calibrate();
    }

    /// <summary>
    /// Change the game score text.
    /// </summary>
    /// <param name="score">Game score.</param>
    public void changeScoreText(int score) {
        scoreText.text = "Score: " + score.ToString();
        if(score >= 0)
        {
            scoreText.color = Color.black;
        }
        else
        {
            scoreText.color = Color.red;
        }
    }

    /// <summary>
    /// Change the game elapsed time text.
    /// </summary>
    /// <param name="minutes">Game elapsed minutes.</param>
    /// <param name="seconds">Game elapsed seconds.</param>
    public void changeTimeText(float minutes, float seconds)
    {
        timeText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    /// <summary>
    /// Change the checkpoint text.
    /// </summary>
    /// <param name="checkpoints">Number of checkpoints caught.</param>
    /// <param name="checkpoints_goal">Total number of checkpoints.</param>
    public void changeCheckPointText(int checkpoints, int checkpoints_goal)
    {
        checkPointsText.text = "Check Points: " + checkpoints.ToString() + "/" + checkpoints_goal.ToString();
    }

    /// <summary>
    /// Returns to the previous menu.
    /// </summary>
    void back()
    {
        mainScript.changeMenu("play_pre_menu");
        mainScript.eraseOnBack();
    }

    /// <summary>
    /// Change the instruction text.
    /// </summary>
    /// <param name="text">New instruction text.</param>
    /// <param name="textColor">Instruction text color.</param>
    public void changeText(string text, Color textColor)
    {
        instructionText.text = text;
        instructionText.color = textColor;
    }

    /// <summary>
    /// Calls the main script enable_without_anyfind function.
    /// </summary>
    void enable_without_anyfind()
    {
        mainScript.enable_without_anyfind();
        enMovButton(false);
    }

    /// <summary>
    /// Change the "enable moving object before finding a plane" button interactivity.
    /// </summary>
    public void enMovButton(bool en)
    {
        enableMovButton.gameObject.SetActive(en);
    }

    /// <summary>
    /// Show the flash image.
    /// </summary>
    public void showFlashLight()
    {
        flashLightImage.gameObject.SetActive(true);
        Invoke("hideFlashLight", 1f);
    }

    /// <summary>
    /// Hide the flash image.
    /// </summary>
    private void hideFlashLight()
    {
        flashLightImage.gameObject.SetActive(false);
    }
}