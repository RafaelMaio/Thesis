// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Class for saving the anchor and the corresponding attached objects in JSON format.
// SPECIAL NOTES: Adapted by CloudAnchorHistory from ARCore
// ===============================
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A serializable struct the basic information of an object attached to the anchor.
/// </summary>
[Serializable]
public struct AnchorObject
{
    /// <summary>
    /// Object prefab type.
    /// </summary>
    public string Prefab_name;

    /// <summary>
    /// Object x position.
    /// </summary>
    public float X;

    /// <summary>
    /// Object y position.
    /// </summary>
    public float Y;

    /// <summary>
    /// Object z position.
    /// </summary>
    public float Z;

    /// <summary>
    /// Object y rotation.
    /// </summary>
    public float Rotation;

    /// <summary>
    /// Object x scale.
    /// </summary>
    public float Scale_X;

    /// <summary>
    /// Object y scale.
    /// </summary>
    public float Scale_Y;

    /// <summary>
    /// Object z scale.
    /// </summary>
    public float Scale_Z;

    /// <summary>
    /// The number in the moving object path -> Deprecated.
    /// </summary>
    public int PathNumber;

    /// <summary>
    /// The bezier index in the bezier curve.
    /// Only for goal objects and brown arrow objects.
    /// </summary>
    public int BezierNumber;

    /// <summary>
    /// Struct constructor.
    /// </summary>
    /// <param name="prefab_name">Object prefab type.</param>
    /// <param name="x">Object x position.</param>
    /// <param name="y">Object y position.</param>
    /// <param name="z">Object z position.</param>
    /// <param name="rotation">Object y rotation.</param>
    /// <param name="scale_x">Object x scale.</param>
    /// <param name="scale_y">Object y scale.</param>
    /// <param name="scale_z">Object z scale.</param>
    /// <param name="path_number">The number in the moving object path -> Deprecated.</param>
    /// <param name="bezier_number">The bezier index in the bezier curve.</param>
    public AnchorObject(string prefab_name, float x, float y, float z, float rotation, float scale_x, float scale_y, float scale_z, int path_number, int bezier_number)
    {
        Prefab_name = prefab_name;
        X = x;
        Y = y;
        Z = z;
        Rotation = rotation;
        Scale_X = scale_x;
        Scale_Y = scale_y;
        Scale_Z = scale_z;
        PathNumber = path_number;
        BezierNumber = bezier_number;
    }
}

/// <summary>
/// A serializable struct that stores the basic information of a persistent cloud anchor.
/// </summary>
[Serializable]
public struct MyThesisHistory
{

    public AnchorFileInfo anchorInfo;

    /// <summary>
    /// An informative name given by the user.
    /// </summary>
    public string Name;

    /// <summary>
    /// The scenario where is located.
    /// </summary>
    public string Scenario;

    /// <summary>
    /// The Cloud Anchor Id which is used for resolving.
    /// </summary>
    public string Id;

    /// <summary>
    /// List of objects attached to the anchor.
    /// </summary>
    public List<AnchorObject> ListAnchorObjects;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">An informative name given by the user.</param>
    /// <param name="id">The Cloud Anchor Id which is used for resolving.</param>
    /// <param name="scenario_name">The scenario where is located.</param>
    /// <param name="anchorObjects">List of objects attached to the anchor.</param>
    /// <param name="anchor_info">Anchor pose information class.</param>
    public MyThesisHistory(string name, string id, string scenario_name, List<AnchorObject> anchorObjects, AnchorFileInfo anchor_info = new AnchorFileInfo())
    {
        Name = name;
        Scenario = scenario_name;
        Id = id;
        ListAnchorObjects = anchorObjects;
        anchorInfo = anchor_info;
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

/// <summary>
/// A wrapper class for serializing a collection of <see cref="MyHistory"/>.
/// </summary>
[Serializable]
public class MyThesisHistoryCollection
{
    /// <summary>
    /// A list of Cloud Anchor History Data.
    /// </summary>
    public List<MyThesisHistory> Collection = new List<MyThesisHistory>();

    /// <summary>
    /// List of scenarios for filling the dropdown.
    /// </summary>
    public List<string> dropdownOptions = new List<string>();
}

/// <summary>
/// A serializable struct that stores the pose of an anchor.
/// </summary>
[Serializable]
public struct AnchorFileInfo
{
    /// <summary>
    /// Anchor x position.
    /// </summary>
    public float anchorX;

    /// <summary>
    /// Anchor y position.
    /// </summary>
    public float anchorY;

    /// <summary>
    /// Anchor z position.
    /// </summary>
    public float anchorZ;

    /// <summary>
    /// Anchor x rotation.
    /// </summary>
    public float anchorRotX;

    /// <summary>
    /// Anchor y rotation.
    /// </summary>
    public float anchorRotY;

    /// <summary>
    /// Anchor z rotation.
    /// </summary>
    public float anchorRotZ;

    /// <summary>
    /// Anchor x scale.
    /// </summary>
    public float anchorScaleX;

    /// <summary>
    /// Anchor y scale.
    /// </summary>
    public float anchorScaleY;

    /// <summary>
    /// Anchor z scale.
    /// </summary>
    public float anchorScaleZ;

    /// <summary>
    /// Anchor pose constuctor.
    /// </summary>
    /// <param name="anchor_x">Anchor x position.</param>
    /// <param name="anchor_y">Anchor y position.</param>
    /// <param name="anchor_z">Anchor z position.</param>
    /// <param name="anchor_rot_x">Anchor x rotation.</param>
    /// <param name="anchor_rot_y">Anchor y rotation.</param>
    /// <param name="anchor_rot_z">Anchor z rotation.</param>
    /// <param name="anchor_scale_x">Anchor x scale.</param>
    /// <param name="anchor_scale_y">Anchor y scale.</param>
    /// <param name="anchor_scale_z">Anchor z scale.</param>
    public AnchorFileInfo(float anchor_x, float anchor_y, float anchor_z, float anchor_rot_x, float anchor_rot_y, float anchor_rot_z,
        float anchor_scale_x, float anchor_scale_y, float anchor_scale_z)
    {
        anchorX = anchor_x;
        anchorY = anchor_y;
        anchorZ = anchor_z;
        anchorRotX = anchor_rot_x;
        anchorRotY = anchor_rot_y;
        anchorRotZ = anchor_rot_z;
        anchorScaleX = anchor_scale_x;
        anchorScaleY = anchor_scale_y;
        anchorScaleZ = anchor_scale_z;
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