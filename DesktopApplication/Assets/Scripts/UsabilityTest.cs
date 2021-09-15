// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the usability test.
// SPECIAL NOTES: Only used for the usability tests.
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class UsabilityTest : MonoBehaviour
{
    /// <summary>
    /// For verifying if the usability tests option is enabled.
    /// </summary>
    private bool usabilityEnabled = false;

    /// <summary>
    /// Participant identifier.
    /// </summary>
    private int participantId = 0;

    /// <summary>
    /// Prefabs for the transparent objects.
    /// </summary>
    public GameObject startingPositionTransparentPrefab;
    public GameObject arrowTransparentPrefab;
    public GameObject barrierTransparentPrefab;
    public GameObject goalTransparentPrefab;

    /// <summary>
    /// Transparent objects.
    /// </summary>
    private GameObject startingPositionTransparent;
    private GameObject arrowTransparent1;
    private GameObject arrowTransparent2;
    private GameObject barrierTransparent;
    private GameObject goalTransparent;

    /// <summary>
    /// List of game objects.
    /// </summary>
    private List<GameObject> goList = new List<GameObject>();

    /// <summary>
    /// File path for saving the usability tests results.
    /// </summary>
    private string filePathName = "Assets/UsabilityTest/testConfDesktop.txt";

    /// <summary>
    /// Usability test date.
    /// </summary>
    private DateTime date;

    /// <summary>
    /// Number of objects placed in the usability test.
    /// </summary>
    private int counter = 0;

    /// <summary>
    /// Connection to the configuration window script.
    /// </summary>
    public ConfigureMenuScript configureMenuScript;

    /// <summary>
    /// Unity OnEnable function.
    /// </summary>
    void OnEnable()
    {
        if (usabilityEnabled)
        {
            startingPositionTransparent = Instantiate(startingPositionTransparentPrefab, new Vector3(14.5f, 0, 1.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
            changeSortOrder(startingPositionTransparent.transform);
            date = DateTime.Now;
            goList.Add(startingPositionTransparent);
        }
    }

    /// <summary>
    /// Enable the next object placement.
    /// </summary>
    /// <param name="numObject">Number of the last placed object.</param>
    public void placeNextObject(int numObject)
    {
        if (usabilityEnabled)
        {
            counter += 1;
            configureMenuScript.changeNumObjectsText(numObject);
            if (numObject == 0)
            {
                arrowTransparent1 = Instantiate(arrowTransparentPrefab, new Vector3(14.3f, 0, -2.9f), Quaternion.Euler(new Vector3(0, 90, 0)));
                changeSortOrder(arrowTransparent1.transform);
                goList.Add(arrowTransparent1);
            }
            else if (numObject == 1)
            {
                arrowTransparent2 = Instantiate(arrowTransparentPrefab, new Vector3(12.5f, 0, -1.2f), Quaternion.Euler(new Vector3(0, 65, 0)));
                arrowTransparent2.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                changeSortOrder(arrowTransparent2.transform);
                goList.Add(arrowTransparent2);
            }
            else if (numObject == 2)
            {
                goalTransparent = Instantiate(goalTransparentPrefab, new Vector3(10f, 0, -3f), Quaternion.Euler(new Vector3(0, 55, 0)));
                changeSortOrder(goalTransparent.transform);
                goList.Add(goalTransparent);
            }
            else if (numObject == 3)
            {
                barrierTransparent = Instantiate(barrierTransparentPrefab, new Vector3(11.8f, 0, -2.9f), Quaternion.Euler(new Vector3(0, 300, 0)));
                changeSortOrder(barrierTransparent.transform);
                barrierTransparent.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                goList.Add(barrierTransparent);
            }
            else {
                configureMenuScript.handleShowText(true);
            }
        }
    }

    /// <summary>
    /// Verify if the usability tests are enabled.
    /// </summary>
    public bool getUsabilityEnabled()
    {
        return usabilityEnabled;
    }

    /// <summary>
    /// Change the transparent object sorting order, so they appear behind the opaque ones.
    /// </summary>
    /// <param name="objectChild">Object piece transform component.</param>
    private void changeSortOrder(Transform objectChild)
    {
        for(int i = 0; i < objectChild.childCount; i++)
        {
            if (objectChild.GetChild(i).GetComponent<Renderer>() != null)
            {
                objectChild.GetChild(i).GetComponent<Renderer>().sortingOrder = 10;
            }
            changeSortOrder(objectChild.GetChild(i));
        }
    }

    /// <summary>
    /// Enable the usability test.
    /// Get the participant identifier.
    /// </summary>
    public void enableTest()
    {
        usabilityEnabled = true;
        if(File.ReadAllLines(filePathName).Length == 0)
        {
            participantId = 1;
        }
        else
        {
            participantId = Int32.Parse(File.ReadAllLines(filePathName)[File.ReadAllLines(filePathName).Length - 1].Split(',')[0]) + 1;
        }
    }

    /// <summary>
    /// Translate the starting object in the YY axis.
    /// </summary>
    public void translateStartingPosition()
    {
        startingPositionTransparent.transform.position += new Vector3(0f, 1.4f, 0f);
    }

    /// <summary>
    /// Stop the object placement timer.
    /// </summary>
    public void stopTime(Transform go)
    {
        TimeSpan time = DateTime.Now - date;
        Vector3 position = go.transform.position - goList[counter].transform.position;
        float rotation = go.transform.rotation.eulerAngles.y - goList[counter].transform.rotation.eulerAngles.y;
        Vector3 scale = go.transform.localScale - goList[counter].transform.localScale;
        using (StreamWriter sw = File.AppendText(filePathName))
        {
            sw.WriteLine(participantId.ToString() + "," + counter.ToString() + "," + time.ToString() + "," +
                position.x.ToString() + "," + position.y.ToString() + "," + position.z.ToString() + "," + 
                rotation.ToString() + "," + scale.x.ToString());
        }
        date = DateTime.Now;
    }

    /// <summary>
    /// Set the initial time.
    /// </summary>
    public void setTime()
    {
        date = DateTime.Now;
    }
}