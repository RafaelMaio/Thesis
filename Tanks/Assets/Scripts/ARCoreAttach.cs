using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using GoogleARCore;

public class ARCoreAttach : MonoBehaviour
{
    public void Start()
    {
        GameObject plan = GameObject.Find("Plan");
        if(plan != null)
        {
            plan.SetActive(false);
        }
    }

    public void Update()
    {
        if (this.transform.childCount > 0)
        {
            this.transform.GetChild(0).transform.position = this.transform.position;
            this.transform.GetChild(0).transform.rotation = this.transform.rotation;
        }
    }
}