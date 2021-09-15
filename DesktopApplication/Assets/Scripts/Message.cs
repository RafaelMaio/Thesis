// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Client-server message structure.
// SPECIAL NOTES: 
// ===============================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Message structure.
/// </summary>
[Serializable]
public class Message
{
    /// <summary>
    /// Player username.
    /// </summary>
    public string user_name;

    /// <summary>
    /// Time when the first message was sent.
    /// </summary>
    public DateTime initial_time;

    /// <summary>
    /// Time when the first message was sent in string format.
    /// </summary>
    public string initial_time_s;

    /// <summary>
    /// Message Type.
    /// Types: REGISTRATION, CAMERA, LOOKAWAY_SUCC, LOOKAWAY_FAIL, SPOTLIGHT_OBJ, STOP_SUCC, STOP_FAIL, STOP_OBJ, DODGE_OBJ, DODGE_FAIL, MOVING_OBJECT, STATIC, MOVING, LINE, END, OUT_OF_ROAD, HINT, GOAL, CHECKPOINT
    /// </summary>
    public string type;

    /// <summary>
    /// Message timestamp.
    /// </summary>
    public TimeSpan current_timestamp;

    /// <summary>
    /// Message timestamp in string format.
    /// </summary>
    public string current_timestamp_s;

    /// <summary>
    /// Pose class variable.
    /// </summary>
    public AuxPose obj;

    /// <summary>
    /// GameState class variable.
    /// </summary>
    public GameState gameState;

    /// <summary>
    /// Line positions.
    /// </summary>
    public Vector3[] line;

    /// <summary>
    /// Message boundary.
    /// </summary>
    public string messageEnd = "split_by_this_message_end";
}

/// <summary>
/// Game State class -> saves the state of the game at the current timestamp.
/// </summary>
[Serializable]
public struct GameState
{
    /// <summary>
    /// Game elapse minutes.
    /// </summary>
    public float minutes;

    /// <summary>
    /// Game elapse seconds.
    /// </summary>
    public float seconds;

    /// <summary>
    /// Number of checkpoints caught at the game.
    /// </summary>
    public int num_checkpoints;

    /// <summary>
    /// Game score.
    /// </summary>
    public int score;

    /// <summary>
    /// Game State constructor.
    /// </summary>
    /// <param name="_score">Game score.</param>
    /// <param name="_time">Game elapsed time.</param>
    /// <param name="_num_checkpoints">Number of checkpoints caught at the game.</param>
    public GameState(int _score, float _time, int _num_checkpoints)
    {
        score = _score;
        minutes = (int)(_time / 60);
        seconds = (int)(_time % 60);
        num_checkpoints = _num_checkpoints;
    }
}

/// <summary>
/// Pose class -> saves the pose of an object.
/// </summary>
[Serializable]
public struct AuxPose
{
    /// <summary>
    /// Object x position.
    /// </summary>
    public float position_x;

    /// <summary>
    /// Object y position.
    /// </summary>
    public float position_y;

    /// <summary>
    /// Object z position.
    /// </summary>
    public float position_z;

    /// <summary>
    /// Object x rotation.
    /// </summary>
    public float rotation_x;

    /// <summary>
    /// Object y rotation.
    /// </summary>
    public float rotation_y;

    /// <summary>
    /// Object z rotation.
    /// </summary>
    public float rotation_z;

    /// <summary>
    /// Pose constructor.
    /// </summary>
    /// <param name="pos_x">Object x position.</param>
    /// <param name="pos_y">Object y position.</param>
    /// <param name="pos_z">Object z position.</param>
    /// <param name="rot_x">Object x rotation.</param>
    /// <param name="rot_y">Object y rotation.</param>
    /// <param name="rot_z">Object z rotation.</param>
    public AuxPose(float pos_x, float pos_y, float pos_z, float rot_x, float rot_y, float rot_z)
    {
        position_x = pos_x;
        position_y = pos_y;
        position_z = pos_z;
        rotation_x = rot_x;
        rotation_y = rot_y;
        rotation_z = rot_z;
    }

    /// <summary>
    /// Overrides ToString() method.
    /// </summary>
    /// <returns>Return the json string of this object.</returns>
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}