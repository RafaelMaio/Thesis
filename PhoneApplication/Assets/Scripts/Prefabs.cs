// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Contains every game object prefab.
// SPECIAL NOTES:
// ===============================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    /// <summary>
    /// Game objects prefabs.
    /// </summary>
    public GameObject arrowPrefab;
    public GameObject goalPrefab;
    public GameObject barrierPrefab;
    public GameObject conePrefab;
    public GameObject stopPrefab;
    public GameObject spotlightPrefab;

    /// <summary>
    /// Game objects name list.
    /// </summary>
    private string[] prefabNameArray = {"Arrow", "Goal", "Barrier", "Cone", "Stop", "Spotlight"};

    /// <summary>
    /// Connects the object prefab to its name.
    /// </summary>
    private Dictionary<string, GameObject> resolveMap = new Dictionary<string, GameObject>();

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        resolveMap.Add("Arrow", arrowPrefab);
        resolveMap.Add("Goal", goalPrefab);
        resolveMap.Add("Barrier", barrierPrefab);
        resolveMap.Add("Cone", conePrefab);
        resolveMap.Add("Stop", stopPrefab);
        resolveMap.Add("Spotlight", spotlightPrefab);
    }

    /// <summary>
    /// Gets the game objects name list.
    /// </summary>
    public string[] getPrefabNameArray()
    {
        return prefabNameArray;
    }

    /// <summary>
    /// Gets the dictionary that connects the object prefab to its name.
    /// </summary>
    public Dictionary<string, GameObject> getResolveMap()
    {
        return resolveMap;
    }
}