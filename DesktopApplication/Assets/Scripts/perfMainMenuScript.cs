// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the performance visualization menu.
// SPECIAL NOTES:
// ===============================

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class perfMainMenuScript : MonoBehaviour
{
    /// <summary>
    /// UI with behavior.
    /// </summary>
    public InputField fileInputField;
    public Button backButton;
    public Button startButton;
    public InputField xPlanIF;
    public InputField yPlanIF;

    /// <summary>
    /// Dropdown containing a list of plan images.
    /// </summary>
    public Dropdown planDropDown;

    /// <summary>
    /// Existing plans in the Assets/Plans folder
    /// </summary>
    private FileInfo[] Plans;

    /// <summary>
    /// Plan size
    /// </summary>
    private string[] plan_size_file;
    private float plan_x = 0;
    private float plan_y = 0;

    /// <summary>
    /// Navegation menus.
    /// </summary>
    public GameObject initialMenu;
    public GameObject performanceMenu;

    /// <summary>
    /// Main camera.
    /// </summary>
    public GameObject main_camera;

    /// <summary>
    /// Plan object.
    /// </summary>
    public GameObject plan;

    /// <summary>
    /// File name.
    /// </summary>
    private string fileName;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        //Obtain the plans for scale comparison from Assets/AnchorFiles folder
        planDropDown.options.Clear();
        DirectoryInfo plansDirectory = new DirectoryInfo("Assets/Plans");
        Plans = plansDirectory.GetFiles("*.png");
        foreach (FileInfo plan in Plans)
        {
            planDropDown.options.Add(new Dropdown.OptionData() { text = plan.Name.Replace(".png", "") });
        }
        planDropDown.RefreshShownValue();
        planDropDown.onValueChanged.AddListener(delegate { change_plan_size(); });

        //Obtain the plan size
        plan_size_file = File.ReadAllLines("Assets/Plans/plan_sizes.txt");
        foreach(var line in plan_size_file)
        {
            if (line.Split(':')[0] == planDropDown.options[planDropDown.value].text)
            {
                xPlanIF.placeholder.GetComponent<Text>().text = "X = "+line.Substring(line.IndexOf("x=")+2, line.IndexOf(",") - line.IndexOf("x=")-2) + "(m)";
                plan_x = float.Parse(line.Substring(line.IndexOf("x=")+2, line.IndexOf(",") - line.IndexOf("x=")-2));
                yPlanIF.placeholder.GetComponent<Text>().text = "Y = " + line.Substring(line.IndexOf("y=")+2, line.Length - line.IndexOf("y=")-2) + "(m)";
                plan_y = float.Parse(line.Substring(line.IndexOf("y=")+2, line.Length - line.IndexOf("y=")-2));
                break;
            }
        }

        startButton.onClick.AddListener(start);
        backButton.onClick.AddListener(back);
    }

    /// <summary>
    /// Returns to the previous menu or the previous mode.
    /// </summary>
    private void back()
    {
        this.gameObject.SetActive(false);
        initialMenu.SetActive(true);
    }

    /// <summary>
    /// Change the plan image size.
    /// </summary>
    public void change_plan_size()
    {
        foreach (var line in plan_size_file)
        {
            if (line.Split(':')[0] == planDropDown.options[planDropDown.value].text)
            {
                xPlanIF.placeholder.GetComponent<Text>().text = "X = " + line.Substring(line.IndexOf("x=") + 2, line.IndexOf(",") - line.IndexOf("x=") - 2) + "(m)";
                plan_x = float.Parse(line.Substring(line.IndexOf("x=") + 2, line.IndexOf(",") - line.IndexOf("x=") - 2));
                yPlanIF.placeholder.GetComponent<Text>().text = "Y = " + line.Substring(line.IndexOf("y=") + 2, line.Length - line.IndexOf("y=") - 2) + "(m)";
                plan_y = float.Parse(line.Substring(line.IndexOf("y=") + 2, line.Length - line.IndexOf("y=") - 2));
                break;
            }
        }
    }

    /// <summary>
    /// Start the perfomance visualization, changing the window.
    /// Transfer the plan and the perfomance file to the main application.
    /// </summary>
    void start()
    {
        activate(true);
        string path = "Assets/Plans/" + Plans[planDropDown.value].Name;
        plan.GetComponent<SpriteRenderer>().sprite = LoadSprite(path);
        if (!string.IsNullOrEmpty(xPlanIF.text) || !string.IsNullOrEmpty(yPlanIF.text))
        {
            plan.transform.GetComponent<SpriteRenderer>().size = new Vector2(float.Parse(xPlanIF.text), float.Parse(yPlanIF.text));
        }
        else
        {
            plan.transform.GetComponent<SpriteRenderer>().size = new Vector2(plan_x, plan_y);
        }

        fileName = fileInputField.text;

        this.gameObject.SetActive(false);
        performanceMenu.SetActive(true);
    }

    /// <summary>
    /// Load sprite image from path.
    /// </summary>
    /// <param name="path">Image path.</param>
    /// <returns>Sprite image.</returns>
    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }

    /// <summary>
    /// Enable/Disable some relevant components for the performance visualization.
    /// </summary>
    public void activate(bool activate)
    {
        main_camera.GetComponent<CameraMovement>().enabled = activate;
        plan.GetComponent<SpriteRenderer>().enabled = activate;
    }

    /// <summary>
    /// Get the chosen file containg the game performance.
    /// </summary>
    /// <returns>The chosen file containg the game perfomance.</returns>
    public string getFileName()
    {
        return fileName;
    }

    /// <summary>
    /// Remove the plan image from the scene.
    /// </summary>
    public void erase_plan()
    {
        plan.GetComponent<SpriteRenderer>().enabled = false;
    }
}