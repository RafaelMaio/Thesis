// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Main script - Configuration logic.
//                             Draws the bezier curve.
// SPECIAL NOTES: Communicates with every menu script.
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using Lean.Touch;
using System;

public class bezier : MonoBehaviour
{
    /// <summary>
    /// The maximum number of anchors for the motion tracking configuration.
    /// </summary>
    public const int maxAnchors = 20;

    /// <summary>
    /// White line delimiting the road path.
    /// </summary>
    public LineRenderer lineRenderer;

    /// <summary>
    /// Road path.
    /// </summary>
    public LineRenderer roadLineRenderer;

    /// <summary>
    /// Anchor prefab.
    /// </summary>
    public GameObject anchorPrefab;

    /// <summary>
    /// Player representation prefab.
    /// </summary>
    public GameObject startingPrefab;

    /// <summary>
    /// Selection arrow prefab.
    /// </summary>
    public GameObject selectedArrowPrefab;

    /// <summary>
    /// Selection arrow game object.
    /// </summary>
    private GameObject selectedArrow;

    /// <summary>
    /// Previous selected object.
    /// </summary>
    private GameObject previousSelectedObject;

    /// <summary>
    /// Previous selected object rotation in every DoF.
    /// </summary>
    private Quaternion previousSelectedObjectRot;

    /// <summary>
    /// Menu scripts.
    /// </summary>
    public ConfigureMenuScript configureMenuScript;
    public MainMenuScript mainMenuScript;

    /// <summary>
    /// Number of points for the bezier curve between goals.
    /// </summary>
    private int numPoints = 50;

    /// <summary>
    /// Line points position.
    /// </summary>
    private Vector3[] positions;

    /// <summary>
    /// List of game objects in the scene.
    /// </summary>
    private List<GameObject> goS = new List<GameObject>();

    /// <summary>
    /// List of anchor representation objects in the scene.
    /// </summary>
    private List<GameObject> anchors = new List<GameObject>();

    /// <summary>
    /// List of goals - Bezier end points.
    /// </summary>
    private List<GameObject> goals = new List<GameObject>();

    /// <summary>
    /// List of arrows - Bezier control points.
    /// </summary>
    private List<GameObject> controls = new List<GameObject>();

    /// <summary>
    /// Verify if the configured changed (new object created, object position changed).
    /// </summary>
    private List<(Vector3, string)> verifyChangesList = new List<(Vector3,string)>();

    /// <summary>
    /// Auxiliar gameobject for the bezier path - representing the starting position.
    /// </summary>
    private GameObject aux_go;

    /// <summary>
    /// Conter - Number of anchors in the scene.
    /// </summary>
    private int numOfAnchors = 0;

    /// <summary>
    /// List of anchor information from the file.
    /// </summary>
    private List<AnchorInstantionHelper> anchor_ids = new List<AnchorInstantionHelper>();

    /// <summary>
    /// MyThesisHistory script - for saving the anchor and corresponding virtual objects relevant info.
    /// Adapted from ARCore application.
    /// </summary>
    private MyThesisHistory _hostedCloudAnchor;

    /// <summary>
    /// Application modes.
    /// </summary>
    public enum Modes {
        ///<summary>Initial menu mode.</summary>
        BEFORE_START,
        ///<summary>Placement of the player starting position.</summary>
        STARTING,
        ///<summary>Anchor placement for cloud operating mode.</summary>
        ANCHORS,
        ///<summary>Virtual game objects placement.</summary>
        OBJECTS,
        ///<summary>Anchor placement for motion tracking operating mode.</summary>
        ANCHORS_FILE
    };
    private Modes mode;

    /// <summary>
    /// Prefabs script, contains every game prefab.
    /// </summary>
    public Prefabs prefabs;

    /// <summary>
    /// Current prefab index.
    /// </summary>
    private int prefabsIdx = 0;

    /// <summary>
    /// Save the gameobject respective prefabe index.
    /// </summary>
    private Dictionary<GameObject, int> respectivePrefabIdx = new Dictionary<GameObject, int>();

    /// <summary>
    /// Angle in radians from the virtual object to store and resolve them in the right position relatively to the anchor.
    /// </summary>
    private double angleTillAnchor;

    /// <summary>
    /// Rotation matrix values.
    /// </summary>
    private List<Vector3> rotationMatrix = new List<Vector3>();

    /// <summary>
    /// Virtual object selected.
    /// </summary>
    private GameObject selectedGameObject;

    /// <summary>
    /// Associate virtual objects to the closest anchor.
    /// </summary>
    private Dictionary<GameObject, GameObject> assoc_obj_anch = new Dictionary<GameObject, GameObject>();

    /// <summary>
    /// List of purple lines assoating the game object to the closest anchor.
    /// </summary>
    private List<GameObject> linesDrawn = new List<GameObject>();

    /// <summary>
    /// Purple line material.
    /// </summary>
    public Material lineMaterial;

    /// <summary>
    /// Moving object - GameObject.
    /// </summary>
    private GameObject movingObject;

    /// <summary>
    /// Moving object prefab.
    /// </summary>
    public GameObject movingObjectPrefab;

    /// <summary>
    /// Moving object current line index.
    /// </summary>
    private float u = 1;

    /// <summary>
    /// Moving object auxiliary line index - for speed purposes.
    /// </summary>
    private float lineDivision = 0;

    /// <summary>
    /// Line positions between each line renderer position.
    /// </summary>
    private List<Vector3> line4 = new List<Vector3>();

    /// <summary>
    /// Moving object velocity index.
    /// </summary>
    private int movingObjectVelocity = 1;

    /// <summary>
    /// Line length (meters).
    /// </summary>
    private float lineLength = 0;

    /// <summary>
    ///  Usability Test Script
    /// </summary>
    public UsabilityTest usabilityTestScript;

