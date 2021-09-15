// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Create the editor window for scaling the game.
// SPECIAL NOTES:
// ===============================

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScaleGame : EditorWindow
{
    // <summary>
    // Increase scale.
    // </summary>
    [SerializeField] bool scaleDistanceEnabled = true;

    // <summary>
    // Increase scale value.
    // </summary>
    [SerializeField] float scaleDistanceValueI = 1;

    // <summary>
    // Decrease scale value.
    // </summary>
    [SerializeField] float scaleDistanceValueD = 1;

    // <summary>
    // Previous scale value from the GameObject created for scaling purposes.
    // </summary>
    [SerializeField] Vector3 previousScale = new Vector3(1,1,1);

    // <summary>
    // Show the plan image for scaling comparison purposes.
    // </summary>
    [SerializeField] bool planCompare = false;

    // <summary>
    // Plan path.
    // </summary>
    [SerializeField] string planPath = "House";

    // <summary>
    // Plan width.
    // </summary>
    [SerializeField] float planX = 1;

    // <summary>
    // Plan height.
    // </summary>
    [SerializeField] float planY = 1;

    // <summary>
    // Create the scaling window on Unity Editor Menu.
    // </summary>
    [MenuItem("ARCore game library/Scale window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ScaleGame window = (ScaleGame)EditorWindow.GetWindow(typeof(ScaleGame));
        window.Show();
    }

    // <summary>
    // Unity OnGUI function.
    // </summary>
    void OnGUI()
    {
        //Scaling GUI
        //Create and obtain values from the scaling window
        GUILayout.Label("Scale Settings", EditorStyles.boldLabel);
        scaleDistanceEnabled = EditorGUILayout.BeginToggleGroup("Increase game scale", scaleDistanceEnabled);
        scaleDistanceValueI = EditorGUILayout.Slider("Scale value:", scaleDistanceValueI, 1, 100);
        EditorGUILayout.EndToggleGroup();
        scaleDistanceEnabled = !EditorGUILayout.BeginToggleGroup("Decrease game scale", !scaleDistanceEnabled);
        scaleDistanceValueD = EditorGUILayout.Slider("Scale value:", scaleDistanceValueD, 0.05f, 1);
        EditorGUILayout.EndToggleGroup();

        planCompare = EditorGUILayout.BeginToggleGroup("Show plan", planCompare);
        planPath = EditorGUILayout.TextField("Plan Resources Path", planPath);
        planX = float.Parse(EditorGUILayout.TextField("Width", planX.ToString()));
        planY = float.Parse(EditorGUILayout.TextField("Height", planY.ToString()));
        EditorGUILayout.EndToggleGroup();

        //Change the scale based on the new new scaling values
        //or create/delete the GameObject plan image for scaling comparion.
        if (GUI.changed)
        {
            // Scaling process: Add all GameObjects to a new one, scale the new one and delete it after scaling.
            GameObject scaling = new GameObject("scaling");
            scaling.transform.localScale = previousScale;
            GameObject[] gosInScene = FindObjectsOfType<GameObject>();
            List<GameObject> gosInTopHierarchy = new List<GameObject>();
            foreach (GameObject go in gosInScene)
            {
                if (go.transform.parent == null && go.name != "Canvas" && go.name != "EventSystem" && go.name != "arcoreHandler" && go.name != "Plan")
                {
                    gosInTopHierarchy.Add(go);
                }
            }
            foreach (GameObject go in gosInTopHierarchy)
            {
                go.transform.SetParent(scaling.transform);
            }

            if (scaleDistanceEnabled)
            {
                scaling.transform.localScale = new Vector3(scaleDistanceValueI, scaleDistanceValueI, scaleDistanceValueI);
            }
            else
            {
                scaling.transform.localScale = new Vector3(scaleDistanceValueD, scaleDistanceValueD, scaleDistanceValueD);
            }
            previousScale = scaling.transform.localScale;

            foreach (GameObject go in gosInTopHierarchy)
            {
                go.transform.SetParent(null);
            }
            DestroyImmediate(scaling);

            // Plane handler - Create or erase the plan image.
            if (planCompare)
            {
                GameObject plan = GameObject.Find("Plan");
                if (plan == null)
                {
                    plan = new GameObject("Plan");
                    plan.transform.Rotate(90, 0, 0);
                    plan.AddComponent<SpriteRenderer>();
                    Sprite sp = LoadSprite("Assets/Resources/Plan/" + planPath);
                    if (sp != null)
                    {
                        plan.GetComponent<SpriteRenderer>().sprite = sp;
                        change_plan_size();
                        plan.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
                        plan.transform.GetComponent<SpriteRenderer>().size = new Vector2(planX, planY);
                    }
                }
                else
                {
                    if (plan.GetComponent<SpriteRenderer>().sprite != null)
                    {
                        if (plan.GetComponent<SpriteRenderer>().sprite.name != planPath.Replace(".jpg", "").Replace(".png", ""))
                        {
                            plan.GetComponent<SpriteRenderer>().sprite = null;
                            Sprite sp = LoadSprite("Assets/Resources/Plan/" + planPath);
                            if (sp != null)
                            {
                                plan.GetComponent<SpriteRenderer>().sprite = sp;
                                change_plan_size();
                                plan.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
                                plan.transform.GetComponent<SpriteRenderer>().size = new Vector2(planX, planY);
                            }
                        }
                        else
                        {
                            plan.transform.GetComponent<SpriteRenderer>().size = new Vector2(planX, planY);
                        }
                    }
                    else
                    {
                        Sprite sp = LoadSprite("Assets/Resources/Plan/" + planPath);
                        if (sp != null)
                        {
                            plan.GetComponent<SpriteRenderer>().sprite = sp;
                            change_plan_size();
                            plan.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
                            plan.transform.GetComponent<SpriteRenderer>().size = new Vector2(planX, planY);
                        }
                    }
                }
            }
            else
            {
                GameObject plan = GameObject.Find("Plan");
                if (plan != null)
                {
                    DestroyImmediate(plan);
                }
            }
        }
    }

    // <summary>
    // Load Sprite image.
    // </summary>
    // <param name="path">Sprite image path.</param>
    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sprite.name = planPath.Replace(".jpg", "").Replace(".png", "");
            return sprite;
        }
        return null;
    }

    // <summary>
    // Change the plan image proportions.
    // </summary>
    public void change_plan_size()
    {
        bool size_in_file = false;
        string[] plan_size_file = File.ReadAllLines("Assets/Resources/Plan/plan_sizes.txt");
        foreach (var line in plan_size_file)
        {
            if (line.Split(':')[0] == planPath.Replace(".jpg", "").Replace(".png", ""))
            {
                planX = float.Parse(line.Substring(line.IndexOf("x=") + 2, line.IndexOf(",") - line.IndexOf("x=") - 2));
                planY = float.Parse(line.Substring(line.IndexOf("y=") + 2, line.Length - line.IndexOf("y=") - 2));
                size_in_file = true;
                break;
            }
        }
        if (!size_in_file)
        {
            GameObject plan = GameObject.Find("Plan");
            planX = plan.transform.GetComponent<SpriteRenderer>().size.x;
            planY = plan.transform.GetComponent<SpriteRenderer>().size.y;
        }
    }

    // <summary>
    // Unity OnEnable function.
    // </summary>
    protected void OnEnable()
    {
        // Here we retrieve the data if it exists or we save the default field initialisers we set above
        var data = EditorPrefs.GetString("ScaleWindow", JsonUtility.ToJson(this, false));
        // Then we apply them to this window
        JsonUtility.FromJsonOverwrite(data, this);
    }

    // <summary>
    // Unity OnDisable function.
    // </summary>
    protected void OnDisable()
    {
        // We get the Json data
        var data = JsonUtility.ToJson(this, false);
        // And we save it
        EditorPrefs.SetString("ScaleWindow", data);
    }
}
