// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : 
// SPECIAL NOTES: Still in testing phase.
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using GoogleARCore;
using GoogleARCore.CrossPlatform;

public class EnhanceTrackExperience : MonoBehaviour
{
    private string fileCoordsInfoPath;
    private int fileCoordsCont = 0;

    public ExperienceMenuScript_2 experienceMenu2Script;

    private float avgPoints;
    private int sumPoints;
    private int countPoints;

    private bool anchorResolved = false;
    XPAnchor anchor;

    // Start is called before the first frame update
    void Start()
    {
        /* Verify the files that exist and create a new one for the result */
        fileCoordsInfoPath = Application.persistentDataPath;
        fileCoordsInfoPath += "/results" + fileCoordsCont.ToString() + ".csv";
        while (File.Exists(fileCoordsInfoPath))
        {
            fileCoordsCont += 1;
            fileCoordsInfoPath = Application.persistentDataPath;
            fileCoordsInfoPath += "/results" + fileCoordsCont.ToString() + ".csv";
        }
        File.Create(fileCoordsInfoPath);

        XPSession.ResolveCloudAnchor("ua-412e78b8cb4c8bc30d2f2be8ef5a8ebb").ThenAction(result =>
        {
            anchor = result.Anchor;
            anchorResolved = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        countPoints += 1;
        sumPoints += Frame.PointCloud.PointCount;
        avgPoints = sumPoints / countPoints;

        experienceMenu2Script.changeDebugTexts("Position: ("+ Math.Round(this.GetComponent<Camera>().transform.position.x, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.position.y, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.position.z, 2).ToString() + ")", 1);

        experienceMenu2Script.changeDebugTexts("Rotation: (" + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.x, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.y, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.z, 2).ToString() + ")", 2);

        experienceMenu2Script.changeDebugTexts("Distance: " + 
            Math.Round(calculate3dDistance(this.GetComponent<Camera>().transform.position, Vector3.zero), 2).ToString(), 3);

        experienceMenu2Script.changeDebugTexts("Nº of points: " + Frame.PointCloud.PointCount.ToString(), 4);

        experienceMenu2Script.changeDebugTexts("Avg nº of points: " + Math.Round(avgPoints, 2).ToString(), 5);

        if (anchorResolved)
        {
            experienceMenu2Script.changeDebugTexts("Anchor Position: (" + Math.Round(anchor.transform.position.x, 2).ToString() +
                "," + Math.Round(anchor.transform.position.y, 2).ToString() +
                "," + Math.Round(anchor.transform.position.z, 2).ToString() + ")", 6);

            experienceMenu2Script.changeDebugTexts("Anchor Rotation: (" + Math.Round(anchor.transform.rotation.eulerAngles.x, 2).ToString() +
                "," + Math.Round(anchor.transform.rotation.eulerAngles.y, 2).ToString() +
                "," + Math.Round(anchor.transform.rotation.eulerAngles.z, 2).ToString() + ")", 7);
        }
    }

    public void writeResults()
    {
        string toWrite = Math.Round(this.GetComponent<Camera>().transform.position.x, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.position.y, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.position.z, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.x, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.y, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.z, 2).ToString() +
                "," + Math.Round(calculate3dDistance(this.GetComponent<Camera>().transform.position, Vector3.zero), 2) +
                "," + Math.Round(avgPoints, 2).ToString();
        File.AppendAllText(fileCoordsInfoPath, toWrite + Environment.NewLine);
    }

    public double calculate3dDistance(Vector3 g, Vector3 h)
    {
        return Mathf.Sqrt(Mathf.Pow(g.x - h.x, 2) + Mathf.Pow(g.z - h.z, 2) + Mathf.Pow(g.y - h.y, 2));
    }
}
