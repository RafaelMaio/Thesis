// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Reads the chosen performance file.
//               Based on the read line, simulates the game real performance.
// SPECIAL NOTES:
// ===============================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceFileReader : MonoBehaviour
{
    /// <summary>
    /// Objects prefabs.
    /// </summary>
    public GameObject wheelchairPrefab;
    public GameObject goalPrefab;
    public GameObject hintPrefab;
    public GameObject movingObjectPrefab;
    public GameObject spotlightPrefab;
    public GameObject stopPrefab;
    public GameObject dodgePrefab;

    /// <summary>
    /// Road path.
    /// </summary>
    public LineRenderer roadLineRenderer;

    /// <summary>
    /// White line delimiting the road path.
    /// </summary>
    public LineRenderer lineRenderer;

    /// <summary>
    /// Red line delimiting the path done by the user.
    /// </summary>
    public LineRenderer linePathDone;

    /// <summary>
    /// List of instatiated game objects.
    /// </summary>
    private List<GameObject> resolvedGo = new List<GameObject>();

    /// <summary>
    /// Game with static objects or with the moving object.
    /// </summary>
    private bool static_or_moving;

    /// <summary>
    /// Player starting position.
    /// </summary>
    private Vector3 starting_position;

    /// <summary>
    /// Player starting rotation.
    /// </summary>
    private Vector3 startion_rotation;

    /// <summary>
    /// Player game object.
    /// </summary>
    private GameObject wheelchair;

    /// <summary>
    /// Moving object.
    /// </summary>
    private GameObject movingObject;

    /// <summary>
    /// Counter to the score feedback text (making the text disappear after a while).
    /// </summary>
    int score_time_counter = 0;

    /// <summary>
    /// Previous score.
    /// </summary>
    int last_score;

    /// <summary>
    /// Saves the real camera positions while the game was being played.
    /// </summary>
    List<Vector3> cameraPositions = new List<Vector3>();

    /// <summary>
    /// Connection to the performance script.
    /// </summary>
    public PerformanceScript performanceScript;

    /// <summary>
    /// Saves every line from the file.
    /// </summary>
    string[] file_lines;

    /// <summary>
    /// Line index.
    /// </summary>
    int lineCounter = 0;

    /// <summary>
    /// Last line index.
    /// </summary>
    int lastlineCounter = -1;

    /// <summary>
    /// Date structure.
    /// </summary>
    DateTime date;

    /// <summary>
    /// Counts the visualization time.
    /// </summary>
    TimeSpan time;

    /// <summary>
    /// Initial timestamp.
    /// </summary>
    TimeSpan initial_timespan;

    /// <summary>
    /// Rotation angle.
    /// </summary>
    private double angleTillAnchor;

    /// <summary>
    /// Rotation matrix values.
    /// </summary>
    private List<Vector3> rotationMatrix = new List<Vector3>();

    /// <summary>
    /// Unity Update function.
    /// </summary>
    public void Update()
    {
        //Read every line from the file and concatenate the incomplete ones.
        if (file_lines != null)
        {
            if (lineCounter < file_lines.Length)
            {
                string line_to_split = file_lines[lineCounter];
                foreach (var splitted_line in line_to_split.Split(new string[] { "}{" }, StringSplitOptions.None))
                {
                    string line = splitted_line;
                    if(line_to_split.Split(new string[] { "}{" }, StringSplitOptions.None).Length > 1)
                    {
                        if(line == line_to_split.Split(new string[] { "}{" }, StringSplitOptions.None)[line_to_split.Split(new string[] { "}{" }, StringSplitOptions.None).Length - 1])
                        {
                            line = "{" + line;
                        }
                        else
                        {
                            line = line + "}";
                        }
                    }
                    Message m = JsonUtility.FromJson<Message>(line);
                    if (lastlineCounter != lineCounter)
                    {
                        lastlineCounter = lineCounter;
                        if (lineCounter == 0)
                        {
                            date = DateTime.Now;
                        }
                        if (lineCounter == 2)
                        {
                            initial_timespan = new TimeSpan(Int32.Parse(m.current_timestamp_s.Split(':')[0]), Int32.Parse(m.current_timestamp_s.Split(':')[1]),
                                Int32.Parse(m.current_timestamp_s.Split(':')[2].Split('.')[0]));
                        }
                        time = (DateTime.Now - date) + initial_timespan;
                        if (m.type == "STATIC" || m.type == "MOVING")
                        {
                            static_or_moving = (m.type == "STATIC");
                        }
                        else if (m.type == "CAMERA")
                        {
                            if (wheelchair == null)
                            {
                                wheelchair = Instantiate(wheelchairPrefab, starting_position, Quaternion.Euler(startion_rotation));
                                wheelchair.transform.GetChild(0).position -= new Vector3(0, starting_position.y, 0);
                            }
                            else
                            {
                                Vector3 pos = rotate_around_point(startion_rotation.y, new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z));
                                wheelchair.transform.position = starting_position + pos;
                                wheelchair.transform.rotation = Quaternion.Euler(startion_rotation + new Vector3(0, m.obj.rotation_y, 0));
                                if (roadLineRenderer.positionCount > 0)
                                {
                                    cameraPositions.Add(new Vector3(pos.x, roadLineRenderer.GetPosition(0).y - starting_position.y + 0.02f, pos.z) + starting_position);
                                    linePathDone.positionCount = linePathDone.positionCount + 1;
                                    linePathDone.SetPositions(cameraPositions.ToArray());
                                }
                            }
                        }
                        else if (m.type == "MOVING_OBJECT")
                        {
                            if (movingObject == null)
                            {
                                movingObject = Instantiate(movingObjectPrefab, starting_position, Quaternion.Euler(startion_rotation));
                            }
                            else
                            {
                                Vector3 pos = rotate_around_point(startion_rotation.y, new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z));
                                movingObject.transform.position = starting_position + pos;
                                movingObject.transform.rotation = Quaternion.Euler(startion_rotation + new Vector3(0, m.obj.rotation_y, 0));
                            }
                        }
                        else if (m.type == "LINE")
                        {
                            List<Vector3> aux_line = new List<Vector3>();
                            foreach (var point in m.line)
                            {
                                aux_line.Add(rotate_around_point(startion_rotation.y, point) + starting_position);
                            }
                            if (static_or_moving)
                            {
                                lineRenderer.positionCount = m.line.Length;
                                lineRenderer.SetPositions(aux_line.ToArray());
                            }
                            roadLineRenderer.positionCount = m.line.Length;
                            roadLineRenderer.SetPositions(aux_line.ToArray());
                        }
                        else if (m.type == "GOAL")
                        {
                            GameObject go = Instantiate(goalPrefab, rotate_around_point(startion_rotation.y, new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z)) + starting_position,
                                Quaternion.Euler(startion_rotation + new Vector3(m.obj.rotation_x, m.obj.rotation_y, m.obj.rotation_z)));
                            resolvedGo.Add(go);
                        }
                        else if (m.type == "HINT")
                        {
                            if (static_or_moving)
                            {
                                GameObject go = Instantiate(hintPrefab, starting_position + rotate_around_point(startion_rotation.y, new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z)),
                                    Quaternion.Euler(startion_rotation + new Vector3(m.obj.rotation_x, m.obj.rotation_y, m.obj.rotation_z)));
                                resolvedGo.Add(go);
                            }
                        }
                        else if (m.type == "SPOTLIGH_OBJ")
                        {
                            GameObject go = Instantiate(spotlightPrefab, starting_position + rotate_around_point(startion_rotation.y, new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z)),
                                Quaternion.Euler(startion_rotation + new Vector3(m.obj.rotation_x, m.obj.rotation_y, m.obj.rotation_z)));
                            resolvedGo.Add(go);
                        }
                        else if (m.type == "STOP_OBJ")
                        {
                            GameObject go = Instantiate(stopPrefab, starting_position + rotate_around_point(startion_rotation.y, new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z)),
                                Quaternion.Euler(startion_rotation + new Vector3(m.obj.rotation_x, m.obj.rotation_y, m.obj.rotation_z)));
                            resolvedGo.Add(go);
                        }
                        else if (m.type == "DODGE_OBJ")
                        {
                            GameObject go = Instantiate(dodgePrefab, starting_position + rotate_around_point(startion_rotation.y, new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z)),
                                Quaternion.Euler(startion_rotation + new Vector3(m.obj.rotation_x, m.obj.rotation_y, m.obj.rotation_z)));
                            resolvedGo.Add(go);
                        }
                        else if (m.type == "CHECKPOINT")
                        {
                            Destroy(closest_object("Goal", new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z), startion_rotation.y).transform.GetChild(0).GetChild(2).gameObject);
                            performanceScript.changeScoreText("+" + (m.gameState.score - last_score).ToString(), Color.green);
                            performanceScript.changeScoreFeedbackText("Checkpoint caught", Color.green);
                            score_time_counter = 0;
                        }
                        else if (m.type == "LOOKAWAY_SUCC")
                        {
                            GameObject go = closest_object("LookAway", new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z), startion_rotation.y);
                            resolvedGo.Remove(go);
                            Destroy(go);
                            performanceScript.changeScoreText("+0", Color.green);
                            performanceScript.changeScoreFeedbackText("Spotlight deflected", Color.green);
                            score_time_counter = 0;
                        }
                        else if (m.type == "LOOKAWAY_FAIL")
                        {
                            GameObject go = closest_object("LookAway", new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z), startion_rotation.y);
                            resolvedGo.Remove(go);
                            Destroy(go);
                            performanceScript.changeScoreText("-100", Color.red);
                            performanceScript.changeScoreFeedbackText("Hit by the light", Color.red);
                            score_time_counter = 0;
                        }
                        else if (m.type == "STOP_SUCC")
                        {
                            GameObject go = closest_object("Stop", new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z), startion_rotation.y);
                            resolvedGo.Remove(go);
                            Destroy(go);
                            performanceScript.changeScoreText("+0", Color.green);
                            performanceScript.changeScoreFeedbackText("Stopped with success", Color.green);
                            score_time_counter = 0;
                        }
                        else if (m.type == "STOP_FAIL")
                        {
                            GameObject go = closest_object("Stop", new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z), startion_rotation.y);
                            resolvedGo.Remove(go);
                            Destroy(go);
                            performanceScript.changeScoreText("-100", Color.red);
                            performanceScript.changeScoreFeedbackText("You need to stop", Color.red);
                            score_time_counter = 0;
                        }
                        else if (m.type == "DODGE_FAIL")
                        {
                            GameObject go = closest_object("Dodge", new Vector3(m.obj.position_x, m.obj.position_y, m.obj.position_z), startion_rotation.y);
                            resolvedGo.Remove(go);
                            Destroy(go);
                            performanceScript.changeScoreText("-100", Color.red);
                            performanceScript.changeScoreFeedbackText("Object collision", Color.red);
                            score_time_counter = 0;
                        }
                        else if (m.type == "OUT_OF_ROAD")
                        {
                            performanceScript.changeScoreText("-50", Color.red);
                            performanceScript.changeScoreFeedbackText("Out of the road", Color.red);
                            score_time_counter = 0;
                        }
                        else if (m.type == "END")
                        {
                            performanceScript.change_replay_active(true);
                        }
                        last_score = m.gameState.score;
                        performanceScript.changeInfoText(m.gameState.score, m.gameState.num_checkpoints, (int)m.gameState.minutes, (int)m.gameState.seconds);
                        if (lineCounter < 2)
                        {
                            score_time_counter += 1;
                            if (score_time_counter >= 50)
                            {
                                performanceScript.changeScoreText("", Color.red);
                                performanceScript.changeScoreFeedbackText("", Color.red);
                            }
                            lineCounter += 1;
                        }
                    }
                    if (m.current_timestamp_s != "")
                    {
                        string[] times = m.current_timestamp_s.Split(':');
                        bool time_passed_by = false;

                        if (time.Hours > float.Parse(times[0]))
                        {
                            time_passed_by = true;
                        }
                        else
                        {
                            if (time.Minutes > float.Parse(times[1]))
                            {
                                time_passed_by = true;
                            }
                            else
                            {
                                if (time.Seconds > float.Parse(times[2].Split('.')[0]))
                                {
                                    time_passed_by = true;
                                }
                                else
                                {
                                    if (time.Milliseconds > float.Parse(times[2].Split('.')[1]))
                                    {
                                        time_passed_by = true;
                                    }
                                }
                            }
                        }
                        time = (DateTime.Now - date) + initial_timespan;
                        if (time_passed_by)
                        {
                            lineCounter += 1;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set the file lines.
    /// </summary>
    /// <param name="_file_lines">File lines.</param>
    public void perform(string[] _file_lines)
    {
        file_lines = _file_lines;
    }

    /// <summary>
    /// Set the player starting pose.
    /// </summary>
    /// <param name="position">Player starting position.</param>
    /// <param name="rotation">Player starting rotation.</param>
    public void setPose(Vector3 position, Vector3 rotation)
    {
        startion_rotation = rotation;
        starting_position = position;
    }

    /// <summary>
    /// Verify the closest object to the player at a given message.
    /// </summary>
    /// <param name="tag">Object tag.</param>
    /// <param name="angle">Player starting rotation.</param>
    /// <param name="file_coords">Current player position.</param>
    /// <returns>The closest object to the player.</returns>
    private GameObject closest_object(string tag, Vector3 file_coords, float angle)
    {
        GameObject closest_go = resolvedGo[0];
        double aux_distance;
        double closest_distance = 99999;
        foreach (var go in resolvedGo)
        {
            if (go.CompareTag(tag))
            {
                aux_distance = calculate3dDistance(go.transform.position, rotate_around_point(angle, file_coords) + starting_position);
                if(closest_distance > aux_distance)
                {
                    closest_distance = aux_distance;
                    closest_go = go;
                }
            }
        }
        return closest_go;
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
    /// Convert from degreens to radians.
    /// </summary>
    /// <param name="angle">Angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    public double ConvertToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }

    /// <summary>
    /// Repeat the performance visualization for the chosen file.
    /// </summary>
    public void replay()
    {
        lineCounter = 0;
        score_time_counter = 0;
    }

    /// <summary>
    /// Rotate a point.
    /// </summary>
    /// <param name="angle">Rotation.</param>
    /// <param name="point">Point to be rotated.</param>
    public Vector3 rotate_around_point(float angle, Vector3 point)
    {
        angleTillAnchor = ConvertToRadians((-angle));
        rotationMatrix.Clear();
        rotationMatrix.Add(new Vector3((float)Math.Cos(angleTillAnchor), 0, -(float)Math.Sin(angleTillAnchor)));
        rotationMatrix.Add(new Vector3(0, 1, 0));
        rotationMatrix.Add(new Vector3((float)Math.Sin(angleTillAnchor), 0, (float)Math.Cos(angleTillAnchor)));
        Vector3 pos = new Vector3(
        point.x * rotationMatrix[0].x + point.y * rotationMatrix[0].y + point.z * rotationMatrix[0].z,
        point.x * rotationMatrix[1].x + point.y * rotationMatrix[1].y + point.z * rotationMatrix[1].z,
        point.x * rotationMatrix[2].x + point.y * rotationMatrix[2].y + point.z * rotationMatrix[2].z);
        return pos;
    }

    /// <summary>
    /// Erase the visualization state when returning to the previous menu window.
    /// </summary>
    public void eraseSimulation()
    {
        Vector3[] clearLineRenderers = new Vector3[0];
        roadLineRenderer.positionCount = 0;
        roadLineRenderer.SetPositions(clearLineRenderers);
        lineRenderer.positionCount = 0;
        lineRenderer.SetPositions(clearLineRenderers);
        linePathDone.positionCount = 0;
        linePathDone.SetPositions(clearLineRenderers);
        foreach (var go in resolvedGo)
        {
            Destroy(go);
        }
        resolvedGo.Clear();

        Destroy(wheelchair);
        if(movingObject != null)
        {
            Destroy(movingObject);
        }

        score_time_counter = 0;
        last_score = 0;

        cameraPositions.Clear();

        lineCounter = 0;
        lastlineCounter = -1;

        file_lines = new string[0];
    }
}