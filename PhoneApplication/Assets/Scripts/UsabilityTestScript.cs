// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the usability test.
// SPECIAL NOTES: Only used for the usability tests.
// ===============================

using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UsabilityTestScript : MonoBehaviour
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
    public GameObject arrowTransparentPrefab;
    public GameObject barrierTransparentPrefab;
    public GameObject goalTransparentPrefab;

    /// <summary>
    /// Transparent objects.
    /// </summary>
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
    private string filePathName;

    /// <summary>
    /// Usability test date.
    /// </summary>
    private DateTime date;

    /// <summary>
    /// Number of objects placed in the usability test.
    /// </summary>
    private int counter = 0;

    /// <summary>
    /// Saves if the object was placed.
    /// </summary>
    private bool attachedTrue = false;

    /// <summary>
    /// Marker image.
    /// </summary>
    private AugmentedImage image;

    /// <summary>
    /// Marker database.
    /// </summary>
    private List<AugmentedImage> _tempAugmentedImages = new List<AugmentedImage>();

    /// <summary>
    /// Connection to the configuration window script.
    /// </summary>
    public ConfigureMenuScript configureMenuScript;

    /// <summary>
    /// Unity Update function.
    /// </summary>
    public void Update()
    {
        //Verify if the marker is being tracked.
        Session.GetTrackables<AugmentedImage>(
            _tempAugmentedImages, TrackableQueryFilter.Updated);

        foreach (var img in _tempAugmentedImages)
        {
            if (img.TrackingState == TrackingState.Tracking)
            {
                if(img.Name == "earth" && counter == 0)
                {
                    Debug.Log("x: " + img.CenterPose.rotation.eulerAngles.x.ToString() + ", y: " + img.CenterPose.rotation.eulerAngles.y.ToString() +
                        ", z: " + img.CenterPose.rotation.eulerAngles.z.ToString());
                    if (!attachedTrue)
                    {
                        configureMenuScript.changeAttachInteractable(true);
                        attachedTrue = true;
                        image = img;
                    }
                }
            }
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
            if (numObject == 0)
            {
                arrowTransparent1 = Instantiate(arrowTransparentPrefab, image.CenterPose.position + new Vector3(-0.5f, 0, 0.4f), Quaternion.Euler(new Vector3(0, 270, 0)));
                changeSortOrder(arrowTransparent1.transform);
                goList.Add(arrowTransparent1);
                counter += 1;
                configureMenuScript.changeNumObjectsText(numObject);
                attachedTrue = false;
                configureMenuScript.changeAttachInteractable(false);
            }
            else if (numObject == 1)
            {
                arrowTransparent2 = Instantiate(arrowTransparentPrefab, image.CenterPose.position + new Vector3(1, 0, -0.8f), Quaternion.Euler(new Vector3(0, 240, 0)));
                arrowTransparent2.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                changeSortOrder(arrowTransparent2.transform);
                goList.Add(arrowTransparent2);
                counter += 1;
                configureMenuScript.changeNumObjectsText(numObject);
                attachedTrue = false;
                configureMenuScript.changeAttachInteractable(false);
            }
            else if (numObject == 2)
            {
                goalTransparent = Instantiate(goalTransparentPrefab, image.CenterPose.position + new Vector3(2f, 0, -0.2f), Quaternion.Euler(new Vector3(0, 90, 0)));
                changeSortOrder(goalTransparent.transform);
                goList.Add(goalTransparent);
                counter += 1;
                configureMenuScript.changeNumObjectsText(numObject);
                attachedTrue = false;
                configureMenuScript.changeAttachInteractable(false);
            }
            else if (numObject == 3)
            {
                barrierTransparent = Instantiate(barrierTransparentPrefab, image.CenterPose.position + new Vector3(1.4f, 0, 0.6f), Quaternion.Euler(new Vector3(0, 270, 0)));
                barrierTransparent.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                changeSortOrder(barrierTransparent.transform);
                goList.Add(barrierTransparent);
                counter += 1;
                configureMenuScript.changeNumObjectsText(numObject);
            }
            else
            {
                configureMenuScript.changeNumObjectsText(numObject);
                configureMenuScript.handleShowText(true);
            }
        }
    }

    /// <summary>
    /// Unity OnEnable funciton.
    /// </summary>
    void OnEnable()
    {
        filePathName = Application.persistentDataPath + "/testConfPhone.txt";
        if (usabilityEnabled)
        {
            date = DateTime.Now;
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
    /// Enable the usability test.
    /// Get the participant identifier.
    /// </summary>
    public void enableTest()
    {
        usabilityEnabled = true;
        if (File.ReadAllLines(filePathName).Length == 0)
        {
            participantId = 1;
        }
        else
        {
            participantId = Int32.Parse(File.ReadAllLines(filePathName)[File.ReadAllLines(filePathName).Length - 1].Split(',')[0]) + 1;
        }
    }

    /// <summary>
    /// Change the transparent object sorting order, so they appear behind the opaque ones.
    /// </summary>
    /// <param name="objectChild">Object piece transform component.</param>
    private void changeSortOrder(Transform objectChild)
    {
        for (int i = 0; i < objectChild.childCount; i++)
        {
            if (objectChild.GetChild(i).GetComponent<Renderer>() != null)
            {
                objectChild.GetChild(i).GetComponent<Renderer>().sortingOrder = 10;
            }
            changeSortOrder(objectChild.GetChild(i));
        }
    }

    /// <summary>
    /// Stop the object placement timer.
    /// </summary>
    public void stopTime(Transform go)
    {
        TimeSpan time = DateTime.Now - date;
        Vector3 position = go.transform.position - goList[counter - 1].transform.position;
        float rotation = go.transform.rotation.eulerAngles.y - goList[counter - 1].transform.rotation.eulerAngles.y;
        Vector3 scale = go.transform.localScale - goList[counter - 1].transform.localScale;
        using (StreamWriter sw = File.AppendText(filePathName))
        {
            sw.WriteLine(participantId.ToString() + "," + counter.ToString() + "," + time.ToString() + "," +
                position.x.ToString() + "," + position.y.ToString() + "," + position.z.ToString() + "," +
                rotation.ToString() + "," + scale.x.ToString());
        }
        date = DateTime.Now;
    }

    /// <summary>
    /// Stop the timer for verifying the anchor quality.
    /// </summary>
    public void stopTimeForQuality()
    {
        TimeSpan time = DateTime.Now - date;
        using (StreamWriter sw = File.AppendText(filePathName))
        {
            sw.WriteLine(participantId.ToString() + "," + counter.ToString() + "," + time.ToString());
        }
    }

    /// <summary>
    /// Set the initial time.
    /// </summary>
    public void setTime()
    {
        date = DateTime.Now;
    }
}