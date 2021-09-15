// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Attach the player pose to the camera pose
// SPECIAL NOTES:
// ===============================

using UnityEngine;

public class ARCoreAttach : MonoBehaviour
{
    // <summary>
    // Unity start function.
    // Remove the plan when the game starts.
    // </summary>
    public void Start()
    {
        GameObject plan = GameObject.Find("Plan");
        if(plan != null)
        {
            plan.SetActive(false);
        }
    }

    // <summary>
    // Unity update function.
    // Keep the player attached to the camera.
    // </summary>
    public void Update()
    {
        if (this.transform.childCount > 0)
        {
            this.transform.GetChild(0).transform.position = this.transform.position;
            this.transform.GetChild(0).transform.rotation = this.transform.rotation;
        }
    }
}