// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Main script - Configuration and game logics.
//                           - Navigation between menus.
// SPECIAL NOTES: Communicates with every menu script.
// ===============================

using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.CrossPlatform;
using UnityEngine.EventSystems;
using System;
using Lean.Touch;
using System.IO;

public class Gameplay : MonoBehaviour
{
    /// <summary>
    /// Application modes.
    /// </summary>
    enum Modes
    {
        ///<summary>Initial menu mode.</summary>
        INITIAL,
        ///<summary>Configure menu mode.</summary>
        PRE_CONFIGURE,
        ///<summary>Configure for the cloud anchors operating mode.</summary>
        CONFIGURE_CLOUD,
        ///<summary>Configure for the motion tracking operating mode.</summary>
        CONFIGURE_FILE,
        ///<summary>Resolving achors and virtual objects for the motion tracking operating mode and the game with static objects.</summary>
        RESOLVING_FILE_STATIC,
        ///<summary>Resolving achors and virtual objects for the motion tracking operating mode and the game with the moving object.</summary>
        RESOLVING_FILE_MOVING,
        ///<summary>Resolving achors and virtual objects for the cloud anchors operating mode and the game with static objects.</summary>
        RESOLVING_CLOUD_STATIC,
        ///<summary>Resolving achors and virtual objects for the cloud anchors operating mode and the game with the moving object.</summary>
        RESOLVING_CLOUD_MOVING,
        ///<summary>Playing the game with the cloud anchors operating mode and the moving object.</summary>
        PLAY_CLOUD_MOVING,
        ///<summary>Playing the game with the cloud anchors operating mode and the static objects.</summary>
        PLAY_CLOUD_STATIC,
        ///<summary>Playing the game with the motion tracking operating mode and the moving object.</summary>
        PLAY_FILE_MOVING,
        ///<summary>Playing the game with the motion tracking operating mode and the static objects.</summary>
        PLAY_FILE_STATIC,
        ///<summary>Experience mode.</summary>
        EXPERIENCE,
        ///<summary>Play menu mode.</summary>
        PRE_PLAY,
        ///<summary>End menu mode.</summary>
        END,
        ///<summary>Settings menu mode.</summary>
        SETTINGS,
        ///<summary>User manual menu mode.</summary>
        USER_MANUAL
    };
    Modes mode;

    /// <summary>
    /// Menu windows.
    /// </summary>
    public GameObject initialMenu_v2;
    public GameObject configureMenu;
    public GameObject playThesisMenu;
    public GameObject experienceMenu;
    public GameObject configurePreMenu;
    public GameObject playPreMenu;
    public GameObject endMenu;
    public GameObject settingsMenu;
    public GameObject userManualPreMenu;
    public GameObject userManualWheelchairMenu;
    public GameObject userManualConfigureMenu;
    public GameObject userManualPlayMenu;

    /// <summary>
    /// Canvas object.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Menu scripts.
    /// </summary>
    public InitialMenuScript_v2 initialMenuScript;
    public ConfigureMenuScript confMenuScript;
    public PlayMenuThesisScript playMenuThesisScript;
    public ConfigurePreMenuScript confPreMenuScript;
    public PlayPreMenuScript playPreMenuScript;
    public EndMenuScript endMenuScript;
    public SettingsMenuScript settingsMenuScript;

    /// <summary>
    /// Rotation dictionary - For saving the applied rotation over the virtual objects.
    /// </summary>
    Dictionary<GameObject, float> firstObjectRotation = new Dictionary<GameObject, float>();

    /// <summary>
    /// Prefabs script, contains every game prefab.
    /// </summary>
    public Prefabs prefabs;

    /// <summary>
    /// Enable the moving object without finding the first anchor.
    /// </summary>
    bool enable_without_anyfind_var = false;

    /// <summary>
    /// List of anchors that are still to resolve.
    /// </summary>
    List<MyThesisHistory> anchorsToResolve = new List<MyThesisHistory>();

    /// <summary>
    /// Calibrating purposes - Testing phase.
    /// </summary>
    Dictionary<GameObject, Vector3> adjustPosition = new Dictionary<GameObject, Vector3>();

    /// <summary>
    /// Placed anchor while configuring the scenario.
    /// </summary>
    Anchor anchor;

    /// <summary>
    /// Placed anchor while configuring the scenario - Variable for deleting created anchors.
    /// </summary>
    List<Anchor> anchorsResolvedFile = new List<Anchor>();

    /// <summary>
    /// Anchor GameObject.
    /// </summary>
    GameObject gameObjectToHost;

    /// <summary>
    /// Game timer variables.
    /// </summary>
    float timer = 0;
    int last_time;

    /// <summary>
    /// Game score variables.
    /// </summary>
    int score = 0;
    double sum = 0;
    int num_sum;

    /// <summary>
    /// Checkpoint counter variables.
    /// </summary>
    int checkpointsReached = 0;
    int checkpointsToGoal = 0;

    /// <summary>
    /// Anchor prefab.
    /// </summary>
    public GameObject anchoredPrefab;

    /// <summary>
    /// Moving object prefab.
    /// </summary>
    public GameObject movingObjectPrefab;

    /// <summary>
    /// Moving object - GameObject.
    /// </summary>
    GameObject movingObject;

    /// <summary>
    /// Next moving object desired position.
    /// </summary>
    Vector3 hintToMove;

    /// <summary>
    /// Moving object finished the path.
    /// </summary>
    bool endMovingObjPath = false;

    /// <summary>
    /// Saves the last moving object positions.
    /// for verifying if the distance is increasing, meaning that the game object got to its target position.
    /// </summary>
    List<float> lastDistances = new List<float>();

    /// <summary>
    /// Moving object velocity index.
    /// </summary>
    float movObjVelocity = 0;

    /// <summary>
    /// Moving object current line index.
    /// </summary>
    int linePosition = 0;

    /// <summary>
    /// Moving object auxiliary line index - for speed purposes.
    /// </summary>
    int auxLinePosition = 0;

    /// <summary>
    /// Line positions between each line renderer position.
    /// </summary>
    List<Vector3> line4 = new List<Vector3>();

    /// <summary>
    /// Line length (meters).
    /// </summary>
    float lineLength = 0;

    /// <summary>
    /// Moving object continues its path.
    /// </summary>
    bool continued = false;

    /// <summary>
    /// GameObjects that were alreary resolved. - Game
    /// </summary>
    List<GameObject> resolvedGO = new List<GameObject>();

    /// <summary>
    /// Anchor walls with green quality (1 - 5).
    /// </summary>
    List<string> hostQuality = new List<string>();

    /// <summary>
    /// Configuration mode - screen click was on UI.
    /// </summary>
    bool clickOnUI = false;

    /// <summary>
    /// Current prefab index.
    /// </summary>
    int prefabsIdx = 0;

    /// <summary>
    /// Save the gameobject respective prefabe index.
    /// </summary>
    Dictionary<GameObject, int> respectivePrefabIdx = new Dictionary<GameObject, int>();

    /// <summary>
    /// New anchor placement enabled.
    /// </summary>
    bool enableNewAnchor = false;

    /// <summary>
    /// New virtual object placement enabled.
    /// </summary>
    bool enableNewObject = false;

    /// <summary>
    /// List to save the virtual objects to host.
    /// </summary>
    List<GameObject> listGameObjectsToHost = new List<GameObject>();

    /// <summary>
    /// List to save the virtual objects to host - Erasing purposes.
    /// </summary>
    List<GameObject> listGameObjectsInConfiguration = new List<GameObject>();

    /// <summary>
    /// Verify if the configured changed (new object created, object position changed).
    /// </summary>
    private List<(Vector3, string)> verifyChangesList = new List<(Vector3, string)>();

    /// <summary>
    /// Virtual objects were hosted.
    /// </summary>
    private bool hosted = false;

    /// <summary>
    /// Last frame camera pose.
    /// </summary>
    Pose lastCameraPose;

    /// <summary>
    /// Selection arrow game object.
    /// </summary>
    GameObject selectedArrow;

    /// <summary>
    /// Selection arrow prefab.
    /// </summary>
    public GameObject selectedArrowPrefab;

    /// <summary>
    /// One object is selected.
    /// </summary>
    bool objectSelected;

    /// <summary>
    /// Previous selected object.
    /// </summary>
    GameObject previousSelectedObject;

    /// <summary>
    /// Application persistent data path.
    /// </summary>
    private string fileCoordsAnchorPath;

    /// <summary>
    /// Current scenario.
    /// </summary>
    private string scenario;

    /// <summary>
    /// Game username - server/client purposes.
    /// </summary>
    private string username;

    /// <summary>
    /// White line delimiting the road path.
    /// </summary>
    public LineRenderer lineRenderer;

    /// <summary>
    /// Road path.
    /// </summary>
    public LineRenderer roadLineRenderer;

    /// <summary>
    /// Number of points for the bezier curve between goals.
    /// </summary>
    private int numPoints = 50;

    /// <summary>
    /// Line points position.
    /// </summary>
    private Vector3[] positions;

    /// <summary>
    /// Auxiliar gameobject for the bezier path - representing the starting position.
    /// </summary>
    GameObject aux_go;

    /// <summary>
    /// List of goals - Bezier end points.
    /// </summary>
    List<GameObject> goals = new List<GameObject>();

    /// <summary>
    /// List of arrows - Bezier control points.
    /// </summary>
    List<GameObject> controls = new List<GameObject>();

    /// <summary>
    /// Gameobjects to destroy on back functions.
    /// </summary>
    List<GameObject> gameObjectsToErase = new List<GameObject>();

    /// <summary>
    /// Anchors to destroy on back functions.
    /// </summary>
    List<Anchor> anchorsToErase = new List<Anchor>();

    /// <summary>
    /// Cloud anchors to destroy on back functions.
    /// </summary>
    List<XPAnchor> xpAnchorsToErase = new List<XPAnchor>();

    /// <summary>
    /// The key name used in PlayerPrefs which stores persistent Cloud Anchors history data.
    /// Expired data will be cleared at runtime.
    /// Adapted from ARCore application.
    /// </summary>
    private const string _persistentCloudAnchorsStorageKey = "PersistentCloudAnchors";

    /// <summary>
    /// The limitation of how many Cloud Anchors can be stored in local storage.
    /// Adapted from ARCore application.
    /// </summary>
    private const int _storageLimit = 30;

    /// <summary>
    /// MyThesisHistory script - for saving the cloud anchor and corresponding virtual objects relevant info.
    /// Adapted from ARCore application.
    /// </summary>
    private MyThesisHistory _hostedCloudAnchor;

    /// <summary>
    /// Angle in radians from the virtual object to store and resolve them in the right position relatively to the anchor.
    /// </summary>
    private double angleTillAnchor;

    /// <summary>
    /// Rotation matrix values.
    /// </summary>
    private List<Vector3> rotationMatrix = new List<Vector3>();

    /// <summary>
    /// Explosion prefab when the green cube is catched.
    /// </summary>
    public GameObject explosion;

    /// <summary>
    /// Explosion game object when the green cube is catched.
    /// </summary>
    GameObject explosionObject;

    /// <summary>
    /// Explosion prefab when a collision with a dodge object occurs.
    /// </summary>
    public GameObject dodgeExplosion;

    /// <summary>
    /// Explosion object when a collision with a dodge object occurs.
    /// </summary>
    GameObject dodgeExplosionObject;

    /// <summary>
    /// Client communicating with the server script.
    /// </summary>
    myClient client;

    /// <summary>
    /// Virtual object selected.
    /// </summary>
    GameObject selectedGo;

    /// <summary>
    /// Save each frame position to build the linerenderer at the end of the game.
    /// </summary>
    private List<Vector3> eachFramePosition = new List<Vector3>();

    /// <summary>
    /// Line representing the path traveled by the user.
    /// </summary>
    public LineRenderer linePathDone;

    /// <summary>
    /// The device left the road.
    /// </summary>
    private bool outOfRoad = false;

    /// <summary>
    /// Device position at the stop moment.
    /// </summary>
    Vector3 stoppedPosition;

    /// <summary>
    /// Visualize cloud points - Cloud anchor experience only.
    /// </summary>
    public GameObject pointCloudVisualizer;

    /// <summary>
    ///  Usability Test Script
    /// </summary>
    public UsabilityTestScript usabilityTestScript;