    /// <summary>
    /// Unity Update function.
    /// </summary>
    private void Update()
    {
        //Space key to place anchors and game objects
        if (Input.GetKeyDown("space")) 
        {
            if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
            {
                if (numOfAnchors > 0)
                {
                    GameObject go;
                    //Anchor placement based on the starting position (if cloud operating mode)
                    if (mode == Modes.ANCHORS)
                    {
                        angleTillAnchor = ConvertToRadians((aux_go.transform.rotation.eulerAngles.y));
                        rotationMatrix.Clear();
                        rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                        rotationMatrix.Add(new Vector3(0, 1, 0));
                        rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                        Vector3 auxGoWorldSpace = new Vector3(
                        anchor_ids[anchor_ids.Count - numOfAnchors].X * rotationMatrix[0].x + anchor_ids[anchor_ids.Count - numOfAnchors].Y * rotationMatrix[0].y + anchor_ids[anchor_ids.Count - numOfAnchors].Z * rotationMatrix[0].z,
                        anchor_ids[anchor_ids.Count - numOfAnchors].X * rotationMatrix[1].x + anchor_ids[anchor_ids.Count - numOfAnchors].Y * rotationMatrix[1].y + anchor_ids[anchor_ids.Count - numOfAnchors].Z * rotationMatrix[1].z,
                        anchor_ids[anchor_ids.Count - numOfAnchors].X * rotationMatrix[2].x + anchor_ids[anchor_ids.Count - numOfAnchors].Y * rotationMatrix[2].y + anchor_ids[anchor_ids.Count - numOfAnchors].Z * rotationMatrix[2].z);
                        Vector3 auxPos = auxGoWorldSpace + aux_go.transform.position;
                        Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, anchor_ids[anchor_ids.Count - numOfAnchors].Rotation + aux_go.transform.rotation.eulerAngles.y, 0.0f));
                        go = Instantiate(anchorPrefab, auxPos, rotation);
                    }
                    else
                    {
                        go = Instantiate(anchorPrefab, Vector3.zero, Quaternion.identity);
                    }
                    anchors.Add(go);
                    foreach (GameObject anchor in anchors)
                    {
                        if (anchor.GetComponent<LeanSelectable>().IsSelected)
                        {
                            anchor.GetComponent<LeanSelectable>().Deselect();
                        }
                    }
                    go.GetComponent<LeanSelectable>().Select();
                    numOfAnchors -= 1;
                    configureMenuScript.changeAddText(numOfAnchors, prefabsIdx);
                    if (mode == Modes.ANCHORS)
                    {
                        if (numOfAnchors == 0)
                        {
                            configureMenuScript.changeSaveButtonInteractable(true);
                        }
                    }
                    else
                    {
                        if (!configureMenuScript.getSaveButtonInteractable())
                        {
                            configureMenuScript.changeSaveButtonInteractable(true);
                        }
                    }
                }
                else
                {
                    configureMenuScript.changeHelpText(false, "It's not possible to place more anchors.");
                }
            }
            //Object placement
            else if(mode == Modes.OBJECTS)
            {
                if (usabilityTestScript.getUsabilityEnabled())
                {
                    if (goS.Count > 0)
                    {
                        usabilityTestScript.stopTime(goS[goS.Count - 1].transform);
                    }
                    else
                    {
                        usabilityTestScript.setTime();
                    }
                }
                usabilityTestScript.placeNextObject(goS.Count);
                GameObject go = Instantiate(prefabs.getResolveMap()[prefabs.getPrefabNameArray()[prefabsIdx]], new Vector3(0, 0.05f, 0), Quaternion.identity);
                goS.Add(go);
                if (go.CompareTag("Hint") || go.CompareTag("Goal"))
                {
                    go.GetComponent<PathHandler>().id = goS.Count;
                }
                respectivePrefabIdx.Add(go, prefabsIdx);
                foreach (GameObject aux_go in goS)
                {
                    if (aux_go.GetComponent<LeanSelectable>().IsSelected)
                    {
                        aux_go.GetComponent<LeanSelectable>().Deselect();
                    }
                }
                go.GetComponent<LeanSelectable>().Select();
                configureMenuScript.changeSlide();
            }
        }

        //Draw the bezier curve
        if (configChanged())
        {
            fillGoalsAndControls(goS);
            DrawNCurve(goals.Count);
            associateAnchor();
            drawAssociationLine();
        }

        //Selection arrow handling
        bool selected = false;
        if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
        {
            foreach (var anchor in anchors)
            {
                if (anchor.GetComponent<LeanSelectable>().IsSelected)
                {
                    selected = true;
                    selectedGameObject = anchor;
                    if (previousSelectedObject != anchor)
                    {
                        if (selectedArrow != null)
                        {
                            configureMenuScript.destroyAxis(previousSelectedObject);
                            Destroy(selectedArrow);
                        }
                        arrowSelection(anchor);
                        configureMenuScript.drawAxis(anchor, aux_go);
                        previousSelectedObject = anchor;
                        configureMenuScript.changeHelpText(selected);
                        configureMenuScript.changeRemoveButtonInteractable(true);
                    }
                    if(previousSelectedObjectRot != anchor.transform.rotation)
                    {
                        configureMenuScript.drawAxis(anchor, aux_go);
                        previousSelectedObjectRot = anchor.transform.rotation;
                    }
                    break;
                }
            }
        }
        else if(mode == Modes.OBJECTS)
        {
            foreach (var go in goS)
            {
                if (go.GetComponent<LeanSelectable>().IsSelected)
                {
                    selected = true;
                    selectedGameObject = go;
                    if (previousSelectedObject != go)
                    {
                        if (selectedArrow != null)
                        {
                            Destroy(selectedArrow);
                            configureMenuScript.destroyAxis(previousSelectedObject);
                        }
                        configureMenuScript.changeSprite(prefabsIdx);
                        arrowSelection(go);
                        previousSelectedObject = go;
                        configureMenuScript.changeHelpText(selected);
                        configureMenuScript.drawAxis(go);
                        configureMenuScript.changeRemoveButtonInteractable(true);
                    }
                    break;
                }
            }

            //Moving object logic.
            if (movingObject == null)
            {
                if (lineRenderer.positionCount > 0)
                {
                    movingObject = Instantiate(movingObjectPrefab, lineRenderer.GetPosition(0), Quaternion.identity);
                    lineLength = calculateLineLength(Mathf.FloorToInt((u / numPoints)));
                }
            }
            else {
                if (lineRenderer.positionCount > 0)
                {
                    if (u < lineRenderer.positionCount)
                    {
                        if(u % numPoints == 0)
                        {
                            lineLength = calculateLineLength(Mathf.FloorToInt((u / numPoints)));
                        }
                        if (lineDivision == 0)
                        {
                            Vector3 aux_line = lineRenderer.GetPosition((int)u) - lineRenderer.GetPosition((int)(u - 1));
                            line4.Clear();

                            for (int i = 0; i < Mathf.RoundToInt(lineLength) - 1; i++)
                            {
                                line4.Add(lineRenderer.GetPosition((int)u - 1) + (i + 1) * aux_line / Mathf.RoundToInt(lineLength));
                            }
                            line4.Add(lineRenderer.GetPosition((int)u));
                            movingObject.transform.rotation = Quaternion.Euler(0, ConvertToDegrees(Mathf.Atan2(lineRenderer.GetPosition((int)u).x - lineRenderer.GetPosition((int)u - 1).x,
                                lineRenderer.GetPosition((int)u).z - lineRenderer.GetPosition((int)u - 1).z)), 0); 
                        }
                        movingObject.transform.position = line4[(int)lineDivision];
                        lineDivision += movingObjectVelocity;

                        if (lineDivision > Mathf.RoundToInt(lineLength) - 1)
                        {
                            lineDivision = 0;
                            u += 1;
                        }

                        /* Rotate wheels */
                        rotateWheels(movingObject.transform);
                    }
                    else
                    {
                        movingObjectVelocity += 1;
                        if(movingObjectVelocity > 4)
                        {
                            movingObjectVelocity = 1;
                        }
                        u = 1;
                        lineLength = calculateLineLength(Mathf.FloorToInt((u / numPoints)));
                    }
                }
                else
                {
                    Destroy(movingObject);
                }
            }
        }
        else
        {
            selected = true;
            selectedGameObject = aux_go;
        }
        if (!selected)
        {
            if (previousSelectedObject == aux_go)
            {
                configureMenuScript.destroyAxis(previousSelectedObject);
            }
            if (configureMenuScript.getManualInteractable())
            {
                configureMenuScript.changeManualInteractable(false);
                configureMenuScript.changeManualTexts(0.0f, 0.0f, 0.0f, 0, 0.0f, 0);
            }
            if (selectedArrow != null)
            {
                if (previousSelectedObject != null)
                {
                    configureMenuScript.destroyAxis(previousSelectedObject);
                    previousSelectedObject = null;
                    previousSelectedObjectRot = Quaternion.identity;
                }
                Destroy(selectedArrow);
                configureMenuScript.changeRemoveButtonInteractable(false);
                configureMenuScript.changeHelpText(selected);
            }
        }
        else
        {
            Vector3 auxGoWorldSpace = new Vector3();
            if (mode == Modes.OBJECTS)
            {
                auxGoWorldSpace = selectedGameObject.transform.position - assoc_obj_anch[selectedGameObject].transform.position;
            }
            else if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
            {
                Vector3 auxPos = selectedGameObject.transform.position - aux_go.transform.position;
                angleTillAnchor = ConvertToRadians((aux_go.transform.rotation.eulerAngles.y));
                rotationMatrix.Clear();
                rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                rotationMatrix.Add(new Vector3(0, 1, 0));
                rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                auxGoWorldSpace = new Vector3(
                auxPos.x * rotationMatrix[0].x + auxPos.y * rotationMatrix[0].y + auxPos.z * rotationMatrix[0].z,
                auxPos.x * rotationMatrix[1].x + auxPos.y * rotationMatrix[1].y + auxPos.z * rotationMatrix[1].z,
                auxPos.x * rotationMatrix[2].x + auxPos.y * rotationMatrix[2].y + auxPos.z * rotationMatrix[2].z);
            }
            else
            {
                auxGoWorldSpace = selectedGameObject.transform.position;
            }

            angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y));
            rotationMatrix.Clear();
            rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
            rotationMatrix.Add(new Vector3(0, 1, 0));
            rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));

            Vector3 textPositions = new Vector3(
                auxGoWorldSpace.x * rotationMatrix[0].x + auxGoWorldSpace.y * rotationMatrix[0].y + auxGoWorldSpace.z * rotationMatrix[0].z,
                auxGoWorldSpace.x * rotationMatrix[1].x + auxGoWorldSpace.y * rotationMatrix[1].y + auxGoWorldSpace.z * rotationMatrix[1].z,
                auxGoWorldSpace.x * rotationMatrix[2].x + auxGoWorldSpace.y   * rotationMatrix[2].y + auxGoWorldSpace.z * rotationMatrix[2].z);

            //Manual menu handler.
            if (mode == Modes.OBJECTS)
            {
                if (selectedGameObject.CompareTag("Hint") || selectedGameObject.CompareTag("Goal"))
                {
                    configureMenuScript.changeManualTexts(textPositions.x, textPositions.y, textPositions.z, selectedGameObject.transform.rotation.eulerAngles.y,
                        selectedGameObject.transform.localScale.x, selectedGameObject.GetComponent<PathHandler>().id);
                    configureMenuScript.changeManualInteractable(true);
                }
                else
                {
                    configureMenuScript.changeManualTexts(textPositions.x, textPositions.y, textPositions.z, selectedGameObject.transform.rotation.eulerAngles.y,
                        selectedGameObject.transform.localScale.x, 0);
                    configureMenuScript.changeManualInteractableAux_v2(true);
                }
            }
            else if(mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
            {
                configureMenuScript.changeManualTexts(auxGoWorldSpace.x, auxGoWorldSpace.y, auxGoWorldSpace.z,
                    selectedGameObject.transform.rotation.eulerAngles.y - aux_go.transform.rotation.eulerAngles.y, selectedGameObject.transform.localScale.x, 0);
                if (!configureMenuScript.getManualInteractable())
                {
                    configureMenuScript.changeManualInteractableAux(true);
                }
            }
            else
            {
                configureMenuScript.changeManualTexts(textPositions.x, textPositions.y, textPositions.z, selectedGameObject.transform.rotation.eulerAngles.y,
                       selectedGameObject.transform.localScale.x, 0);
                if (!configureMenuScript.getManualInteractable())
                {
                    configureMenuScript.changeManualInteractableAux(true);
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
        for(int i = startingIndex * numPoints; i < (startingIndex * numPoints) + numPoints - 1; i++)
        {
            distance += (float)calculate3dDistance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
        }
        return distance;
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
    /// Creates the selection arrow on top of the selected object.
    /// </summary>
    /// <param name="go">The selected object.</param>
    public void arrowSelection(GameObject go)
    {
        Vector3 arrowPos = go.transform.GetComponent<Collider>().bounds.center + new Vector3(0, go.GetComponent<Collider>().bounds.size.y / 2 + 0.2f, go.GetComponent<Collider>().bounds.size.z / 2 + 0.2f); ;
        selectedArrow = Instantiate(selectedArrowPrefab, arrowPos, new Quaternion(0, 0, 0, 0));
        selectedArrow.transform.Rotate(-30, 0, 0);
        selectedArrow.transform.SetParent(go.transform, true);
    }

    /// <summary>
    /// Draw bezier curve with N degree.
    /// </summary>
    /// <param name="numEndPoints">Number of end points in the bezier curve.</param>
    public void DrawNCurve(int numEndPoints)
    {
        GameObject drawLineObj = new GameObject();
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
                drawLineObj.transform.position = new Vector3(aux_go.transform.position.x, goals[j].transform.position.y, aux_go.transform.position.z);
                drawLineObj.transform.rotation = aux_go.transform.rotation;
                drawLineObj.AddComponent<PathHandler>().id = 0;
                inBetween.Add(drawLineObj);
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
            for(int i = 0; i < positions.Length; i++)
            {
                roadPositions[i] = new Vector3(positions[i].x, positions[i].y - 0.01f, positions[i].z);
            }
            roadLineRenderer.SetPositions(roadPositions);
        }
        Destroy(drawLineObj);
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
    /// Get the anchors information from file.
    /// </summary>
    /// <param name="path">Path to the file where the anchors information is stored.</param>
    /// <param name="cloudMode">Cloud/Motion tracking operating mode.</param>
    public void setFile(string path, bool cloudMode)
    {
        if (cloudMode)
        {
            string[] file_lines = File.ReadAllLines(path);
            numOfAnchors = Regex.Matches(file_lines[0], "anchorInfo").Count;
            string[] ids = file_lines[0].Split(new string[] { "Id\":\"" }, StringSplitOptions.None);
            string[] anchorX = file_lines[0].Split(new string[] { "anchorX\":" }, StringSplitOptions.None);
            string[] anchorY = file_lines[0].Split(new string[] { "anchorY\":" }, StringSplitOptions.None);
            string[] anchorZ = file_lines[0].Split(new string[] { "anchorZ\":" }, StringSplitOptions.None);
            string[] anchorRot = file_lines[0].Split(new string[] { "anchorRotY\":" }, StringSplitOptions.None);
            for (int i = 1; i <= numOfAnchors; i++)
            {
                AnchorInstantionHelper anchorHelper = new AnchorInstantionHelper(ids[i].Substring(0, ids[i].IndexOf("\"")), float.Parse(anchorX[i].Substring(0, anchorX[i].IndexOf(","))),
                    float.Parse(anchorY[i].Substring(0, anchorY[i].IndexOf(","))), float.Parse(anchorZ[i].Substring(0, anchorZ[i].IndexOf(","))), float.Parse(anchorRot[i].Substring(0, anchorRot[i].IndexOf(","))));
                anchor_ids.Add(anchorHelper);
            }
            configureMenuScript.changeAddText(numOfAnchors, prefabsIdx);
        }
        else
        {
            numOfAnchors = maxAnchors;
            configureMenuScript.changeAddText(numOfAnchors, prefabsIdx);
        }
    }

    /// <summary>
    /// Change the configuration mode.
    /// </summary>
    /// <param name="_anchorsMode">Current mode</param>
    /// <param name="k">Change to the next/previous mode.</param>
    public void setMode(Modes _anchorsMode, bool next)
    {
        mode = _anchorsMode;
        if (next)
        {
            if (mode == Modes.ANCHORS)
            {
                if (usabilityTestScript.getUsabilityEnabled())
                {
                    usabilityTestScript.stopTime(aux_go.transform);
                    aux_go.transform.position += new Vector3(0f, 1.4f, 0f);
                    usabilityTestScript.translateStartingPosition();
                }
                if (!mainMenuScript.getCloudMode())
                {
                    mode = Modes.ANCHORS_FILE;
                }
                aux_go.GetComponent<LeanTranslate>().enabled = false;
                aux_go.GetComponent<LeanRotateCustomAxis>().enabled = false;
                foreach (GameObject anchor in anchors)
                {
                    anchor.GetComponent<LeanSelectable>().enabled = true;
                    anchor.GetComponent<LeanTranslate>().enabled = true;
                    anchor.GetComponent<LeanRotateCustomAxis>().enabled = true;
                    if (anchor.GetComponent<LeanScale>() != null)
                    {
                        anchor.GetComponent<LeanScale>().enabled = true;
                    }
                }
            }
            else if (mode == Modes.OBJECTS)
            {
                foreach (GameObject anchor in anchors)
                {
                    anchor.GetComponent<LeanSelectable>().Deselect();
                    anchor.GetComponent<LeanSelectable>().enabled = false;
                    anchor.GetComponent<LeanTranslate>().enabled = false;
                    anchor.GetComponent<LeanRotateCustomAxis>().enabled = false;
                    if (anchor.GetComponent<LeanScale>() != null)
                    {
                        anchor.GetComponent<LeanScale>().enabled = false;
                    }
                }
                foreach(GameObject go in goS)
                {
                    go.GetComponent<LeanSelectable>().enabled = true;
                    go.GetComponent<LeanTranslate>().enabled = true;
                    go.GetComponent<LeanRotateCustomAxis>().enabled = true;
                    if (go.GetComponent<LeanScale>() != null)
                    {
                        go.GetComponent<LeanScale>().enabled = true;
                    }
                }
            }
        }
        else
        {
            if (mode == Modes.STARTING)
            {
                aux_go.GetComponent<LeanTranslate>().enabled = true;
                aux_go.GetComponent<LeanRotateCustomAxis>().enabled = true;
                foreach(GameObject anchor in anchors)
                {
                    anchor.GetComponent<LeanSelectable>().Deselect();
                    configureMenuScript.destroyAxis(anchor);
                    anchor.GetComponent<LeanSelectable>().enabled = false;
                    anchor.GetComponent<LeanTranslate>().enabled = false;
                    anchor.GetComponent<LeanRotateCustomAxis>().enabled = false;
                }
            }
            else if (mode == Modes.ANCHORS)
            {
                if (!mainMenuScript.getCloudMode())
                {
                    mode = Modes.ANCHORS_FILE;
                }
                foreach (GameObject anchor in anchors)
                {
                    anchor.GetComponent<LeanSelectable>().enabled = true;
                    anchor.GetComponent<LeanTranslate>().enabled = true;
                    anchor.GetComponent<LeanRotateCustomAxis>().enabled = true;
                    if (anchor.GetComponent<LeanScale>() != null)
                    {
                        anchor.GetComponent<LeanScale>().enabled = true;
                    }
                }
                foreach (GameObject go in goS)
                {
                    go.GetComponent<LeanSelectable>().enabled = false;
                    go.GetComponent<LeanTranslate>().enabled = false;
                    go.GetComponent<LeanRotateCustomAxis>().enabled = false;
                    if (go.GetComponent<LeanScale>() != null)
                    {
                        go.GetComponent<LeanScale>().enabled = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Change the object prefab.
    /// </summary>
    /// <param name="increment">Next or previous prefab (in the prefab list).</param>
    public void ChangePrefab(int increment)
    {
        prefabsIdx = prefabsIdx + increment;
        if (prefabsIdx >= prefabs.getPrefabNameArray().Length)
        {
            prefabsIdx = 0;
        }
        if (prefabsIdx < 0)
        {
            prefabsIdx = prefabs.getPrefabNameArray().Length - 1;
        }
        configureMenuScript.changeSprite(prefabsIdx);
        foreach (var go in goS)
        {
            if (go.GetComponent<LeanSelectable>().IsSelected)
            {
                Vector3 lastPos = go.transform.position;
                goS.Remove(go);
                respectivePrefabIdx.Remove(go);
                go.GetComponent<LeanSelectable>().Deselect();
                Destroy(go);

                GameObject newGo = Instantiate(prefabs.getResolveMap()[prefabs.getPrefabNameArray()[prefabsIdx]],
                                           lastPos,
                                           Quaternion.identity);

                goS.Add(newGo);
                if (newGo.CompareTag("Goal") || newGo.CompareTag("Hint"))
                {
                    newGo.GetComponent<PathHandler>().id = goS.Count;
                }
                respectivePrefabIdx.Add(newGo, prefabsIdx);

                arrowSelection(newGo);
                newGo.GetComponent<LeanSelectable>().Select();
                break;
            }
        }
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
            if (go.tag == "Hint")
            {
                controls.Add(go);
            }
            else if (go.tag == "Goal")
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
        if (verifyChangesList != null && goS != null)
        {
            if (goS.Count != verifyChangesList.Count)
            {
                verifyChangesList.Clear();
                foreach (var go in goS)
                {
                    verifyChangesList.Add((go.transform.position, go.name));
                }
                return true;
            }
            for (int i = 0; i < goS.Count; i++)
            {
                if (goS[i].transform.position != verifyChangesList[i].Item1 || goS[i].name != verifyChangesList[i].Item2)
                {
                    verifyChangesList.Clear();
                    foreach (var go in goS)
                    {
                        verifyChangesList.Add((go.transform.position, go.name));
                    }
                    return true;
                }
            }
        }
        verifyChangesList.Clear();
        foreach (var go in goS)
        {
            verifyChangesList.Add((go.transform.position, go.name));
        }
        return false;
    }

    /// <summary>
    /// Draw the orientation axis for the selected object.
    /// </summary>
    public void drawSelectedObjectAxis()
    {
        if (mode == Modes.OBJECTS)
        {
            foreach (var go in goS)
            {
                if (go.GetComponent<LeanSelectable>() != null)
                {
                    if (go.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        configureMenuScript.drawAxis(go);
                        break;
                    }
                }
            }
        }
        else if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
        {
            foreach(var anchor in anchors)
            {
                if (anchor.GetComponent<LeanSelectable>() != null)
                {
                    if (anchor.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        configureMenuScript.drawAxis(anchor, aux_go);
                        break;
                    }
                }
            }
        }
        else
        {
            configureMenuScript.drawAxis(aux_go);
        }
    }

    /// <summary>
    /// Destroy the orientation axis from the selected object.
    /// </summary>
    public void destroySelectedObjectAxis()
    {
        if (mode == Modes.OBJECTS)
        {
            foreach (var go in goS)
            {
                if (go.GetComponent<LeanSelectable>() != null)
                {
                    if (go.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        configureMenuScript.destroyAxis(go);
                        break;
                    }
                }
            }
        }
        else if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
        {
            foreach (var anchor in anchors)
            {
                if (anchor.GetComponent<LeanSelectable>() != null)
                {
                    if (anchor.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        configureMenuScript.destroyAxis(anchor);
                        break;
                    }
                }
            }
        }
        else
        {
            configureMenuScript.destroyAxis(aux_go);
        }
    }

    /// <summary>
    /// Change the object properties using the typing input field on the manual insertion interface.
    /// </summary>
    /// <param name="index">Property index to be changed.</param>
    /// <param name="moreOrLess">Property new value.</param>
    public void changeManualByInput(int index, float value)
    {
        if (mode == Modes.OBJECTS)
        {
            foreach (var go in goS)
            {
                if (go.GetComponent<LeanSelectable>() != null)
                {
                    if (go.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        selectedGameObject = go;
                        break;
                    }
                }
            }
        }
        else if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
        {
            foreach (var anchor in anchors)
            {
                if (anchor.GetComponent<LeanSelectable>() != null)
                {
                    if (anchor.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        selectedGameObject = anchor;
                        break;
                    }
                }
            }
        }
        else
        {
            selectedGameObject = aux_go;
        }

        switch (index)
        {
            case 0: // Change x value
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    angleTillAnchor = ConvertToRadians((aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 objPos = new Vector3(
                    selectedGameObject.transform.position.x * rotationMatrix[0].x + selectedGameObject.transform.position.y * rotationMatrix[0].y + selectedGameObject.transform.position.z * rotationMatrix[0].z,
                    selectedGameObject.transform.position.x * rotationMatrix[1].x + selectedGameObject.transform.position.y * rotationMatrix[1].y + selectedGameObject.transform.position.z * rotationMatrix[1].z,
                    selectedGameObject.transform.position.x * rotationMatrix[2].x + selectedGameObject.transform.position.y * rotationMatrix[2].y + selectedGameObject.transform.position.z * rotationMatrix[2].z);
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y - aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 auxGoWorldSpace = new Vector3(
                    (value - objPos.x) * rotationMatrix[0].x + 0 * rotationMatrix[0].y + 0 * rotationMatrix[0].z,
                    (value - objPos.x) * rotationMatrix[1].x + 0 * rotationMatrix[1].y + 0 * rotationMatrix[1].z,
                    (value - objPos.x) * rotationMatrix[2].x + 0 * rotationMatrix[2].y + 0 * rotationMatrix[2].z);
                    selectedGameObject.transform.Translate(auxGoWorldSpace);
                }
                else if(mode == Modes.OBJECTS)
                {
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 objPos = new Vector3(
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[0].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[0].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[0].z,
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[1].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[1].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[1].z,
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[2].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[2].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[2].z);

                    selectedGameObject.transform.Translate(new Vector3(value - objPos.x, 0, 0));
                }
                else
                {
                    selectedGameObject.transform.Translate(new Vector3(value - selectedGameObject.transform.position.x, 0, 0));
                }
                break;
            case 1: // Change y value
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    angleTillAnchor = ConvertToRadians((aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 objPos = new Vector3(
                    selectedGameObject.transform.position.x * rotationMatrix[0].x + selectedGameObject.transform.position.y * rotationMatrix[0].y + selectedGameObject.transform.position.z * rotationMatrix[0].z,
                    selectedGameObject.transform.position.x * rotationMatrix[1].x + selectedGameObject.transform.position.y * rotationMatrix[1].y + selectedGameObject.transform.position.z * rotationMatrix[1].z,
                    selectedGameObject.transform.position.x * rotationMatrix[2].x + selectedGameObject.transform.position.y * rotationMatrix[2].y + selectedGameObject.transform.position.z * rotationMatrix[2].z);
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y - aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 auxGoWorldSpace = new Vector3(
                    0 * rotationMatrix[0].x + (value - objPos.y) * rotationMatrix[0].y + 0 * rotationMatrix[0].z,
                    0 * rotationMatrix[1].x + (value - objPos.y) * rotationMatrix[1].y + 0 * rotationMatrix[1].z,
                    0 * rotationMatrix[2].x + (value - objPos.y) * rotationMatrix[2].y + 0 * rotationMatrix[2].z);
                    selectedGameObject.transform.Translate(auxGoWorldSpace);
                }
                else if (mode == Modes.OBJECTS)
                {
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 objPos = new Vector3(
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[0].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[0].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[0].z,
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[1].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[1].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[1].z,
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[2].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[2].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[2].z);

                    selectedGameObject.transform.Translate(new Vector3(0, (value - objPos.y), 0));
                }
                else
                {
                    selectedGameObject.transform.Translate(new Vector3(0, value - selectedGameObject.transform.position.y, 0));
                }
                break;
            case 2: // Change z value
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    angleTillAnchor = ConvertToRadians((aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 objPos = new Vector3(
                    selectedGameObject.transform.position.x * rotationMatrix[0].x + selectedGameObject.transform.position.y * rotationMatrix[0].y + selectedGameObject.transform.position.z * rotationMatrix[0].z,
                    selectedGameObject.transform.position.x * rotationMatrix[1].x + selectedGameObject.transform.position.y * rotationMatrix[1].y + selectedGameObject.transform.position.z * rotationMatrix[1].z,
                    selectedGameObject.transform.position.x * rotationMatrix[2].x + selectedGameObject.transform.position.y * rotationMatrix[2].y + selectedGameObject.transform.position.z * rotationMatrix[2].z);
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y - aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 auxGoWorldSpace = new Vector3(
                    0 * rotationMatrix[0].x + 0 * rotationMatrix[0].y + (value - objPos.z) * rotationMatrix[0].z,
                    0 * rotationMatrix[1].x + 0 * rotationMatrix[1].y + (value - objPos.z) * rotationMatrix[1].z,
                    0 * rotationMatrix[2].x + 0 * rotationMatrix[2].y + (value - objPos.z) * rotationMatrix[2].z);
                    selectedGameObject.transform.Translate(auxGoWorldSpace);
                }
                else if (mode == Modes.OBJECTS)
                {
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 objPos = new Vector3(
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[0].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[0].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[0].z,
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[1].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[1].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[1].z,
                    (selectedGameObject.transform.position.x - assoc_obj_anch[selectedGameObject].transform.position.x) * rotationMatrix[2].x + (selectedGameObject.transform.position.y - assoc_obj_anch[selectedGameObject].transform.position.y) * rotationMatrix[2].y + (selectedGameObject.transform.position.z - assoc_obj_anch[selectedGameObject].transform.position.z) * rotationMatrix[2].z);

                    selectedGameObject.transform.Translate(new Vector3(0, 0, value - objPos.z));
                }
                else
                {
                    selectedGameObject.transform.Translate(new Vector3(0, 0, value - selectedGameObject.transform.position.z));
                }
                break;
            case 3: // Change rotation value
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    selectedGameObject.transform.Rotate(0, value - selectedGameObject.transform.rotation.eulerAngles.y + aux_go.transform.rotation.eulerAngles.y, 0);
                }
                else
                {
                    selectedGameObject.transform.Rotate(0, value - selectedGameObject.transform.rotation.eulerAngles.y, 0);
                }
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    configureMenuScript.drawAxis(selectedGameObject, aux_go);
                }
                break;
            case 4: // Change scaling value
                float previousY = selectedGameObject.transform.localScale.y;
                selectedGameObject.transform.localScale = new Vector3(selectedGameObject.transform.localScale.x * (value / selectedGameObject.transform.localScale.x), selectedGameObject.transform.localScale.y * (value / selectedGameObject.transform.localScale.x), selectedGameObject.transform.localScale.z * (value / selectedGameObject.transform.localScale.x));
                break;
            case 5: // Change id value
                if (mode == Modes.OBJECTS) 
                { 
                    if (value > 0 && value <= goS.Count)
                    {
                        if (selectedGameObject.GetComponent<PathHandler>().id < value)
                        {
                            for (int i = selectedGameObject.GetComponent<PathHandler>().id; i < value; i++)
                            {
                                goS[i].GetComponent<PathHandler>().id = goS[i].GetComponent<PathHandler>().id - 1;
                            }
                        }
                        else
                        {
                            for (int i = selectedGameObject.GetComponent<PathHandler>().id - 2; i >= value; i--)
                            {
                                goS[i].GetComponent<PathHandler>().id = goS[i].GetComponent<PathHandler>().id + 1;
                            }
                        }
                        selectedGameObject.GetComponent<PathHandler>().id = (int)value;
                        goS.Sort((p1, p2) => p1.GetComponent<PathHandler>().id.CompareTo(p2.GetComponent<PathHandler>().id));
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Change the object properties using the '+' or '-' button on the manual insertion interface.
    /// </summary>
    /// <param name="index">Property index to be changed.</param>
    /// <param name="moreOrLess">Increase/decrease the property value.</param>
    public void changeManualMoreOrLess(int index, bool moreOrLess)
    {
        if (mode == Modes.OBJECTS)
        {
            foreach (var go in goS)
            {
                if (go.GetComponent<LeanSelectable>() != null)
                {
                    if (go.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        selectedGameObject = go;
                        break;
                    }
                }
            }
        }
        else if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
        {
            foreach (var anchor in anchors)
            {
                if (anchor.GetComponent<LeanSelectable>() != null)
                {
                    if (anchor.GetComponent<LeanSelectable>().IsSelected == true)
                    {
                        selectedGameObject = anchor;
                        break;
                    }
                }
            }
        }
        else
        {
            selectedGameObject = aux_go;
        }

        float value = 0;
        switch (index)
        {
            case 0: //Change x value
                if (moreOrLess)
                {
                    value = 0.1f;          
                }
                else
                {
                    value = -0.1f;
                }
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y - aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 auxGoWorldSpace = new Vector3(
                    value * rotationMatrix[0].x + 0 * rotationMatrix[0].y + 0 * rotationMatrix[0].z,
                    value * rotationMatrix[1].x + 0 * rotationMatrix[1].y + 0 * rotationMatrix[1].z,
                    value * rotationMatrix[2].x + 0 * rotationMatrix[2].y + 0 * rotationMatrix[2].z);
                    selectedGameObject.transform.Translate(auxGoWorldSpace);
                }
                else
                {
                    selectedGameObject.transform.Translate(new Vector3(value, 0, 0));
                }
                break;
            case 1: //Change y value
                if (moreOrLess)
                {
                    value = 0.1f;
                }
                else
                {
                    value = -0.1f;
                }
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y - aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 auxGoWorldSpace = new Vector3(
                    0 * rotationMatrix[0].x + value * rotationMatrix[0].y + 0 * rotationMatrix[0].z,
                    0 * rotationMatrix[1].x + value * rotationMatrix[1].y + 0 * rotationMatrix[1].z,
                    0 * rotationMatrix[2].x + value * rotationMatrix[2].y + 0 * rotationMatrix[2].z);
                    selectedGameObject.transform.Translate(auxGoWorldSpace);
                }
                else
                {
                    selectedGameObject.transform.Translate(new Vector3(0, value, 0));
                }
                break;
            case 2://Change z value
                if (moreOrLess)
                {
                    value = 0.1f;
                }
                else
                {
                    value = -0.1f;
                }
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    angleTillAnchor = ConvertToRadians((selectedGameObject.transform.rotation.eulerAngles.y - aux_go.transform.rotation.eulerAngles.y));
                    rotationMatrix.Clear();
                    rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
                    rotationMatrix.Add(new Vector3(0, 1, 0));
                    rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
                    Vector3 auxGoWorldSpace = new Vector3(
                    0 * rotationMatrix[0].x + 0 * rotationMatrix[0].y + value * rotationMatrix[0].z,
                    0 * rotationMatrix[1].x + 0 * rotationMatrix[1].y + value * rotationMatrix[1].z,
                    0 * rotationMatrix[2].x + 0 * rotationMatrix[2].y + value * rotationMatrix[2].z);
                    selectedGameObject.transform.Translate(auxGoWorldSpace);
                }
                else
                {
                    selectedGameObject.transform.Translate(new Vector3(0, 0, value));
                }
                break;
            case 3: //Change rotation value
                if (moreOrLess)
                {
                    selectedGameObject.transform.Rotate(0, 1, 0);
                }
                else
                {
                    selectedGameObject.transform.Rotate(0, -1, 0);
                }
                if (mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
                {
                    configureMenuScript.drawAxis(selectedGameObject, aux_go);
                }
                break;
            case 4: //Change scaling value
                float previousY = selectedGameObject.transform.localScale.y;
                if (moreOrLess)
                {
                    selectedGameObject.transform.localScale = new Vector3(selectedGameObject.transform.localScale.x * 1.1f, selectedGameObject.transform.localScale.y * 1.1f, selectedGameObject.transform.localScale.z * 1.1f);
                }
                else
                {
                    selectedGameObject.transform.localScale = new Vector3(selectedGameObject.transform.localScale.x * 0.9f, selectedGameObject.transform.localScale.y * 0.9f, selectedGameObject.transform.localScale.z * 0.9f);
                }
                break;
            case 5: //Change id value
                if (mode == Modes.OBJECTS)
                {
                    if (moreOrLess)
                    {
                        if (selectedGameObject.GetComponent<PathHandler>().id < goS.Count)
                        {
                            goS[selectedGameObject.GetComponent<PathHandler>().id].GetComponent<PathHandler>().id = selectedGameObject.GetComponent<PathHandler>().id;
                            selectedGameObject.GetComponent<PathHandler>().id = selectedGameObject.GetComponent<PathHandler>().id + 1;
                        }
                    }
                    else
                    {
                        if (selectedGameObject.GetComponent<PathHandler>().id > 1)
                        {
                            goS[selectedGameObject.GetComponent<PathHandler>().id - 2].GetComponent<PathHandler>().id = selectedGameObject.GetComponent<PathHandler>().id;
                            selectedGameObject.GetComponent<PathHandler>().id = selectedGameObject.GetComponent<PathHandler>().id - 1;
                        }
                    }
                    goS.Sort((p1, p2) => p1.GetComponent<PathHandler>().id.CompareTo(p2.GetComponent<PathHandler>().id));
                }
                break;
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
    /// Associate every placed game object to the closest anchor object.
    /// </summary>
    private void associateAnchor()
    {
        assoc_obj_anch.Clear();
        foreach(var go in goS)
        {
            double distance = 9999;
            foreach(var anchor in anchors)
            {
                double aux_distance = calculate3dDistance(go.transform.position, anchor.transform.position);
                if(distance > aux_distance)
                {
                    distance = aux_distance;
                    if (!assoc_obj_anch.ContainsKey(go))
                    {
                        assoc_obj_anch.Add(go, anchor);
                    }
                    else
                    {
                        assoc_obj_anch[go] = anchor;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draw the purple line between every game object and the closest anchor object.
    /// </summary>
    private void drawAssociationLine()
    {
        foreach(var line in linesDrawn)
        {
            Destroy(line);
        }
        linesDrawn.Clear();
        foreach (var association in assoc_obj_anch)
        {
            GameObject myLine = new GameObject("Line"+linesDrawn.Count.ToString());
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startColor = new Color32(143, 0, 254, 180); // Purple
            lr.endColor = new Color32(143, 0, 254, 180); // Purple
            lr.startWidth = 0.06f;
            lr.endWidth = 0.06f;
            lr.sortingOrder = 4;
            lr.SetPosition(0, association.Key.transform.position);
            lr.SetPosition(1, association.Value.transform.position);
            linesDrawn.Add(myLine);
        }
    }

    /// <summary>
    /// Write all the required configuration information into the output file.
    /// </summary>
    public void writeInFile()
    {
        string path = "Assets/Output/" + mainMenuScript.getFileName() + ".json";
        if (!File.Exists(path))
        {
            File.Create(path);
        }
        else
        {
            File.WriteAllText(path, string.Empty);
        }
        int counter = 0;
        foreach (GameObject anchor in anchors)
        {
            List<AnchorObject> auxAnchorObjectList = new List<AnchorObject>();


            Vector3 auxAnchorPos = anchor.transform.position - aux_go.transform.position;
            angleTillAnchor = ConvertToRadians((aux_go.transform.rotation.eulerAngles.y));
            rotationMatrix.Clear();
            rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
            rotationMatrix.Add(new Vector3(0, 1, 0));
            rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
            Vector3 auxGoWorldSpace = new Vector3(
            auxAnchorPos.x * rotationMatrix[0].x + auxAnchorPos.y * rotationMatrix[0].y + auxAnchorPos.z * rotationMatrix[0].z,
            auxAnchorPos.x * rotationMatrix[1].x + auxAnchorPos.y * rotationMatrix[1].y + auxAnchorPos.z * rotationMatrix[1].z,
            auxAnchorPos.x * rotationMatrix[2].x + auxAnchorPos.y * rotationMatrix[2].y + auxAnchorPos.z * rotationMatrix[2].z);


            angleTillAnchor = ConvertToRadians((anchor.transform.rotation.eulerAngles.y));
            rotationMatrix.Clear();
            rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
            rotationMatrix.Add(new Vector3(0, 1, 0));
            rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
            List<GameObject> listGameObjectsToHost = new List<GameObject>();
            foreach(var word in assoc_obj_anch)
            {
                if(word.Value == anchor)
                {
                    listGameObjectsToHost.Add(word.Key);
                }
            }
            foreach (var go in listGameObjectsToHost)
            {
                Vector3 auxPos = go.transform.position - anchor.transform.position;
                Vector3 pos = new Vector3(
                    auxPos.x * rotationMatrix[0].x + auxPos.y * rotationMatrix[0].y + auxPos.z * rotationMatrix[0].z,
                    auxPos.x * rotationMatrix[1].x + auxPos.y * rotationMatrix[1].y + auxPos.z * rotationMatrix[1].z,
                    auxPos.x * rotationMatrix[2].x + auxPos.y * rotationMatrix[2].y + auxPos.z * rotationMatrix[2].z);
                float rot = go.transform.rotation.eulerAngles.y - anchor.transform.rotation.eulerAngles.y;
                AnchorObject auxAnchorObject;
                if (go.CompareTag("Hint") || go.CompareTag("Goal"))
                {
                    auxAnchorObject = new AnchorObject(prefabs.getPrefabNameArray()[respectivePrefabIdx[go]], pos.x, pos.y, pos.z, rot,
                        go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z, 0, go.GetComponent<PathHandler>().id);
                }
                else
                {
                    auxAnchorObject = new AnchorObject(prefabs.getPrefabNameArray()[respectivePrefabIdx[go]], pos.x, pos.y, pos.z, rot,
                        go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z, 0, 0);
                }
                auxAnchorObjectList.Add(auxAnchorObject);
            }
            AnchorFileInfo auxAnchorFileInfo = new AnchorFileInfo(auxGoWorldSpace.x, auxGoWorldSpace.y, auxGoWorldSpace.z,
                   anchor.transform.rotation.eulerAngles.x, anchor.transform.rotation.eulerAngles.y - aux_go.transform.rotation.eulerAngles.y, anchor.transform.rotation.eulerAngles.z,
                   anchor.transform.localScale.x, anchor.transform.localScale.y, anchor.transform.localScale.z);

            if (anchor_ids.Count > 0)
            {
                _hostedCloudAnchor = new MyThesisHistory("CloudAnchor" + counter.ToString(), anchor_ids[counter].Id, mainMenuScript.getFileName(), auxAnchorObjectList, auxAnchorFileInfo);
            }
            else
            {
                _hostedCloudAnchor = new MyThesisHistory("CloudAnchor" + counter.ToString(), "id"+counter.ToString(), mainMenuScript.getFileName(), auxAnchorObjectList, auxAnchorFileInfo);
            }
            counter += 1;
            File.AppendAllText(path, JsonUtility.ToJson(_hostedCloudAnchor));
        }
    }

    /// <summary>
    /// Get the number of anchor objects in the scene.
    /// </summary>
    /// <returns>The number of anchor objects in the scene</returns>
    public int getNumOfAnchors()
    {
        return numOfAnchors;
    }

    /// <summary>
    /// Remove the selected object from the scene.
    /// </summary>
    public void remove()
    {
        if (mode == Modes.OBJECTS)
        {
            int removed_id = 99999;
            foreach (var go in goS)
            {
                if (go.GetComponent<LeanSelectable>().IsSelected)
                {
                    if (go.CompareTag("Hint") || go.CompareTag("Goal"))
                    {
                        removed_id = go.GetComponent<PathHandler>().id;
                    }
                    goS.Remove(go);
                    respectivePrefabIdx.Remove(go);
                    go.GetComponent<LeanSelectable>().Deselect();
                    Destroy(go);
                    break;
                }
            }
            foreach(var go in goS)
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
        else if(mode == Modes.ANCHORS || mode == Modes.ANCHORS_FILE)
        {
            foreach (var anchor in anchors)
            {
                if (anchor.GetComponent<LeanSelectable>().IsSelected)
                {
                    anchors.Remove(anchor);
                    anchor.GetComponent<LeanSelectable>().Deselect();
                    Destroy(anchor);
                    numOfAnchors += 1;
                    configureMenuScript.changeAddText(numOfAnchors);
                    if(mode == Modes.ANCHORS)
                    {
                        configureMenuScript.changeSaveButtonInteractable(false);
                    }
                    else
                    {
                        if(numOfAnchors == 20)
                        {
                            configureMenuScript.changeSaveButtonInteractable(false);
                        }
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Change between rotation and scaling for interacting with the virtual objects in the configuration modes.
    /// </summary>
    /// <param name="value">Slider value - Rotation(0)/Scale(1)</param>
    public void changeBetweenSlideRot(int value)
    {
        foreach(var go in goS)
        {
            go.GetComponent<LeanScale>().enabled = (value == 1);
            go.GetComponent<LeanRotateCustomAxis>().enabled = (value == 0);
        }
    }

    /// <summary>
    /// Rotate the truck wheels.
    /// </summary>
    /// <param name="go">The moving object transform component.</param>
    private void rotateWheels(Transform go)
    {
        for (int i = 0; i < go.childCount; i++)
        {
            if (go.GetChild(i).name.Contains("wheel"))
            {
                go.GetChild(i).Rotate(0, 0, 5);
            }
            if (go.GetChild(i).childCount > 0)
            {
                rotateWheels(go.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Erase the configuration state when returning to the previous menu window.
    /// </summary>
    public void backToMain() { 
        foreach(GameObject go in goS)
        {
            Destroy(go);
        }
        goS.Clear();
        foreach(GameObject anchor in anchors)
        {
            Destroy(anchor);
        }
        anchors.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.SetPositions(new Vector3[0]);
        roadLineRenderer.positionCount = 0;
        roadLineRenderer.SetPositions(new Vector3[0]);
        verifyChangesList.Clear();
        goals.Clear();
        controls.Clear();
        Destroy(aux_go);
        mode = Modes.BEFORE_START;
        anchor_ids.Clear();
        prefabsIdx = 0;
        respectivePrefabIdx.Clear();
        assoc_obj_anch.Clear();
        foreach(GameObject line in linesDrawn){
            Destroy(line);
        }
        linesDrawn.Clear();
        if(movingObject != null)
        {
            Destroy(movingObject);
        }
        mainMenuScript.activate(false);
    }

    /// <summary>
    /// Unity OnEnable function.
    /// Creates the starting game object, representing the player starting pose.
    /// </summary>
    public void OnEnable()
    {
        if (aux_go == null)
        {
            aux_go = Instantiate(startingPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            aux_go.AddComponent<PathHandler>().id = 0;
            previousSelectedObject = aux_go;
        }
    }
}