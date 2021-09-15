using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaleGame : EditorWindow
{
    [SerializeField] bool scaleDistanceEnabled = true;
    [SerializeField] float scaleDistanceValueI = 1;
    [SerializeField] float scaleDistanceValueD = 1;
    [SerializeField] Vector3 previousScale = new Vector3(1,1,1);

    [SerializeField] bool planCompare = false;
    [SerializeField] string planPath = "House";
    [SerializeField] float planX = 1;
    [SerializeField] float planY = 1;

    [MenuItem("ARCore game library/Scale window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ScaleGame window = (ScaleGame)EditorWindow.GetWindow(typeof(ScaleGame));
        window.Show();
    }

    void OnGUI()
    {
        //Scaling GUI
        GUILayout.Label("Scale Settings", EditorStyles.boldLabel);
        scaleDistanceEnabled = EditorGUILayout.BeginToggleGroup("Increase gamescale", scaleDistanceEnabled);
        scaleDistanceValueI = EditorGUILayout.Slider("Scale value:", scaleDistanceValueI, 1, 20);
        EditorGUILayout.EndToggleGroup();
        scaleDistanceEnabled = !EditorGUILayout.BeginToggleGroup("Decrease game scale", !scaleDistanceEnabled);
        scaleDistanceValueD = EditorGUILayout.Slider("Scale value:", scaleDistanceValueD, 0.05f, 1);
        EditorGUILayout.EndToggleGroup();

        planCompare = EditorGUILayout.BeginToggleGroup("Show plan", planCompare);
        planPath = EditorGUILayout.TextField("Plan Resources Path", planPath);
        planX = float.Parse(EditorGUILayout.TextField("Plan Resources Path", planX.ToString()));
        planY = float.Parse(EditorGUILayout.TextField("Plan Resources Path", planY.ToString()));
        EditorGUILayout.EndToggleGroup();


        if (GUI.changed)
        {
            // Scaling process: Add all GameObjects to a new one, scale the new one and delete it after scaling
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

            // Plane handler
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
                if (gosInTopHierarchy.Contains(plan))
                {
                    DestroyImmediate(plan);
                }
            }
        }
    }

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

    protected void OnEnable()
    {
        // Here we retrieve the data if it exists or we save the default field initialisers we set above
        var data = EditorPrefs.GetString("ScaleWindow", JsonUtility.ToJson(this, false));
        // Then we apply them to this window
        JsonUtility.FromJsonOverwrite(data, this);
    }

    protected void OnDisable()
    {
        // We get the Json data
        var data = JsonUtility.ToJson(this, false);
        // And we save it
        EditorPrefs.SetString("ScaleWindow", data);

        // Et voilà !
    }
}
