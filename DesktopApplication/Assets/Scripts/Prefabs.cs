// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Contains every game object prefab and game object image.
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
    public GameObject spotlightPrefab;
    public GameObject stopPrefab;

    /// <summary>
    /// Game objects images.
    /// </summary>
    public Sprite arrowSprite;
    public Sprite goalSprite;
    public Sprite barrierSprite;
    public Sprite coneSprite;
    public Sprite spotlightSprite;
    public Sprite stopSprite;

    /// <summary>
    /// Game objects name list.
    /// </summary>
    private string[] prefabNameArray = { "Arrow", "Goal", "Barrier", "Cone", "Spotlight", "Stop" };

    /// <summary>
    /// Connects the object prefab to its name.
    /// </summary>
    private Dictionary<string, GameObject> resolveMap = new Dictionary<string, GameObject>();

    /// <summary>
    /// Connects the object image to its name.
    /// </summary>
    private Dictionary<string, Sprite> spriteMap = new Dictionary<string, Sprite>();

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        resolveMap.Add("Arrow", arrowPrefab);
        resolveMap.Add("Goal", goalPrefab);
        resolveMap.Add("Barrier", barrierPrefab);
        resolveMap.Add("Cone", conePrefab);
        resolveMap.Add("Spotlight", spotlightPrefab);
        resolveMap.Add("Stop", stopPrefab);
        spriteMap.Add("Arrow", arrowSprite);
        spriteMap.Add("Goal", goalSprite);
        spriteMap.Add("Barrier", barrierSprite);
        spriteMap.Add("Cone", coneSprite);
        spriteMap.Add("Spotlight", spotlightSprite);
        spriteMap.Add("Stop", stopSprite);
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

    /// <summary>
    /// Gets the dictionary that connects the object image to its name.
    /// </summary>
    public Dictionary<string, Sprite> getSpriteMap()
    {
        return spriteMap;
    }
}