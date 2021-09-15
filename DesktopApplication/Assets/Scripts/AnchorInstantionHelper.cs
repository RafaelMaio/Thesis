// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Struct for instatiate the object representing the anchor.
//               Fills the structure from a file.
// SPECIAL NOTES: 
// ===============================


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AnchorInstantionHelper
{
    /// <summary>
    /// Anchor id.
    /// </summary>
    public string Id;

    /// <summary>
    /// Anchor x position.
    /// </summary>
    public float X;

    /// <summary>
    /// Anchor y position.
    /// </summary>
    public float Y;

    /// <summary>
    /// Anchor z position.
    /// </summary>
    public float Z;

    /// <summary>
    /// Anchor y rotation.
    /// </summary>
    public float Rotation;

    /// <summary>
    /// Constructor -> Fills the structure.
    /// </summary>
    /// <param name="id">Anchor id.</param>
    /// <param name="x">Anchor x position.</param>
    /// <param name="y">Anchor y position.</param>
    /// <param name="z">Anchor z position.</param>
    /// <param name="rot">Anchor y rotation.</param>
    public AnchorInstantionHelper(string id, float x, float y, float z, float rot)
    {
        Id = id;
        X = x;
        Y = y;
        Z = z;
        Rotation = rot;
    }
}