    /// <summary>
    /// Load the persistent Cloud Anchors history from local storage.
    /// Adapted from ARCore.
    /// </summary>
    /// <returns>A collection of persistent Cloud Anchors history data.</returns>
    public MyThesisHistoryCollection LoadCloudAnchorHistory()
    {
        if (PlayerPrefs.HasKey(_persistentCloudAnchorsStorageKey))
        {
            var history = JsonUtility.FromJson<MyThesisHistoryCollection>(
                PlayerPrefs.GetString(_persistentCloudAnchorsStorageKey));
            //history.Collection.OnBeforeSerialize();
            return history;
        }

        return new MyThesisHistoryCollection();
    }

    /// <summary>
    /// Save the persistent Cloud Anchors history to local storage,
    /// also remove the oldest data if current storage has met maximal capacity.
    /// Adapted from ARCore.
    /// </summary>
    /// <param name="data">The Cloud Anchor history data needs to be stored.</param>
    public void SaveCloudAnchorHistory(MyThesisHistory data)
    {
        var history = LoadCloudAnchorHistory();
        // Sort the data from latest record to oldest record which affects the option order in
        // multiselection dropdown.
        history.Collection.Add(data);


        // Remove the oldest data if the capacity exceeds storage limit.
        if (history.Collection.Count > _storageLimit)
        {
            history.Collection.RemoveRange(
                0, _storageLimit - 1);
        }

        PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey, JsonUtility.ToJson(history));
    }

    /// <summary>
    /// Load the saved scenario from local storage.
    /// </summary>
    /// <returns>A list of scenarios for the dropdown UI.</returns>
    public List<string> LoadDropdownHistory()
    {
        if (PlayerPrefs.HasKey(_persistentCloudAnchorsStorageKey))
        {
            var history = JsonUtility.FromJson<MyThesisHistoryCollection>(
                PlayerPrefs.GetString(_persistentCloudAnchorsStorageKey));

            return history.dropdownOptions;
        }

        return new List<string>();
    }

    /// <summary>
    /// Save a new scenario for the dropdown UI.
    /// </summary>
    /// <param name="option">The new scenario to be hosted.</param>
    public void SaveDropdownHistory(string option)
    {
        var history = LoadCloudAnchorHistory();

        history.dropdownOptions.Add(option);

        PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey, JsonUtility.ToJson(history));
    }

    /// <summary>
    /// Remove a scenario from the dropdown UI.
    /// </summary>
    /// <param name="option">The scenario to be removed.</param>
    public void RemoveDropdownHistory(string option)
    {
        var history = LoadCloudAnchorHistory();

        history.dropdownOptions.Remove(option);

        PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey, JsonUtility.ToJson(history));
    }

    /// <summary>
    /// Get desktop scenarios for the play dropdown list
    /// </summary>
    /// /// <returns>A list of scenarios for the dropdown UI.</returns>
    public List<string> LoadDesktopScenarios()
    {
        List<string> desktopScenarios = new List<string>();
        DirectoryInfo filesDirectory = new DirectoryInfo(fileCoordsAnchorPath + "/Output_Desktop_Configuration");
        FileInfo[] anchorFiles = filesDirectory.GetFiles("*.json");
        foreach (FileInfo file in anchorFiles)
        {
            desktopScenarios.Add(file.Name.Replace(".json", ""));
        }
        return desktopScenarios;
    }

    /// <summary>
    /// Unity Start function.
    /// </summary>
    private void Start()
    {
        this.GetComponent<Rigidbody>().detectCollisions = false;
        this.lastCameraPose = new Pose(new Vector3(999, 999, 999), new Quaternion(999, 999, 999, 999));
        fileCoordsAnchorPath = Application.persistentDataPath;

        // Auxiliar gameobject for the bezier path
        aux_go = new GameObject();
    }

    /// <summary>
    /// Unity Update function.
    /// </summary>
    void Update()
    {
        // While the session is not tracking - do nothing
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        //Verify and inform if the device is lost.
        Pose currentPose = new Pose(this.transform.position, this.transform.rotation);
        if(lastCameraPose == currentPose)
        {
            if(mode == Modes.CONFIGURE_FILE || mode == Modes.CONFIGURE_CLOUD)
            {
                confMenuScript.changeText("The location that the device's camera is pointing at does not have enough texture.", Color.red);
            }
            else if(mode == Modes.PLAY_CLOUD_MOVING || mode == Modes.PLAY_CLOUD_STATIC || mode == Modes.PLAY_FILE_MOVING || 
                mode == Modes.PLAY_FILE_STATIC || mode == Modes.RESOLVING_CLOUD_MOVING || mode == Modes.RESOLVING_CLOUD_STATIC ||
                mode == Modes.RESOLVING_FILE_MOVING || mode == Modes.RESOLVING_FILE_STATIC)
            {
                playMenuThesisScript.changeText("Move to a calibration point. The device is lost.", Color.red);
            }
        }
        else // Help user texts
        {
            if (mode == Modes.CONFIGURE_FILE || mode == Modes.CONFIGURE_CLOUD)
            {
                if (confMenuScript.getTextColor() == Color.red)
                {
                    foreach (var go in listGameObjectsToHost)
                    {
                        if (go.GetComponent<LeanSelectable>() != null)
                        {
                            if (go.GetComponent<LeanSelectable>().IsSelected == true)
                            {
                                confMenuScript.changeText("Drag the screen to move, scale and rotate the object.", Color.black);
                                break;
                            }
                            else
                            {
                                confMenuScript.changeText("Place a new anchor/object or select the object to change.", Color.black);
                            }
                        }
                    }
                }
            }
        }
        lastCameraPose = currentPose;


        if (mode == Modes.RESOLVING_CLOUD_MOVING || mode == Modes.RESOLVING_CLOUD_STATIC) // Resolve anchors
        {
            foreach (var cloud in anchorsToResolve)
            {
                XPSession.ResolveCloudAnchor(cloud.Id).ThenAction(result =>
                {
                    if (result.Response != CloudServiceResponse.Success) { }
                    else
                    {
                        xpAnchorsToErase.Add(result.Anchor);
                        foreach (var obj in cloud.ListAnchorObjects) // Instantiate every virtual object to the corresponding anchor and apply the needed transformations
                        {
                            GameObject go = Instantiate(prefabs.getResolveMap()[obj.Prefab_name], result.Anchor.transform.position, result.Anchor.transform.rotation, result.Anchor.transform);
                            go.transform.Translate(obj.X, obj.Y, obj.Z);
                            go.transform.Rotate(0, obj.Rotation, 0);

                            go.transform.localScale = new Vector3(obj.Scale_X, obj.Scale_Y, obj.Scale_Z);
                            if (go.CompareTag("Hint")) // Arrow - Show only at static objects game
                            {
                                if (settingsMenuScript.server_enabled)
                                {
                                    if (client.clientSocket.Connected)
                                    {
                                        client.message.setGameState("HINT", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                            checkpointsReached, score, timer);
                                        client.SendData();
                                    }
                                }
                                resolvedGO.Add(go);
                                go.GetComponent<PathHandler>().id = obj.BezierNumber;
                                go.SetActive(mode == Modes.RESOLVING_CLOUD_STATIC || mode == Modes.PLAY_CLOUD_STATIC);
                            }
                            else if(go.CompareTag("Goal")) /// Goals
                            {
                                if (settingsMenuScript.server_enabled)
                                {
                                    if (client.clientSocket.Connected)
                                    {
                                        client.message.setGameState("GOAL", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                            checkpointsReached, score, timer);
                                        client.SendData();
                                    }
                                }
                                resolvedGO.Add(go);
                                go.GetComponent<PathHandler>().id = obj.BezierNumber;
                                go.SetActive(true);
                            }
                            else if(go.CompareTag("Stop") || go.CompareTag("LookAway")) // Hide stop and spotlight object
                            {
                                resolvedGO.Add(go);
                                go.SetActive(false);
                            }
                            else
                            {
                                if (go.CompareTag("Dodge")) // Dodge object instatiation
                                {
                                    if (settingsMenuScript.server_enabled)
                                    {
                                        if (client.clientSocket.Connected)
                                        {
                                            client.message.setGameState("DODGE_OBJ", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                                checkpointsReached, score, timer);
                                            client.SendData();
                                        }
                                    }
                                    go.GetComponent<BoxCollider>().size = new Vector3(go.GetComponent<BoxCollider>().size.x,
                                        go.GetComponent<BoxCollider>().size.y * 3, go.GetComponent<BoxCollider>().size.z);
                                }
                                resolvedGO.Add(go);
                                go.SetActive(true);
                            }
                        }
                        // Draw beazier curve
                        fillGoalsAndControls(resolvedGO);
                        DrawNCurve(goals.Count);
                    }
                });
            }

            if (mode == Modes.RESOLVING_CLOUD_MOVING) // Resolve done, change mode to play only.
            {
                mode = Modes.PLAY_CLOUD_MOVING;
            }
            else
            {
                mode = Modes.PLAY_CLOUD_STATIC;
            }
        }
        else if(mode == Modes.RESOLVING_FILE_MOVING || mode == Modes.RESOLVING_FILE_STATIC) // Resolve objects when looking to their saved position (coordinates)
        {
            MyThesisHistory toRemove = new MyThesisHistory();
            foreach (var cloud in anchorsToResolve) 
            {
                GameObject auxVerifier = new GameObject();
                auxVerifier.transform.position = new Vector3(cloud.anchorInfo.anchorX, cloud.anchorInfo.anchorY, cloud.anchorInfo.anchorZ);
                auxVerifier.transform.Rotate(cloud.anchorInfo.anchorRotX, cloud.anchorInfo.anchorRotY, cloud.anchorInfo.anchorRotZ);
                auxVerifier.transform.localScale = new Vector3(cloud.anchorInfo.anchorScaleX, cloud.anchorInfo.anchorScaleY, cloud.anchorInfo.anchorScaleZ);
                if (IsLookingAtObject(auxVerifier.transform)) //
                {
                    anchor = Session.CreateAnchor(new Pose(transform.position, auxVerifier.transform.rotation));
                    anchorsToErase.Add(anchor);
                    anchorsResolvedFile.Add(anchor);
                    foreach (var obj in cloud.ListAnchorObjects) // Instantiate every virtual object to the corresponding anchor and apply the needed transformations
                    {
                        GameObject go = Instantiate(prefabs.getResolveMap()[obj.Prefab_name], auxVerifier.transform.position, anchor.transform.rotation, anchor.transform);
                        go.transform.Translate(obj.X, obj.Y, obj.Z);
                        go.transform.Rotate(0, obj.Rotation, 0);

                        go.transform.localScale = new Vector3(obj.Scale_X, obj.Scale_Y, obj.Scale_Z);
                        if (go.CompareTag("Hint")) // Arrow - Show only at static objects game
                        {
                            if (settingsMenuScript.server_enabled)
                            {
                                if (client.clientSocket.Connected)
                                {
                                    client.message.setGameState("HINT", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                        checkpointsReached, score, timer);
                                    client.SendData();
                                }
                            }
                            resolvedGO.Add(go);
                            go.GetComponent<PathHandler>().id = obj.BezierNumber;
                            go.SetActive(mode == Modes.RESOLVING_FILE_STATIC || mode == Modes.PLAY_FILE_STATIC);
                        }
                        else if(go.CompareTag("Goal")) // Goals
                        {
                            if (settingsMenuScript.server_enabled)
                            {
                                if (client.clientSocket.Connected)
                                {
                                    client.message.setGameState("GOAL", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                        checkpointsReached, score, timer);
                                    client.SendData();
                                }
                            }
                            resolvedGO.Add(go);
                            go.GetComponent<PathHandler>().id = obj.BezierNumber;
                            go.SetActive(true);
                        }
                        else if (go.CompareTag("Stop") || go.CompareTag("LookAway")) // Hide stop and spotlight object
                        {
                            resolvedGO.Add(go);
                            go.SetActive(false);
                        }
                        else
                        {
                            if (go.CompareTag("Dodge")) // Dodge object instatiation
                            {
                                if (settingsMenuScript.server_enabled)
                                {
                                    if (client.clientSocket.Connected)
                                    {
                                        client.message.setGameState("DODGE_OBJ", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                            checkpointsReached, score, timer);
                                        client.SendData();
                                    }
                                }
                                go.GetComponent<BoxCollider>().size = new Vector3(go.GetComponent<BoxCollider>().size.x,
                                    go.GetComponent<BoxCollider>().size.y * 3, go.GetComponent<BoxCollider>().size.z);
                            }
                            resolvedGO.Add(go);
                            go.SetActive(true);
                        }
                        adjustPosition[go] = new Vector3(auxVerifier.transform.position.x + obj.X,
                            auxVerifier.transform.position.y + obj.Y,
                            auxVerifier.transform.position.z + obj.Z);
                    }
                    toRemove = cloud;
                    // Draw beazier curve
                    fillGoalsAndControls(resolvedGO);
                    DrawNCurve(goals.Count);
                    break;
                }
            }
            if(anchorsToResolve.Contains(toRemove)){
                anchorsToResolve.Remove(toRemove);
            }
            if(anchorsToResolve.Count == 0) // Resolve done, change mode to play only.
            {
                if (mode == Modes.RESOLVING_FILE_MOVING)
                {
                    mode = Modes.PLAY_FILE_MOVING;
                }
                else
                {
                    mode = Modes.PLAY_FILE_STATIC;
                }
            }
        }

        if (mode == Modes.RESOLVING_CLOUD_MOVING  || mode == Modes.RESOLVING_CLOUD_STATIC || mode == Modes.RESOLVING_FILE_MOVING ||
            mode == Modes.RESOLVING_FILE_STATIC || mode == Modes.PLAY_CLOUD_STATIC || mode == Modes.PLAY_CLOUD_MOVING || 
            mode == Modes.PLAY_FILE_MOVING || mode == Modes.PLAY_FILE_STATIC)
        {
            // Change help texts.
            if (mode == Modes.RESOLVING_CLOUD_STATIC || mode == Modes.RESOLVING_FILE_STATIC || 
                mode == Modes.PLAY_CLOUD_STATIC || mode == Modes.PLAY_FILE_STATIC)
            {
                playMenuThesisScript.changeText("Follow the arrows path!\nUse the device to catch the green cubes and win the race!", Color.black);
            }
            else if(mode == Modes.RESOLVING_CLOUD_MOVING || mode == Modes.RESOLVING_FILE_MOVING ||
                 mode == Modes.PLAY_CLOUD_MOVING || mode == Modes.PLAY_FILE_MOVING)
            {
                 if (movingObject == null)
                 {
                    if (enable_without_anyfind_var)
                    {
                        playMenuThesisScript.changeText("Point to the horizontal surface and place the moving object.", Color.black);
                    }
                    else
                    {
                        playMenuThesisScript.changeText("Look around to find an anchor or enable the moving object to find the path.", Color.black);
                    }
                 }
                 else
                 {
                    playMenuThesisScript.changeText("Follow the moving object\nUse the device to catch the green cubes and win the race!", Color.black);
                 }
            }
            if (settingsMenuScript.server_enabled) // Send camera position to server every frame
            {
                if (client.clientSocket.Connected)
                {
                    client.message.objCoords("CAMERA", this.transform.position, this.transform.rotation.eulerAngles, DateTime.Now);
                    client.SendData();
                }
            }

            // Activate hidden virtual game objects
            foreach (var go in resolvedGO)
            {
                if (go.CompareTag("LookAway"))
                {
                    if (IsLookingAtObjectWith2Ddistance(go.transform, 4f))
                    {
                        if (!go.activeSelf)
                        {
                            if (settingsMenuScript.server_enabled)
                            {
                                if (client.clientSocket.Connected)
                                {
                                    client.message.setGameState("SPOTLIGH_OBJ", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                        checkpointsReached, score, timer);
                                    client.SendData();
                                }
                            }
                            go.SetActive(true);
                            Invoke("Lookaway", 3f);
                        }
                    }
                }
                else if (go.CompareTag("Stop"))
                {
                    if (IsLookingAtObjectWith2Ddistance(go.transform, 2))
                    {
                        if (!go.activeSelf)
                        {
                            if (settingsMenuScript.server_enabled)
                            {
                                if (client.clientSocket.Connected)
                                {
                                    client.message.setGameState("STOP_OBJ", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                        checkpointsReached, score, timer);
                                    client.SendData();
                                }
                            }
                            go.transform.Rotate(90, 0, 0);
                            go.SetActive(true);
                            Invoke("rotateStop", 0.012f);
                        }
                    }
                }
            }

            // Handle time
            timer += Time.deltaTime;
            float minutes = (int)(timer / 60);
            float seconds = (int)(timer % 60);
            playMenuThesisScript.changeTimeText(minutes, seconds);

            //Handle score (time related)
            score -= ((int)(minutes * 60 + seconds) - last_time); 
            last_time = (int)(minutes * 60 + seconds);
            playMenuThesisScript.changeScoreText(score);

            // Check Points text
            playMenuThesisScript.changeCheckPointText(checkpointsReached, checkpointsToGoal);

            // Destroy Lean library before playing
            foreach(var go in resolvedGO)
            {
                if(go.GetComponent<LeanSelectable>() != null)
                {
                    Destroy(go.GetComponent<LeanSelectable>());
                }
                if(go.GetComponent<LeanTranslate>() != null)
                {
                    Destroy(go.GetComponent<LeanTranslate>());
                }
                if(go.GetComponent<LeanScale>() != null)
                {
                    Destroy(go.GetComponent<LeanTranslate>());
                }
                if(go.GetComponent<LeanRotateCustomAxis>() != null)
                {
                    Destroy(go.GetComponent<LeanTranslate>());
                }
            }

            // End of the game - change menu and mode
            if(checkpointsReached == checkpointsToGoal)
            {
                endMenuScript.changeTimeText(minutes, seconds);
                endMenuScript.changeScoreText(score);
                if(mode == Modes.RESOLVING_CLOUD_STATIC || mode == Modes.PLAY_CLOUD_STATIC)
                {
                    endMenuScript.change_last_mode(scenario,"play_cloud_static", username);
                }
                else if(mode == Modes.RESOLVING_CLOUD_MOVING || mode == Modes.PLAY_CLOUD_MOVING)
                {
                    endMenuScript.change_last_mode(scenario, "play_cloud_moving", username);
                }
                else if(mode == Modes.RESOLVING_FILE_STATIC || mode == Modes.PLAY_FILE_STATIC)
                {
                    endMenuScript.change_last_mode(scenario, "play_file_static", username);
                }
                else if(mode == Modes.RESOLVING_FILE_MOVING || mode == Modes.PLAY_FILE_MOVING)
                {
                    endMenuScript.change_last_mode(scenario, "play_file_moving", username);
                }

                if(movingObject != null)
                {
                    Destroy(movingObject);
                }
                 
                if (settingsMenuScript.server_enabled) // Inform the server that the game ended
                {
                    if (client.clientSocket.Connected)
                    {
                        client.message.setGameState("END", DateTime.Now, this.transform.position, this.transform.rotation.eulerAngles, checkpointsReached, score, timer);
                        client.SendData();
                    }
                }
                changeMenu("end_menu");
            }

            // Save the current frame device position
            if (lineRenderer.positionCount > 0) {
                eachFramePosition.Add(new Vector3(currentPose.position.x, lineRenderer.GetPosition(0).y, currentPose.position.z));
            }

            if (lineRenderer.positionCount > 0) // Verify if the device left the road (or returned to it). Exception: Dodging a dodge object or the spotlight.
            {
                double closest_distance_to_line = 999999;
                for (int i = 0; i < (checkpointsReached + 1) * numPoints; i++)
                {
                    double aux_distance = calculate2dDistance(lineRenderer.GetPosition(i), new Vector3(currentPose.position.x, lineRenderer.GetPosition(0).y, currentPose.position.z));
                    if(aux_distance < closest_distance_to_line)
                    {
                        closest_distance_to_line = aux_distance;
                    }
                }
                double closest_distance_to_dodge = 99999;
                foreach(var go in resolvedGO) 
                {
                    if (go.CompareTag("Dodge"))
                    {
                        double aux_distance = calculate3dDistance(go.transform.position, currentPose.position);
                        if (aux_distance < closest_distance_to_dodge)
                        {
                            closest_distance_to_dodge = aux_distance;
                        }
                    }
                }
                if (!IsInvoking("Lookaway") && !(closest_distance_to_dodge < 2.2))
                {
                    if (closest_distance_to_line >= 0.75)
                    {
                        if (!outOfRoad)
                        {
                            score -= 50;
                            playMenuThesisScript.changeFeedbackText(false, "50");
                            outOfRoad = true;
                            if (settingsMenuScript.server_enabled)
                            {
                                if (client.clientSocket.Connected)
                                {
                                    client.message.setGameState("OUT_OF_ROAD", DateTime.Now, this.transform.position, this.transform.rotation.eulerAngles,
                                        checkpointsReached, score, timer);
                                    client.SendData();
                                }
                            }
                        }
                    }
                    else
                    {
                        outOfRoad = false;
                    }
                }
                else
                {
                    outOfRoad = true;
                }
            }
        }

        if (mode == Modes.PLAY_FILE_MOVING || mode == Modes.RESOLVING_FILE_MOVING || mode == Modes.PLAY_CLOUD_MOVING)
        {
            /* Moving object logic */
            if (!endMovingObjPath)
            {
                if (movingObject == null)
                {
                    if (resolvedGO.Count > 0) // Create the moving object when virtual objects appear
                    {
                        Vector3 aux_vector = lineRenderer.GetPosition(0) - lineRenderer.GetPosition(1);
                        float distanceBetween2P = Mathf.Sqrt(Mathf.Pow(aux_vector.x, 2) + Mathf.Pow(aux_vector.z, 2));
                        float angleBetweenLinePoints = Vector3.Angle(Vector3.zero, aux_vector);
                        movingObject = Instantiate(movingObjectPrefab, lineRenderer.GetPosition(0), Quaternion.Euler(0, angleBetweenLinePoints, 0));
                        linePosition = 1;
                        lineLength = calculateLineLength(Mathf.FloorToInt((linePosition / numPoints))) * 2;
                    }
                }
                else
                {
                    if (settingsMenuScript.server_enabled) // Send the moving object position to the server application.
                    {
                        if (client.clientSocket.Connected)
                        {
                            client.message.objCoords("MOVING_OBJECT", movingObject.transform.position, movingObject.transform.rotation.eulerAngles, DateTime.Now);
                            client.SendData();
                        }
                    }

                    if (linePosition < lineRenderer.positionCount)
                    {
                        if (!IsInvoking("Stop") && !IsInvoking("rotateStop")) // Stop the moving object when stop appears
                        {
                            if (continued)
                            {
                                double distance = 9999;
                                for (int i = 1; i < lineRenderer.positionCount; i++)
                                {
                                    float aux_distance = (float)calculate3dDistance(lineRenderer.GetPosition(i), movingObject.transform.position);
                                    if (aux_distance < distance)
                                    {
                                        distance = aux_distance;
                                        hintToMove = lineRenderer.GetPosition(i);
                                        linePosition = i;
                                        lineLength = calculateLineLength(Mathf.FloorToInt((linePosition / numPoints))) * 2;
                                    }
                                }
                                continued = false;
                            }
                            if (linePosition % numPoints == 0) // Checkpoint reached by the moving object
                            {
                                playMenuThesisScript.changeText("Follow the moving object\nUse the device to catch the green cubes and win the race!", Color.black);
                                lineLength = calculateLineLength(Mathf.FloorToInt((linePosition / numPoints))) * 2;
                            }

                            if (auxLinePosition == 0)
                            {
                                // Create a "auxiliarly path" between two points from the road line renderer 
                                Vector3 aux_line = lineRenderer.GetPosition(linePosition) - lineRenderer.GetPosition(linePosition - 1);
                                line4.Clear();
                                for (int i = 0; i < Mathf.RoundToInt(lineLength) - 1; i++)
                                {
                                    line4.Add(lineRenderer.GetPosition(linePosition - 1) + (i + 1) * aux_line / Mathf.RoundToInt(lineLength));
                                }
                                line4.Add(lineRenderer.GetPosition(linePosition));
                                movingObject.transform.rotation = Quaternion.Euler(0, ConvertToDegrees(Mathf.Atan2(lineRenderer.GetPosition((int)linePosition).x - lineRenderer.GetPosition((int)linePosition - 1).x,
                                lineRenderer.GetPosition((int)linePosition).z - lineRenderer.GetPosition((int)linePosition - 1).z)), 0);

                                // Moving object score handler
                                if (calculate2dDistance(movingObject.transform.position,
                                    new Vector3(this.transform.position.x, movingObject.transform.position.y, this.transform.position.z)) - 0.4 < 1)
                                {
                                    sum += 1 - calculate2dDistance(movingObject.transform.position,
                                            new Vector3(this.transform.position.x, movingObject.transform.position.y, this.transform.position.z)) - 0.4;
                                }
                                num_sum += 1;
                            }

                            // Change the moving object position based on its velocity and on the road length
                            movingObject.transform.position = line4[auxLinePosition];
                            auxLinePosition += (int)movObjVelocity;
                            if (auxLinePosition > Mathf.RoundToInt(lineLength) - 1)
                            {
                                auxLinePosition = 0;
                                linePosition += 1;
                            }

                            // Rotate moving object wheels
                            rotateWheels(movingObject.transform);
                        }
                    }
                    else
                    {
                        if (goals.Count == checkpointsToGoal)
                        {
                            Destroy(movingObject);
                            lastDistances.Clear();
                            hintToMove = new Vector3();
                            endMovingObjPath = true;
                        }
                        else // The moving object will continue its previous path until the remaining road is found
                        {
                            playMenuThesisScript.changeText("Finding the remaining path.", Color.black);
                            movingObject.transform.position += (lineRenderer.GetPosition(lineRenderer.positionCount - 1) - lineRenderer.GetPosition(lineRenderer.positionCount - 2)) / Mathf.RoundToInt(lineLength);
                            continued = true;
                        }
                    }
                }
            }
        }

        if (mode == Modes.CONFIGURE_CLOUD || mode == Modes.CONFIGURE_FILE)
        {
            // Draw beazier curve
            if (configChanged())
            {
                fillGoalsAndControls(listGameObjectsInConfiguration);
                DrawNCurve(goals.Count);
            }

            // Selection handling
            objectSelected = false;
            if (anchor != null && listGameObjectsToHost.Count != 0)
            {
                foreach (var go in listGameObjectsToHost)
                {
                    if (go.GetComponent<LeanSelectable>() != null)
                    {
                        if (go.GetComponent<LeanSelectable>().IsSelected == true)
                        {
                            objectSelected = true;
                            if (previousSelectedObject != go)
                            {
                                if(selectedArrow != null)
                                {
                                    Destroy(selectedArrow);
                                    confMenuScript.destroyAxis(previousSelectedObject);
                                }
                                arrowSelection(go);
                                confMenuScript.drawAxis(go);
                                previousSelectedObject = go;
                                confMenuScript.changeText("Drag the screen to move, scale and rotate the object.", Color.black);
                            }
                            angleTillAnchor = ConvertToRadians((go.transform.rotation.eulerAngles.y));
                            rotationMatrix.Clear();
                            rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                            rotationMatrix.Add(new Vector3(0, 1, 0));
                            rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                            Vector3 auxPos = go.transform.position - anchor.transform.position;
                            Vector3 textPositions = new Vector3(
                                auxPos.x * rotationMatrix[0].x + auxPos.y * rotationMatrix[0].y + auxPos.z * rotationMatrix[0].z,
                                auxPos.x * rotationMatrix[1].x + auxPos.y * rotationMatrix[1].y + auxPos.z * rotationMatrix[1].z,
                                auxPos.x * rotationMatrix[2].x + auxPos.y * rotationMatrix[2].y + auxPos.z * rotationMatrix[2].z);
                            if (go.CompareTag("Hint") || go.CompareTag("Goal"))
                            {
                                confMenuScript.changeManualTexts(textPositions.x, textPositions.y, textPositions.z, go.transform.rotation.eulerAngles.y,
                                    go.transform.localScale.x, go.GetComponent<PathHandler>().id);
                                confMenuScript.changeManualInteractable(true);
                            }
                            else
                            {
                                confMenuScript.changeManualTexts(textPositions.x, textPositions.y, textPositions.z, go.transform.rotation.eulerAngles.y,
                                    go.transform.localScale.x, 0);
                                confMenuScript.changeManualInteractableAux(true);
                            }
                        }
                    }
                }
            }

            // Destroy selection arrow if there are no selected objects
            if(!objectSelected)
            {
                if (confMenuScript.getManualInteractable())
                {
                    confMenuScript.changeManualInteractable(false);
                    confMenuScript.changeManualTexts(0.0f, 0.0f, 0.0f, 0, 0.0f, 0);
                }
                if (selectedArrow != null)
                {
                    if (previousSelectedObject != null)
                    {
                        confMenuScript.destroyAxis(previousSelectedObject);
                        previousSelectedObject = null;
                    }
                    Destroy(selectedArrow);
                    confMenuScript.changeText("Place a new anchor/object or select the object to change.", Color.black);
                }
            }

            // Add a new anchor
            if (enableNewAnchor)
            {
                confMenuScript.changeText("Touch the horizontal surface to add the new anchor.", Color.black);
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                {
                    //VERIFY IF CLICK ON BUTTON
                    if (!verifyClickOnButton(Input.touches[0].position))
                    {
                        (bool hitfound, TrackableHit hit) hit_bool = verifyClickOnPlane();
                        if (hit_bool.hitfound)
                        {
                            foreach (var obj in listGameObjectsToHost)
                            {
                                if (obj.GetComponent<LeanSelectable>().IsSelected)
                                {
                                    confMenuScript.destroyAxis(obj);
                                    obj.GetComponent<LeanSelectable>().Deselect();
                                }
                            }
                            foreach(var go in listGameObjectsToHost)
                            {
                                gameObjectsToErase.Add(go);
                            }
                            if (!hosted)
                            {
                                foreach(var go in listGameObjectsToHost)
                                {
                                    listGameObjectsInConfiguration.Remove(go);
                                    Destroy(go);
                                }
                            }
                            listGameObjectsToHost.Clear();
                            firstObjectRotation.Clear();
                            respectivePrefabIdx.Clear();
                            if (anchor != null)
                            {
                                anchorsToErase.Add(anchor);
                            }
                            if (gameObjectToHost != null)
                            {
                                if (gameObjectToHost.name.Contains("anchorObject"))
                                {
                                    anchorsToErase.Remove(anchor);
                                    Destroy(anchor);
                                    Destroy(gameObjectToHost);
                                }
                                else
                                {
                                    gameObjectsToErase.Add(gameObjectToHost);
                                }
                            }
                            anchor = Session.CreateAnchor(new Pose(hit_bool.hit.Pose.position, hit_bool.hit.Pose.rotation));

                            if (usabilityTestScript.getUsabilityEnabled())
                            {
                                usabilityTestScript.setTime();
                            }

                            if (mode == Modes.CONFIGURE_CLOUD)
                            {
                                gameObjectToHost = Instantiate(anchoredPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform);
                                confMenuScript.enableHosting(false);
                            }
                            else
                            {
                                if (!usabilityTestScript.getUsabilityEnabled())
                                {
                                    gameObjectToHost = Instantiate(prefabs.getResolveMap()[prefabs.getPrefabNameArray()[prefabsIdx]], anchor.transform.position, anchor.transform.rotation, anchor.transform);
                                    firstObjectRotation.Add(gameObjectToHost, gameObjectToHost.transform.rotation.eulerAngles.y);
                                    listGameObjectsToHost.Add(gameObjectToHost);
                                    if (gameObjectToHost.CompareTag("Hint") || gameObjectToHost.CompareTag("Goal"))
                                    {
                                        listGameObjectsInConfiguration.Add(gameObjectToHost);
                                        gameObjectToHost.GetComponent<PathHandler>().id = listGameObjectsInConfiguration.Count;
                                    }
                                    respectivePrefabIdx.Add(gameObjectToHost, prefabsIdx);
                                    gameObjectToHost.GetComponent<LeanSelectable>().Select();
                                    confMenuScript.enableHosting(true);
                                    confMenuScript.changeAnchorButtonInteract(true);
                                }
                            }
                            hostQuality.Clear();
                            confMenuScript.changeAnchorButtonInteract(true);
                            enableNewAnchor = false;
                            hosted = false;
                        }
                    }
                }
            }

            //Add new Object
            if (enableNewObject)
            {
                confMenuScript.changeText("Touch the horizontal surface to add a new virtual object.", Color.black);
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                {
                    //VERIFY IF CLICK ON BUTTON
                    if (!verifyClickOnButton(Input.touches[0].position))
                    {
                        (bool hitfound, TrackableHit hit) hit_bool = verifyClickOnPlane();
                        if (hit_bool.hitfound)
                        {
                            if (selectedArrow != null)
                            {
                                Destroy(selectedArrow);
                                foreach (var selected in listGameObjectsToHost)
                                {
                                    if (selected.GetComponent<LeanSelectable>().IsSelected)
                                    {
                                        selected.GetComponent<LeanSelectable>().Deselect();
                                        confMenuScript.destroyAxis(selected);
                                        break;
                                    }
                                }
                            }
                            if (usabilityTestScript.getUsabilityEnabled())
                            {
                                if (listGameObjectsToHost.Count > 0)
                                {
                                    usabilityTestScript.stopTime(listGameObjectsToHost[listGameObjectsToHost.Count - 1].transform);
                                }
                                else
                                {
                                    usabilityTestScript.setTime();
                                }
                                usabilityTestScript.placeNextObject(listGameObjectsToHost.Count);
                            }
                            GameObject go = Instantiate(prefabs.getResolveMap()[prefabs.getPrefabNameArray()[prefabsIdx]], hit_bool.hit.Pose.position, anchor.transform.rotation, anchor.transform);
                            listGameObjectsToHost.Add(go);
                            if (go.CompareTag("Hint") || go.CompareTag("Goal"))
                            {
                                listGameObjectsInConfiguration.Add(go);
                                go.GetComponent<PathHandler>().id = listGameObjectsInConfiguration.Count;
                            }
                            firstObjectRotation.Add(go, go.transform.rotation.eulerAngles.y);
                            respectivePrefabIdx.Add(go, prefabsIdx);
                            
                            confMenuScript.enableHosting(true);
                            confMenuScript.changeAnchorButtonInteract(true);
                            confMenuScript.changeText("Drag the screen to move, scale and rotate the object.", Color.black);
                            enableNewObject = false;

                            go.GetComponent<LeanSelectable>().Select();
                        }   
                    }
                }
            }
        }

        if(mode == Modes.CONFIGURE_CLOUD) {
            //VERIFY ANCHOR QUALITY
            if (anchor != null && gameObjectToHost != null)
            {
                if (gameObjectToHost.name.Contains("anchorObject"))
                {
                    foreach (Transform child in gameObjectToHost.transform)
                    {
                        if (child.gameObject.name != "Anchor" && !hostQuality.Contains(child.gameObject.name))
                        {
                            if (IsLookingAtSide(child))
                            {
                                if (XPSession.EstimateFeatureMapQualityForHosting(Frame.Pose).ToString() == "Good")
                                {
                                    child.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                                    hostQuality.Add(child.gameObject.name);
                                }
                                else if (XPSession.EstimateFeatureMapQualityForHosting(Frame.Pose).ToString() == "Sufficient")
                                {
                                    child.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                                }
                            }
                            //IF GOOD ANCHOR
                            if (hostQuality.Count == 5)
                            {
                                if (usabilityTestScript.getUsabilityEnabled())
                                {
                                    usabilityTestScript.stopTimeForQuality();
                                    Destroy(gameObjectToHost);
                                    confMenuScript.enableHosting(true);
                                    confMenuScript.changeAttachInteractable(false);
                                }
                                else
                                {
                                    Invoke("invokedInstantiation", 1.5f);
                                }

                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calculate a line portion length.
    /// </summary>
    /// <param name="startingIndex">The line portion starting index.</param>
    /// <returns>The line length (meters).</returns>
    public float calculateLineLength(int startingIndex)
    {
        float distance = 0;
        for (int i = startingIndex * numPoints; i < (startingIndex * numPoints) + numPoints - 1; i++)
        {
            distance += (float)calculate3dDistance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
        }
        return distance;
    }

    /// <summary>
    /// Destroy anchor prefab after one second of its maximum quality is achieved.  Instatiates a new object from the prefabs list too.
    /// </summary>
    void invokedInstantiation()
    {
        Destroy(gameObjectToHost);
        confMenuScript.enableHosting(true);
        confMenuScript.changeText("Drag the screen to move, scale and rotate the object.", Color.black);
        GameObject go = Instantiate(prefabs.getResolveMap()[prefabs.getPrefabNameArray()[prefabsIdx]], anchor.transform.position, anchor.transform.rotation, anchor.transform);
        listGameObjectsToHost.Add(go);
        if (go.CompareTag("Hint") || go.CompareTag("Goal"))
        {
            listGameObjectsInConfiguration.Add(go);
            go.GetComponent<PathHandler>().id = listGameObjectsInConfiguration.Count;
        }
        firstObjectRotation.Add(go, go.transform.rotation.eulerAngles.y);
        respectivePrefabIdx.Add(go, prefabsIdx);
        go.GetComponent<LeanSelectable>().Select();
    }

    /// <summary>
    /// Creates the selection arrow on top of the selected object.
    /// </summary>
    /// <param name="go">The selected object.</param>
    public void arrowSelection(GameObject go)
    {
        Vector3 arrowPos = go.transform.GetComponent<Collider>().bounds.center + new Vector3(0, go.GetComponent<Collider>().bounds.size.y / 2 + 0.1f, 0); ;
        selectedArrow = Instantiate(selectedArrowPrefab, arrowPos, new Quaternion(0, 0, 0, 0));
        selectedArrow.transform.Rotate(270, 0, 0);
        selectedArrow.transform.SetParent(go.transform, true);
    }

    /// <summary>
    /// Host an anchor and its attached objects information in the GCP or/and in the player storage.
    /// </summary>
    public void HostCloudAnchor()
    {
        if (mode == Modes.CONFIGURE_CLOUD)
        {
            confMenuScript.changeText("Wait while we are transfering the anchor the cloud.", Color.black);
            XPSession.CreateCloudAnchor(anchor, 360).ThenAction(result =>
            {
                if (result.Response != CloudServiceResponse.Success)
                {
                    confMenuScript.changeText(result.Response.ToString(), Color.red);
                    confMenuScript.enableHosting(true);
                }
                else
                {
                    List<AnchorObject> auxAnchorObjectList = new List<AnchorObject>();
                    angleTillAnchor = ConvertToRadians((result.Anchor.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    //int num_of_mov_objs_in_anchor = 1;
                    foreach (var go in listGameObjectsToHost)
                    {
                        Vector3 auxPos = go.transform.position - result.Anchor.transform.position;
                        Vector3 pos = new Vector3(
                            auxPos.x * rotationMatrix[0].x + auxPos.y * rotationMatrix[0].y + auxPos.z * rotationMatrix[0].z,
                            auxPos.x * rotationMatrix[1].x + auxPos.y * rotationMatrix[1].y + auxPos.z * rotationMatrix[1].z,
                            auxPos.x * rotationMatrix[2].x + auxPos.y * rotationMatrix[2].y + auxPos.z * rotationMatrix[2].z);
                        float rot = go.transform.eulerAngles.y - firstObjectRotation[go];

                        /* Logic for the others objects */
                        if(go.CompareTag("Hint") || go.CompareTag("Goal"))
                        {
                            AnchorObject auxAnchorObject = new AnchorObject(prefabs.getPrefabNameArray()[respectivePrefabIdx[go]], pos.x, pos.y, pos.z, rot,
                                go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z, 0, go.GetComponent<PathHandler>().id);
                            auxAnchorObjectList.Add(auxAnchorObject);
                        }
                        else
                        {
                            AnchorObject auxAnchorObject = new AnchorObject(prefabs.getPrefabNameArray()[respectivePrefabIdx[go]], pos.x, pos.y, pos.z, rot,
                                go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z, 0, 0);
                            auxAnchorObjectList.Add(auxAnchorObject);
                        }
                    }
                    int count = LoadCloudAnchorHistory().Collection.Count;
                    AnchorFileInfo auxAnchorFileInfo = new AnchorFileInfo(result.Anchor.transform.position.x, result.Anchor.transform.position.y, result.Anchor.transform.position.z,
                           result.Anchor.transform.rotation.eulerAngles.x, result.Anchor.transform.rotation.eulerAngles.y, result.Anchor.transform.rotation.eulerAngles.z,
                           result.Anchor.transform.localScale.x, result.Anchor.transform.localScale.y, result.Anchor.transform.localScale.z);
                    _hostedCloudAnchor = new MyThesisHistory("CloudAnchor" + count.ToString(), result.Anchor.CloudId, scenario, auxAnchorObjectList, auxAnchorFileInfo);
                    SaveCloudAnchorHistory(_hostedCloudAnchor);
                    File.AppendAllText(fileCoordsAnchorPath + "/" + scenario + ".json", JsonUtility.ToJson(_hostedCloudAnchor));
                    confMenuScript.changeText("Host Succeded into " + scenario, Color.black);
                    confMenuScript.enableHosting(false);
                    hosted = true;
                }
            });
        }
        else
        {
            List<AnchorObject> auxAnchorObjectList = new List<AnchorObject>();
            angleTillAnchor = ConvertToRadians((anchor.transform.rotation.eulerAngles.y));
            rotationMatrix.Clear();
            rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
            rotationMatrix.Add(new Vector3(0, 1, 0));
            rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
            //int num_of_mov_objs_in_anchor = 1;
            foreach (var go in listGameObjectsToHost)
            {
                Vector3 auxPos = go.transform.position - anchor.transform.position;
                Vector3 pos = new Vector3(
                    auxPos.x * rotationMatrix[0].x + auxPos.y * rotationMatrix[0].y + auxPos.z * rotationMatrix[0].z,
                    auxPos.x * rotationMatrix[1].x + auxPos.y * rotationMatrix[1].y + auxPos.z * rotationMatrix[1].z,
                    auxPos.x * rotationMatrix[2].x + auxPos.y * rotationMatrix[2].y + auxPos.z * rotationMatrix[2].z);
                float rot = go.transform.eulerAngles.y - firstObjectRotation[go];

                /* Logic for the others objects */
                if (go.CompareTag("Hint") || go.CompareTag("Goal"))
                {
                    AnchorObject auxAnchorObject = new AnchorObject(prefabs.getPrefabNameArray()[respectivePrefabIdx[go]], pos.x, pos.y, pos.z, rot,
                        go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z, 0, go.GetComponent<PathHandler>().id);
                    auxAnchorObjectList.Add(auxAnchorObject);
                }
                else
                {
                    AnchorObject auxAnchorObject = new AnchorObject(prefabs.getPrefabNameArray()[respectivePrefabIdx[go]], pos.x, pos.y, pos.z, rot,
                        go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z, 0, 0);
                    auxAnchorObjectList.Add(auxAnchorObject);
                }
            }
            int count = LoadCloudAnchorHistory().Collection.Count;
            AnchorFileInfo auxAnchorFileInfo = new AnchorFileInfo(anchor.transform.position.x, anchor.transform.position.y, anchor.transform.position.z,
                anchor.transform.rotation.eulerAngles.x, anchor.transform.rotation.eulerAngles.y, anchor.transform.rotation.eulerAngles.z,
                anchor.transform.localScale.x, anchor.transform.localScale.y, anchor.transform.localScale.z);
            _hostedCloudAnchor = new MyThesisHistory("CloudAnchor" + count.ToString(), "0", scenario, auxAnchorObjectList, auxAnchorFileInfo);
            SaveCloudAnchorHistory(_hostedCloudAnchor);
            File.AppendAllText(fileCoordsAnchorPath + "/" + scenario + ".json", JsonUtility.ToJson(_hostedCloudAnchor));
            confMenuScript.changeText("Host Succeded into " + scenario, Color.black);
            confMenuScript.enableHosting(false);
            hosted = true;
        }
    }

    /// <summary>
    /// Calculate the straight forward 3d distance between two vectors.
    /// </summary>
    /// <param name="g">First vector.</param>
    /// <param name="h">Second vector.</param>
    /// <returns>The straight forward 3d distance between the vectors.</returns>
    public double calculate3dDistance(Vector3 g, Vector3 h)
    {
        return Mathf.Sqrt(Mathf.Pow(g.x - h.x, 2) + Mathf.Pow(g.z - h.z, 2) + Mathf.Pow(g.y - h.y, 2));
    }

    /// <summary>
    /// Calculate the straight forward 2d distance (x and z) between two vectors.
    /// </summary>
    /// <param name="g">First vector.</param>
    /// <param name="h">Second vector.</param>
    /// <returns>The straight forward 2d distance between the vectors.</returns>
    public double calculate2dDistance(Vector3 g, Vector3 h)
    {
        return Mathf.Sqrt(Mathf.Pow(g.x - h.x, 2) + Mathf.Pow(g.z - h.z, 2));
    }

    /// <summary>
    /// Change the object prefab.
    /// </summary>
    /// <param name="increment">Next or previous prefab (in the prefab list).</param>
    public void ChangePrefab(int increment)
    {
        bool selected = false;
        foreach (var go in listGameObjectsToHost)
        {
            if (go.GetComponent<LeanSelectable>().IsSelected)
            {
                selected = true;
                Vector3 lastPos = go.transform.position;
                listGameObjectsToHost.Remove(go);
                if (go.CompareTag("Hint") || go.CompareTag("Goal"))
                {
                    listGameObjectsInConfiguration.Remove(go);
                }
                firstObjectRotation.Remove(go);
                respectivePrefabIdx.Remove(go);
                go.GetComponent<LeanSelectable>().Deselect();
                Destroy(go);

                prefabsIdx = prefabsIdx + increment;
                if (prefabsIdx >= prefabs.getPrefabNameArray().Length)
                {
                    prefabsIdx = 0;
                }
                if (prefabsIdx < 0)
                {
                    prefabsIdx = prefabs.getPrefabNameArray().Length - 1;
                }

                GameObject newGo = Instantiate(prefabs.getResolveMap()[prefabs.getPrefabNameArray()[prefabsIdx]],
                                           lastPos,
                                           anchor.transform.rotation,
                                           anchor.transform);

                firstObjectRotation.Add(newGo, newGo.transform.rotation.eulerAngles.y);
                listGameObjectsToHost.Add(newGo);
                if (newGo.CompareTag("Hint") || newGo.CompareTag("Goal"))
                {
                    listGameObjectsInConfiguration.Add(newGo);
                    newGo.GetComponent<PathHandler>().id = listGameObjectsInConfiguration.Count;
                }
                respectivePrefabIdx.Add(newGo, prefabsIdx);

                arrowSelection(newGo);
                newGo.GetComponent<LeanSelectable>().Select();

                break;
            }
        }
        if (!selected)
        {
            confMenuScript.changeText("Select the virtual object.", Color.black);
        }
    }

    /// <summary>
    /// Clear the anchors on the selected scenario storage.
    /// </summary>
    /// <param name="scenarioToHost">Selected scenario.</param>
    public void ClearAnchors(string scenarioToHost)
    {
        var history = LoadCloudAnchorHistory();
        List<MyThesisHistory> toRemove = new List<MyThesisHistory>();
        foreach(var cloud in history.Collection)
        {
            if(cloud.Scenario == scenarioToHost)
            {
                toRemove.Add(cloud);
            }
        }
        foreach (var cloud in toRemove) {
            history.Collection.Remove(cloud);
        }
        PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey, JsonUtility.ToJson(history));
        anchorsToResolve.Clear();
        File.WriteAllText(fileCoordsAnchorPath+"/"+scenarioToHost+".json", string.Empty);
    }

    /// <summary>
    /// Verify if the device camera is looking at an object.
    /// </summary>
    /// <param name="go">Game object pose.</param>
    /// <returns>If the device camera is looking at the object.</returns>
    private bool IsLookingAtObject(Transform go)
    {
        var screenPoint = this.GetComponent<Camera>().WorldToViewportPoint(go.position);
        if (screenPoint.z <= 0 || screenPoint.x <= 0 || screenPoint.x >= 1 ||
            screenPoint.y <= 0 || screenPoint.y >= 1)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Verify if the device camera is looking at an object with a certain distance.
    /// </summary>
    /// <param name="go">Game object pose.</param>
    /// <param name="compare_distance">Distance between the device camera and the object.</param>
    /// <returns>If the device camera is looking at the object with the certain distance.</returns>
    private bool IsLookingAtObjectWith2Ddistance(Transform go, float compare_distance = 3)
    {
        var screenPoint = this.GetComponent<Camera>().WorldToViewportPoint(go.position);
        if (screenPoint.z <= 0 || screenPoint.x <= 0 || screenPoint.x >= 1 ||
            screenPoint.y <= 0 || screenPoint.y >= 1)
        {
            return false;
        }
        // Check the distance between the indicator and the camera.
        double distance = calculate2dDistance(go.transform.position, this.transform.position);
        if (distance >= compare_distance)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Verify if the user click intercepted a recognized plane.
    /// </summary>
    /// <returns>If the click was on a plane and the corresponding hit.</returns>
    private (bool, TrackableHit) verifyClickOnPlane()
    {
        TrackableHit hit;

        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

        bool hitfound = Frame.Raycast(Input.touches[0].position.x, Input.touches[0].position.y, raycastFilter, out hit);
        return (hitfound, hit);
    }

    /// <summary>
    /// Verify if the user clicked a UI button.
    /// </summary>
    /// <param name="pos">Click 2D position.</param>
    /// <returns>If the user clicked a UI button.</returns>
    private bool verifyClickOnButton(Vector2 pos)
    {
        clickOnUI = false;
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResult = new List<RaycastResult>();
        pointer.position = pos;
        EventSystem.current.RaycastAll(pointer, raycastResult);
        foreach (RaycastResult result in raycastResult)
        {
            if (result.gameObject.tag == "Button")
            {
                clickOnUI = true;
            }
        }
        raycastResult.Clear();
        return clickOnUI;
    }

    /// <summary>
    /// Verify if the camera is looking to a specif angle of the object.
    /// </summary>
    /// <param name="side">Object transform component.</param>
    /// <returns>If the camera is looking to a specif angle of the object.</returns>
    private bool IsLookingAtSide(Transform side)
    {
        // Check whether the bar is inside camera's view:
        var screenPoint = this.GetComponent<Camera>().WorldToViewportPoint(side.position);
        if (screenPoint.z <= 0 || screenPoint.x <= 0 || screenPoint.x >= 1 ||
            screenPoint.y <= 0 || screenPoint.y >= 1)
        {
            confMenuScript.changeText("Point to the virtual object.", Color.black);
            return false;
        }

        // Check the distance between the indicator and the camera.
        float distance = (side.position - this.GetComponent<Camera>().transform.position).magnitude;
        if (distance >= 2.5)
        {
            confMenuScript.changeText("Get closer to the virtual object.", Color.black);
            return false;
        }

        // Check the angle between the camera and the object.
        if (side.rotation.eulerAngles.y - this.GetComponent<Camera>().transform.rotation.eulerAngles.y >= -15 && side.rotation.eulerAngles.y - this.GetComponent<Camera>().transform.rotation.eulerAngles.y <= 15)
        {
            confMenuScript.changeText("Move the device around the virtual object until it turns green.", Color.black);
            return true;
        }
        else
        {
            confMenuScript.changeText("Move the device around the virtual object until it turns green.", Color.black);
            return false;
        }
    }

    /// <summary>
    /// Change application window.
    /// </summary>
    /// <param name="menu">Menu to change.</param>
    public void changeMenu(string menu)
    {
        if (menu == "initial") // Change to initial menu.
        {
            mode = Modes.INITIAL;
            initialMenu_v2.SetActive(true);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
            initialMenuScript.changeConfButtonInterectable(settingsMenuScript.phone_enabled);
        }
        else if (menu == "configure_pre_menu") // Change to configre menu.
        {
            mode = Modes.PRE_CONFIGURE;
            confPreMenuScript.changeNumberOfAnchors();
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(true);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
        }
        else if (menu == "experience_menu") // Change to the experience interface.
        {
            mode = Modes.EXPERIENCE;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(true);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
            this.GetComponent<EnhanceTrackExperience>().enabled = true;
            this.enabled = false;
            pointCloudVisualizer.SetActive(true);

        }
        else if (menu == "play_pre_menu") // Change to play menu.
        {
            mode = Modes.PRE_PLAY;
            playPreMenuScript.changeTheNumberOfAnchors();
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(true);
            playPreMenuScript.refreshDropdown();
            configurePreMenu.SetActive(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
        }
        else if (menu == "cloud_conf") // Change to the configuration interface on cloud anchors operating mode.
        {
            mode = Modes.CONFIGURE_CLOUD;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(true);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            confMenuScript.enableHosting(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
        }
        else if (menu == "file_conf") // Change to the configuration interface on motion tracking operating mode.
        {
            mode = Modes.CONFIGURE_FILE;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(true);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            confMenuScript.enableHosting(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
        }
        else if (menu == "play_cloud_static") // Change to the static objects game interface on cloud anchors operating mode.
        {
            mode = Modes.RESOLVING_CLOUD_STATIC;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(true);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            playMenuThesisScript.enMovButton(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = true;
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            roadLineRenderer.startWidth = 1.2f;
            roadLineRenderer.endWidth = 1.2f;
        }
        else if (menu == "play_cloud_moving") // Change to the moving object game interface on cloud anchors operating mode.
        {
            mode = Modes.RESOLVING_CLOUD_MOVING;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(true);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            playMenuThesisScript.enMovButton(true);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = true;
            lineRenderer.startWidth = 0.00f;
            lineRenderer.endWidth = 0.00f;
            roadLineRenderer.startWidth = 1.2f;
            roadLineRenderer.endWidth = 1.2f;
        }
        else if (menu == "play_file_static") // Change to the static objects game interface on motion tracking operating mode.
        {
            mode = Modes.RESOLVING_FILE_STATIC;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(true);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            playMenuThesisScript.enMovButton(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = true;
            playMenuThesisScript.changeCalibrationActive();
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            roadLineRenderer.startWidth = 1.2f;
            roadLineRenderer.endWidth = 1.2f;
        }
        else if (menu == "play_file_moving") // Change to the moving object game interface on motion tracking operating mode.
        {
            mode = Modes.RESOLVING_FILE_MOVING;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(true);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = true;
            playMenuThesisScript.enMovButton(false);
            playMenuThesisScript.changeCalibrationActive();
            lineRenderer.startWidth = 0.00f;
            lineRenderer.endWidth = 0.00f;
            roadLineRenderer.startWidth = 1.2f;
            roadLineRenderer.endWidth = 1.2f;
        }
        else if (menu == "end_menu") // Change to the end interface.
        {
            mode = Modes.END;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            endMenu.SetActive(true);
            settingsMenu.SetActive(false);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = true;
        }
        else if (menu == "settings") // Change to settings menu.
        {
            mode = Modes.SETTINGS;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            confMenuScript.enableHosting(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(true);
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
        }
        else if (menu == "user_manual") // Change to user manual menu.
        {
            mode = Modes.USER_MANUAL;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            confMenuScript.enableHosting(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
            userManualPreMenu.SetActive(true);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
        }
        else if (menu == "um_wheelchair") // Change to the wheelchair user manual
        {
            mode = Modes.USER_MANUAL;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            confMenuScript.enableHosting(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(true);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(false);
        }
        else if (menu == "um_conf") // Change to the configuration user manual
        {
            mode = Modes.USER_MANUAL;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            confMenuScript.enableHosting(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(true);
            userManualPlayMenu.SetActive(false);
        }
        else if (menu == "um_play") // Change to the play user manual
        {
            mode = Modes.USER_MANUAL;
            initialMenu_v2.SetActive(false);
            configureMenu.SetActive(false);
            experienceMenu.SetActive(false);
            playThesisMenu.SetActive(false);
            playPreMenu.SetActive(false);
            configurePreMenu.SetActive(false);
            confMenuScript.enableHosting(false);
            endMenu.SetActive(false);
            settingsMenu.SetActive(false);
            this.GetComponent<Rigidbody>().detectCollisions = false;
            userManualPreMenu.SetActive(false);
            userManualWheelchairMenu.SetActive(false);
            userManualConfigureMenu.SetActive(false);
            userManualPlayMenu.SetActive(true);
        }
    }

    /// <summary>
    /// Get the anchors and checkpoint number from a specific scenario.
    /// </summary>
    /// <param name="scenario">Chosen scenario.</param>
    /// <param name="mode">Operating mode.</param>
    /// <param name="username">Player username.</param>
    public void chosenScenario(string scenario, string mode, string username)
    {
        setScenarioAndUsername(scenario, username);
        MyThesisHistoryCollection coll = new MyThesisHistoryCollection();
        if (settingsMenuScript.phone_enabled)
        {
            coll = LoadCloudAnchorHistory();
        }
        else
        {
            string[] line_file = File.ReadAllLines(fileCoordsAnchorPath + "/Output_Desktop_Configuration/" + scenario + ".json");
            string[] lines = line_file[0].Split(new string[] { "]}" }, StringSplitOptions.None);
            for(int i = 0; i < lines.Length - 1; i++)
            { 
                coll.Collection.Add(JsonUtility.FromJson<MyThesisHistory>(lines[i] + "]}"));
            }
        }
        foreach (var cloud in coll.Collection)
        {
            if (cloud.Scenario == scenario)
            {
                anchorsToResolve.Add(cloud);
                foreach (var obj in cloud.ListAnchorObjects)
                {
                    if (obj.Prefab_name == "Goal")
                    {
                        checkpointsToGoal += 1;
                    }
                }
            }
        }
        changeMenu(mode);
        serverHandler(username);
    }

    /// <summary>
    /// Detect if the user reached a checkpoint or colided with a dodge object.
    /// </summary>
    /// <param name="collision">Collision occured</param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Checkpoint"))
        {
            Destroy(collision.gameObject);
            checkpointsReached += 1;
            playMenuThesisScript.changeFeedbackText2(checkpointsReached, checkpointsToGoal);
            explosionObject = Instantiate(explosion, collision.transform.GetComponent<BoxCollider>().bounds.center, collision.transform.rotation);
            Invoke("destroyExplosion", 3f);
            linePathDone.positionCount = eachFramePosition.Count;
            linePathDone.SetPositions(eachFramePosition.ToArray());

            // Handle score
            int aux_score = 500;
            if (mode == Modes.RESOLVING_CLOUD_STATIC || mode == Modes.RESOLVING_FILE_STATIC || mode == Modes.PLAY_FILE_STATIC || mode == Modes.PLAY_CLOUD_STATIC) {
                for(int i = (checkpointsReached - 1) * numPoints; i < checkpointsReached * numPoints; i++)
                {
                    double closest_distance = Math.Round(checkClosestLinePoint(lineRenderer.GetPosition(i)), 1);
                    if(closest_distance >= 1)
                    {
                        aux_score -= 10;
                    }
                    else
                    {
                        aux_score -= (int)((float)closest_distance * 10);
                    }
                }
            }
            else if (mode == Modes.RESOLVING_CLOUD_MOVING || mode == Modes.RESOLVING_FILE_MOVING || mode == Modes.PLAY_FILE_MOVING || mode == Modes.PLAY_CLOUD_MOVING)
            {
                double media = sum / num_sum;
                sum = 0;
                num_sum = 0;
                score += (int)(media * 10);
            }
            score += aux_score;

            if (settingsMenuScript.server_enabled)
            {
                if (client.clientSocket.Connected)
                {
                    client.message.setGameState("CHECKPOINT", DateTime.Now, collision.transform.position, collision.transform.rotation.eulerAngles,
                        checkpointsReached, score, timer);
                    client.SendData();
                }
            }
        }
        else if(collision.gameObject.CompareTag("Dodge")) {
            playMenuThesisScript.changeFeedbackText(false);
            score -= 100;
            resolvedGO.Remove(collision.gameObject);
            Destroy(collision.gameObject);
            dodgeExplosionObject = Instantiate(dodgeExplosion, collision.transform.GetComponent<BoxCollider>().bounds.center, collision.transform.rotation);
            Invoke("destroyDodgeExplosion", 3f);

            if (settingsMenuScript.server_enabled)
            {
                if (client.clientSocket.Connected)
                {
                    client.message.setGameState("DODGE_FAIL", DateTime.Now, collision.transform.position, collision.transform.rotation.eulerAngles,
                        checkpointsReached, score, timer);
                    client.SendData();
                }
            }
        }
        else
        {
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Check the closest point to the line representing the path done.
    /// </summary>
    /// <param name="point">Position to do calculations.</param>
    /// <returns>The distance to the closest path in the line.</returns>
    private double checkClosestLinePoint(Vector3 point)
    {
        double distance = 99999;
        for(int i = 0; i < linePathDone.positionCount; i++)
        {
            double aux_distance = calculate3dDistance(point, linePathDone.GetPosition(i));
            if(aux_distance < distance)
            {
                distance = aux_distance;
            }
        }
        return distance;
    }

    /// <summary>
    /// Destroy checkpoint explosion.
    /// </summary>
    void destroyExplosion()
    {
        Destroy(explosionObject);
    }

    /// <summary>
    /// Destroy the dodge explosion.
    /// </summary>
    void destroyDodgeExplosion()
    {
        Destroy(dodgeExplosionObject);
    }

    /// <summary>
    /// Enable the placement of a new anchor.
    /// </summary>
    public void EnableNewAnchor()
    {
        enableNewAnchor = true;
        confMenuScript.changeAnchorButtonInteract(false);
    }

    /// <summary>
    /// Enable the placement of a new virtual object.
    /// </summary>
    public void EnableNewObject()
    {
        enableNewObject = true;
        confMenuScript.enableHosting(false);
        confMenuScript.changeAnchorButtonInteract(false);
    }

    /// <summary>
    /// Convert from degreens to radians.
    /// </summary>
    /// <param name="angle">Angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    public double ConvertToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }

    /// <summary>
    /// Convert from radians to degrees
    /// </summary>
    /// <param name="angle">Angle in radians.</param>
    /// <returns>The angle in degrees.</returns>
    public float ConvertToDegrees(double angle)
    {
        return (float)(angle * (180 / Math.PI));
    }

    /// <summary>
    /// Calibrate function - Not used.
    /// </summary>
    public void calibrate()
    {
        if (mode == Modes.PLAY_FILE_MOVING || mode == Modes.PLAY_FILE_STATIC)
        {
            foreach (var go in resolvedGO)
            {
                Vector3 auxPos = adjustPosition[go] - go.transform.position;
                angleTillAnchor = ConvertToRadians((go.transform.rotation.eulerAngles.y));
                rotationMatrix.Clear();
                rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                rotationMatrix.Add(new Vector3(0, 1, 0));
                rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                Vector3 posGo = new Vector3(
                            auxPos.x * rotationMatrix[0].x + auxPos.y * rotationMatrix[0].y + auxPos.z * rotationMatrix[0].z,
                            auxPos.x * rotationMatrix[1].x + auxPos.y * rotationMatrix[1].y + auxPos.z * rotationMatrix[1].z,
                            auxPos.x * rotationMatrix[2].x + auxPos.y * rotationMatrix[2].y + auxPos.z * rotationMatrix[2].z);
                go.transform.Translate(posGo);
            }
        }
    }

    /// <summary>
    /// Set the chosen scenario.
    /// </summary>
    /// <param name="scenario">Chosen scenario.</param>
    public void setScenario(string scenario)
    {
        this.scenario = scenario;
    }

    /// <summary>
    /// Set the chosen scenario and the player username.
    /// </summary>
    /// <param name="scenario">Chosen scenario.</param>
    /// <param name="username">Player username.</param>
    public void setScenarioAndUsername(string scenario, string username)
    {
        setScenario(scenario);
        this.username = username;
    }

    /// <summary>
    /// Erase the game state when returning to the previous menu window.
    /// </summary>
    public void eraseOnBack()
    {
        firstObjectRotation.Clear();
        anchorsToResolve.Clear();
        adjustPosition.Clear();
        foreach (var go in anchorsResolvedFile)
        {
            Destroy(go);
        }
        anchorsResolvedFile.Clear();
        if(anchor != null)
        {
            Destroy(anchor);
        }
        if(gameObjectToHost != null)
        {
            Destroy(gameObjectToHost);
        }
        timer = 0;
        score = 0;
        checkpointsReached = 0;
        checkpointsToGoal = 0;
        if(movingObject != null)
        {
            Destroy(movingObject);
        }
        if(hintToMove != null) {
            //Destroy(hintToMove);
            hintToMove = new Vector3();
        }
        foreach(var go in resolvedGO)
        {
            Destroy(go);
        }
        resolvedGO.Clear();
        hostQuality.Clear();
        prefabsIdx = 0;
        respectivePrefabIdx.Clear();
        enableNewAnchor = false;
        confMenuScript.changeAnchorButtonInteract(true);
        enableNewObject = false;
        foreach (var go in listGameObjectsToHost)
        {
            Destroy(go);
        }
        listGameObjectsToHost.Clear();
        listGameObjectsInConfiguration.Clear();
        if (selectedArrow != null)
        {
            Destroy(selectedArrow);
        }
        scenario = "";
        angleTillAnchor = 0;
        rotationMatrix.Clear();
        foreach(var anch in anchorsToErase)
        {
            Destroy(anch);
        }
        anchorsToErase.Clear();
        foreach(var go in gameObjectsToErase)
        {
            Destroy(go);
        }
        gameObjectsToErase.Clear();
        if(client != null)
        {
            client.CloseSocket();
        }
        goals.Clear();
        controls.Clear();
        Vector3[] clear = new Vector3[0];
        lineRenderer.positionCount = 0;
        lineRenderer.SetPositions(clear);
        roadLineRenderer.positionCount = 0;
        roadLineRenderer.SetPositions(clear);
        linePathDone.positionCount = 0;
        linePathDone.SetPositions(clear);
        foreach (var xpanchor in xpAnchorsToErase)
        {
            Destroy(xpanchor);
        }
        xpAnchorsToErase.Clear();
    }

    /// <summary>
    /// Change the object properties using the '+' or '-' button on the manual insertion interface.
    /// </summary>
    /// <param name="index">Property index to be changed.</param>
    /// <param name="moreOrLess">Increase/decrease the property value.</param>
    public void changeManualMoreOrLess(int index, bool moreOrLess)
    {
        if (anchor != null && listGameObjectsToHost.Count != 0)
        {
            foreach (var go in listGameObjectsToHost)
            {
                if (go.GetComponent<LeanSelectable>() != null)
                {
                    if (go.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        selectedGo = go;
                        break;
                    }
                }
            }
        }

        switch (index)
        {
            case 0: //Change x value
                if (moreOrLess)
                {
                    selectedGo.transform.Translate(new Vector3(0.1f, 0, 0));
                }
                else
                {
                    selectedGo.transform.Translate(new Vector3(-0.1f, 0, 0));
                }
                break;
            case 1: //Change y value
                if (moreOrLess)
                {
                    selectedGo.transform.Translate(new Vector3(0, 0.1f , 0));
                }
                else
                {
                    selectedGo.transform.Translate(new Vector3(0, -0.1f, 0));
                }
                break;
            case 2: //Change z value
                if (moreOrLess)
                {

                    selectedGo.transform.Translate(new Vector3(0, 0, 0.1f));
                }
                else
                {
                    selectedGo.transform.Translate(new Vector3(0, 0, -0.1f));
                }
                break;
            case 3: //Change rotation value
                if (moreOrLess)
                {
                    selectedGo.transform.Rotate(0, 1, 0);
                }
                else
                {
                    selectedGo.transform.Rotate(0, -1, 0);
                }
                break;
            case 4: //Change scaling value
                float previousY = selectedGo.transform.localScale.y;
                if (moreOrLess)
                {
                    selectedGo.transform.localScale = new Vector3(selectedGo.transform.localScale.x * 1.1f, selectedGo.transform.localScale.y * 1.1f, selectedGo.transform.localScale.z * 1.1f);
                }
                else
                {
                    selectedGo.transform.localScale = new Vector3(selectedGo.transform.localScale.x * 0.9f, selectedGo.transform.localScale.y * 0.9f, selectedGo.transform.localScale.z * 0.9f);
                }
                break;
            case 5: //Change id value
                if (moreOrLess)
                {
                    if (selectedGo.GetComponent<PathHandler>().id < listGameObjectsInConfiguration.Count) {
                        listGameObjectsInConfiguration[selectedGo.GetComponent<PathHandler>().id].GetComponent<PathHandler>().id = selectedGo.GetComponent<PathHandler>().id;
                        selectedGo.GetComponent<PathHandler>().id = selectedGo.GetComponent<PathHandler>().id + 1;
                    }
                }
                else
                {
                    if (selectedGo.GetComponent<PathHandler>().id > 1)
                    {
                        listGameObjectsInConfiguration[selectedGo.GetComponent<PathHandler>().id - 2].GetComponent<PathHandler>().id = selectedGo.GetComponent<PathHandler>().id;
                        selectedGo.GetComponent<PathHandler>().id = selectedGo.GetComponent<PathHandler>().id - 1;
                    }
                }
                listGameObjectsInConfiguration.Sort((p1, p2) => p1.GetComponent<PathHandler>().id.CompareTo(p2.GetComponent<PathHandler>().id));
                break;
        }
    }

    /// <summary>
    /// Change the object properties using the typing input field on the manual insertion interface.
    /// </summary>
    /// <param name="index">Property index to be changed.</param>
    /// <param name="moreOrLess">Property new value.</param>
    public void changeManualByInput(int index, float value)
    {
        if (anchor != null && listGameObjectsToHost.Count != 0)
        {
            foreach (var go in listGameObjectsToHost)
            {
                if (go.GetComponent<LeanSelectable>() != null)
                {
                    if (go.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        selectedGo = go;
                        break;
                    }
                }
            }
        }

        angleTillAnchor = ConvertToRadians((selectedGo.transform.rotation.eulerAngles.y));
        rotationMatrix.Clear();
        rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
        rotationMatrix.Add(new Vector3(0, 1, 0));
        rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
        Vector3 auxPos = selectedGo.transform.position - anchor.transform.position;
        Vector3 textPositions = new Vector3(
            auxPos.x * rotationMatrix[0].x + auxPos.y * rotationMatrix[0].y + auxPos.z * rotationMatrix[0].z,
            auxPos.x * rotationMatrix[1].x + auxPos.y * rotationMatrix[1].y + auxPos.z * rotationMatrix[1].z,
            auxPos.x * rotationMatrix[2].x + auxPos.y * rotationMatrix[2].y + auxPos.z * rotationMatrix[2].z);

        switch (index)
        {
            case 0: // Change x value
                selectedGo.transform.Translate(new Vector3(value - textPositions.x, 0, 0));
                break;
            case 1: // Change y value
                selectedGo.transform.Translate(new Vector3(0, value - textPositions.y, 0));
                break;
            case 2: // Change z value
                selectedGo.transform.Translate(new Vector3(0, 0, value - textPositions.z));
                break;
            case 3: // Change rotation value
                selectedGo.transform.Rotate(0, value - selectedGo.transform.rotation.eulerAngles.y, 0);
                break;
            case 4: // Change scaling value
                float previousY = selectedGo.transform.localScale.y;
                selectedGo.transform.localScale = new Vector3(selectedGo.transform.localScale.x * (value / selectedGo.transform.localScale.x), selectedGo.transform.localScale.y * (value / selectedGo.transform.localScale.x), selectedGo.transform.localScale.z * (value / selectedGo.transform.localScale.x));
                break;
            case 5: // Change id value
                if (value > 0 && value <= listGameObjectsInConfiguration.Count)
                {
                    if (selectedGo.GetComponent<PathHandler>().id < value) { 
                        for (int i = selectedGo.GetComponent<PathHandler>().id; i < value; i++) {
                            listGameObjectsInConfiguration[i].GetComponent<PathHandler>().id = listGameObjectsInConfiguration[i].GetComponent<PathHandler>().id - 1;
                        }
                    }
                    else
                    {
                        for (int i = selectedGo.GetComponent<PathHandler>().id - 2; i >= value; i--)
                        {
                            listGameObjectsInConfiguration[i].GetComponent<PathHandler>().id = listGameObjectsInConfiguration[i].GetComponent<PathHandler>().id + 1;
                        }
                    }
                    selectedGo.GetComponent<PathHandler>().id = (int)value;
                    listGameObjectsInConfiguration.Sort((p1, p2) => p1.GetComponent<PathHandler>().id.CompareTo(p2.GetComponent<PathHandler>().id));
                }
                break;
        }
    }

    /// <summary>
    /// Connection to server.
    /// Enable sending messages to server.
    /// </summary>
    /// <param name="username">Player username.</param>
    void serverHandler(string username)
    {
        if (settingsMenuScript.server_enabled)
        {
            string add = settingsMenuScript.getAddressField();
            int port = settingsMenuScript.getPortField();
            if (add != "" && port != -1)
                client = new myClient(username, add, port);
            else if (add != "" && port == -1)
                client = new myClient(username, add);
            else if (add == "" && port != -1)
                client = new myClient(username, _port: port);
            else
                client = new myClient(username);
            client.ConnectToServer();
            while (!client.clientSocket.Connected) ;
            if (client.clientSocket.Connected)
            {
                client.SendData();
                if (mode == Modes.RESOLVING_CLOUD_STATIC || mode == Modes.RESOLVING_FILE_STATIC || mode == Modes.PLAY_FILE_STATIC || mode == Modes.PLAY_CLOUD_STATIC)
                {
                    client.message.setTypeOfGame("STATIC");
                }
                else if(mode == Modes.RESOLVING_CLOUD_MOVING || mode == Modes.RESOLVING_FILE_MOVING || mode == Modes.PLAY_FILE_MOVING || mode == Modes.PLAY_CLOUD_MOVING)
                {
                    client.message.setTypeOfGame("MOVING");
                }
                client.SendData();
            }
        }
    }

    /// <summary>
    /// Enable the moving object without finding anchors or virtual objects.
    /// </summary>
    public void enable_without_anyfind()
    {
        enable_without_anyfind_var = true;
    }

    /// <summary>
    /// Destroy the orientation axis from the selected object.
    /// </summary>
    public void destroySelectedObjectAxis()
    {
        foreach (var go in listGameObjectsToHost)
        {
            if (go.GetComponent<LeanSelectable>() != null)
            {
                if (go.GetComponent<LeanSelectable>().IsSelected == true)
                {
                    confMenuScript.destroyAxis(go);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Draw the orientation axis for the selected object.
    /// </summary>
    public void drawSelectedObjectAxis()
    {
        foreach (var go in listGameObjectsToHost)
        {
            if (go.GetComponent<LeanSelectable>() != null)
            {
                if (go.GetComponent<LeanSelectable>().IsSelected == true)
                {
                    confMenuScript.drawAxis(go);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Set the moving object velocity.
    /// </summary>
    /// <param name="index">Velocity index.</param>
    public void setVelocity(float index)
    {
        movObjVelocity = index;
    }

    /// <summary>
    /// Draw bezier curve with N degree.
    /// </summary>
    /// <param name="numEndPoints">Number of end points in the bezier curve.</param>
    public void DrawNCurve(int numEndPoints)
    {
        positions = new Vector3[numPoints * numEndPoints];
        lineRenderer.positionCount = numPoints * numEndPoints;
        roadLineRenderer.positionCount = numPoints * numEndPoints;
        for (int j = 0; j < numEndPoints; j++)
        {
            List<GameObject> inBetween = new List<GameObject>();
            if (j > 0)
            {
                inBetween.Add(goals[j - 1]);
            }
            else
            {
                aux_go.transform.position = new Vector3(0, goals[j].transform.position.y, 0);
                aux_go.transform.rotation = Quaternion.identity;
                aux_go.AddComponent<PathHandler>().id = 0;
                inBetween.Add(aux_go);
            }
            foreach (var control in controls)
            {
                if (j > 0)
                {
                    if (control.GetComponent<PathHandler>().id > goals[j - 1].GetComponent<PathHandler>().id &&
                        control.GetComponent<PathHandler>().id < goals[j].GetComponent<PathHandler>().id)
                    {
                        inBetween.Add(control);
                        inBetween.Sort((p1, p2) => p1.GetComponent<PathHandler>().id.CompareTo(p2.GetComponent<PathHandler>().id));
                    }
                }
                else
                {
                    if (control.GetComponent<PathHandler>().id > 0 &&
                        control.GetComponent<PathHandler>().id < goals[j].GetComponent<PathHandler>().id)
                    {
                        inBetween.Add(control);
                        inBetween.Sort((p1, p2) => p1.GetComponent<PathHandler>().id.CompareTo(p2.GetComponent<PathHandler>().id));
                    }
                }
            }
            inBetween.Add(goals[j]);
            for (int i = 1; i <= numPoints; i++)
            {
                float t = i / (float)numPoints;
                positions[(i - 1) + (j * numPoints)] = CalculateNBezierPoint(t, inBetween.Count - 1, inBetween); ;
            }
            lineRenderer.SetPositions(positions);

            Vector3[] roadPositions = new Vector3[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                roadPositions[i] = new Vector3(positions[i].x, positions[i].y - 0.03f, positions[i].z);
            }
            roadLineRenderer.SetPositions(roadPositions);

            if (settingsMenuScript.server_enabled)
            {
                if (client.clientSocket.Connected)
                {
                    client.message.setLine("LINE", DateTime.Now, positions);
                    client.SendData();
                }
            }
        }
    }

    /// <summary>
    /// Calculate the bezier point with degree N.
    /// </summary>
    /// <param name="t">...</param>
    /// <param name="numControlPoints">Number of control points.</param>
    /// <param name="list">List of game objects to access their position.</param>
    /// <returns>The bezier point.</returns>
    private Vector3 CalculateNBezierPoint(float t, int numControlPoints, List<GameObject> list)
    {
        Vector3 p = Mathf.Pow((1 - t), numControlPoints) * list[0].transform.position;
        for (int j = 1; j <= numControlPoints; j++)
        {
            int result = binomialCoeffienct(numControlPoints, j);
            p += result * Mathf.Pow((1 - t), numControlPoints - j) * Mathf.Pow(t, j) * list[j].transform.position;
        }
        return p;
    }

    /// <summary>
    /// Binomial coeffiecient calculation.
    /// </summary>
    /// <param name="n">Up number.</param>
    /// <param name="k">Down number.</param>
    /// <returns>The binomial calculation.</returns>
    private int binomialCoeffienct(int n, int k)
    {
        // Base Cases
        if (k > n)
            return 0;
        if (k == 0 || k == n)
            return 1;

        // Recur
        return binomialCoeffienct(n - 1, k - 1)
            + binomialCoeffienct(n - 1, k);
    }

    /// <summary>
    /// Fill the lists for the end points and for the control points (Bezier curve calculation purposes).
    /// </summary>
    /// <param name="goS">List of virtual objects in the scene.</param>
    private void fillGoalsAndControls(List<GameObject> goS)
    {
        controls.Clear();
        goals.Clear();
        foreach (GameObject go in goS)
        {
            if (go.CompareTag("Hint"))
            {
                controls.Add(go);
            }
            else if (go.CompareTag("Goal"))
            {
                goals.Add(go);
            }
        }
    }

    /// <summary>
    /// Verify if the configuration changed (a new object was added, the previous ones suffered from transformations).
    /// </summary>
    /// <returns>If the configuration changed.</returns>
    private bool configChanged()
    {
        if (verifyChangesList != null && listGameObjectsToHost != null)
        {
            if (listGameObjectsToHost.Count != verifyChangesList.Count)
            {
                verifyChangesList.Clear();
                foreach (var go in listGameObjectsToHost)
                {
                    verifyChangesList.Add((go.transform.position, go.name));
                }
                return true;
            }
            for (int i = 0; i < listGameObjectsToHost.Count; i++)
            {
                if (listGameObjectsToHost[i].transform.position != verifyChangesList[i].Item1 || listGameObjectsToHost[i].name != verifyChangesList[i].Item2)
                {
                    verifyChangesList.Clear();
                    foreach (var go in listGameObjectsToHost)
                    {
                        verifyChangesList.Add((go.transform.position, go.name));
                    }
                    return true;
                }
            }
        }
        verifyChangesList.Clear();
        foreach (var go in listGameObjectsToHost)
        {
            verifyChangesList.Add((go.transform.position, go.name));
        }
        return false;
    }

    /// <summary>
    /// Change between rotation and scaling for interacting with the virtual objects in the configuration modes.
    /// </summary>
    /// <param name="value">Slider value - Rotation(0)/Scale(1)</param>
    public void changeBetweenSlideRot(int value)
    {
        foreach (var go in listGameObjectsToHost)
        {
            go.GetComponent<LeanScale>().enabled = (value == 1);
            go.GetComponent<LeanRotateCustomAxis>().enabled = (value == 0);
        }
    }

    /// <summary>
    /// Remove the selected object from the scene.
    /// </summary>
    public void remove()
    {
        int removed_id = 214748364;
        foreach (var go in listGameObjectsToHost)
        {
            if (go.GetComponent<LeanSelectable>().IsSelected)
            {
                if (go.CompareTag("Hint") || go.CompareTag("Goal"))
                {
                    removed_id = go.GetComponent<PathHandler>().id;
                    listGameObjectsInConfiguration.Remove(go);
                }
                listGameObjectsToHost.Remove(go);
                firstObjectRotation.Remove(go);
                respectivePrefabIdx.Remove(go);
                go.GetComponent<LeanSelectable>().Deselect();
                Destroy(go);
                break;
            }
        }
        if(listGameObjectsToHost.Count == 0)
        {
            confMenuScript.enableHosting(false, false);
        }
        foreach (var go in listGameObjectsInConfiguration)
        {
            if (go.GetComponent<PathHandler>() != null)
            {
                if (go.GetComponent<PathHandler>().id > removed_id)
                {
                    go.GetComponent<PathHandler>().id -= 1;
                }
            }
        }
    }

    /// <summary>
    /// Verify if the device camera is looking away from the spotlight.
    /// </summary>
    private void Lookaway()
    {
        foreach(var go in resolvedGO)
        {
            if (go.CompareTag("LookAway"))
            {
                if (go.activeSelf)
                {
                    if (IsLookingAtObject(go.transform))
                    {
                        playMenuThesisScript.showFlashLight();
                        playMenuThesisScript.changeFeedbackText(false);
                        score -= 100;
                        if (settingsMenuScript.server_enabled)
                        {
                            if (client.clientSocket.Connected)
                            {
                                client.message.setGameState("LOOKAWAY_FAIL", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                    checkpointsReached, score, timer);
                                client.SendData();
                            }
                        }
                    }
                    else
                    {
                        playMenuThesisScript.changeFeedbackText(true);
                        if (settingsMenuScript.server_enabled)
                        {
                            if (client.clientSocket.Connected)
                            {
                                client.message.setGameState("LOOKAWAY_SUCC", DateTime.Now, go.transform.position, go.transform.rotation.eulerAngles,
                                    checkpointsReached, score, timer);
                                client.SendData();
                            }
                        }
                    }
                    resolvedGO.Remove(go);
                    Destroy(go);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Rotate the stop signal until it's vertical.
    /// </summary>
    private void rotateStop()
    {
        foreach (var go in resolvedGO)
        {
            if (go.CompareTag("Stop"))
            {
                if (go.activeSelf)
                {
                    go.transform.Rotate(-1, 0, 0);
                    if(go.transform.rotation.eulerAngles.x <= 0 || go.transform.rotation.eulerAngles.x > 90)
                    {
                        Stop();
                    }
                    else
                    {
                        Invoke("rotateStop", 0.024f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get the device pose when the stop signal it's vertical.
    /// </summary>
    private void Stop()
    {
        stoppedPosition = this.transform.position;
        Invoke("isStopped", 1.5f);
    }

    /// <summary>
    /// Verify if the device stopped.
    /// </summary>
    private void isStopped()
    {
        if(calculate3dDistance(stoppedPosition, transform.position) > 0.3)
        {
            if (settingsMenuScript.server_enabled)
            {
                if (client.clientSocket.Connected)
                {
                    client.message.setGameState("STOP_FAIL", DateTime.Now, this.transform.position, this.transform.rotation.eulerAngles,
                        checkpointsReached, score, timer);
                    client.SendData();
                }
            }
            score -= 100;
            playMenuThesisScript.changeFeedbackText(false);
        }
        else
        {
            playMenuThesisScript.changeFeedbackText(true);
            if (settingsMenuScript.server_enabled)
            {
                if (client.clientSocket.Connected)
                {
                    client.message.setGameState("STOP_SUCC", DateTime.Now, this.transform.position, this.transform.rotation.eulerAngles,
                        checkpointsReached, score, timer);
                    client.SendData();
                }
            }
        }
        foreach (var go in resolvedGO)
        {
            if (go.CompareTag("Stop"))
            {
                if (go.activeSelf)
                {
                    resolvedGO.Remove(go);
                    Destroy(go);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Rotate the truck wheels.
    /// </summary>
    /// <param name="go">The moving object transform component.</param>
    private void rotateWheels(Transform go)
    {
        for(int i = 0; i < go.childCount; i++)
        {
            if (go.GetChild(i).name.Contains("wheel"))
            {
                go.GetChild(i).Rotate(0, 0, 5);
            }
            if(go.GetChild(i).childCount > 0)
            {
                rotateWheels(go.GetChild(i));
            }
        }
    }
}