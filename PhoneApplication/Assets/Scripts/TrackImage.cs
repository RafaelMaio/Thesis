// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Experience logic -> Recognizes the markers enabling to take its poses.
// SPECIAL NOTES:
// ===============================

using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TrackImage : MonoBehaviour
{
    /// <summary>
    /// List of markers.
    /// </summary>
    private List<AugmentedImage> _tempAugmentedImages = new List<AugmentedImage>();

    /// <summary>
    /// Connection to the experience menu script.
    /// </summary>
    public ExperienceMenuScript experienceMenuScript;

    /// <summary>
    /// Result button interactable.
    /// </summary>
    bool resultsInteraction = false;

    /// <summary>
    /// Path for the file for saving the camera pose in each frame. 
    /// </summary>
    private string fileCoordsInfoPath;

    /// <summary>
    /// Path for the file for saving the image pose in each frame (if any image is recognized as such at the current frame).
    /// </summary>
    private string fileImgInfoPath;

    /// <summary>
    /// Path for the files for taking the results -> image poses at the button click.
    /// </summary>
    private string fileFinalResultsPath;

    /// <summary>
    /// Counts the number of files for saving the camera pose in each frame. 
    /// </summary>
    private int fileCoordsCont = 0;

    /// <summary>
    /// Counts the number of files for saving the image pose in each frame.
    /// </summary>
    private int fileImgCont = 0;

    /// <summary>
    /// Counts the number of files for taking the results.
    /// </summary>
    private int fileFinalResultsCont = 0;

    /// <summary>
    /// Position offset -> Saved at the first marker.
    /// </summary>
    private Vector3 position_offset;

    /// <summary>
    /// Rotation offset -> Saved at the first marker.
    /// </summary>
    private Vector3 rotation_offset;

    /// <summary>
    /// Initial date.
    /// </summary>
    DateTime initial;

    /// <summary>
    /// Current timestamp.
    /// </summary>
    TimeSpan timestamp;

    /// <summary>
    /// Auxiliary sentence for the result files.
    /// </summary>
    string finalResultString = "";

    /// <summary>
    /// Auxiliary sentence for the result files.
    /// </summary>
    string finalResultString2 = "";

    /// <summary>
    /// Lines for error calculation.
    /// </summary>
    string[] lines;

    /// <summary>
    /// Render texture variables -> deprecated.
    /// </summary>
    public int resWidth = 255;
    public int resHeight = 330;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    void Start()
    {
        /* Get real marker coords */
        fileCoordsInfoPath = Application.persistentDataPath + "/real_markers.txt";

        /* Read file */
        var sr = new StreamReader(fileCoordsInfoPath);
        var fileContents = sr.ReadToEnd();
        sr.Close();
        lines = fileContents.Split('\n');

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

        /* Verify the files that exist and create a new one for the image debuging */
        fileImgInfoPath = Application.persistentDataPath;
        fileImgInfoPath += "/imageDebug" + fileImgCont.ToString() + ".csv";
        while (File.Exists(fileImgInfoPath))
        {
            fileImgCont += 1;
            fileImgInfoPath = Application.persistentDataPath;
            fileImgInfoPath += "/imageDebug" + fileImgCont.ToString() + ".csv";
        }
        File.Create(fileImgInfoPath);

        /* Get the initial time */
        initial = DateTime.Now;

        //Directory.CreateDirectory(Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString());
    }

    /// <summary>
    /// Unity Update function.
    /// </summary>
    void Update()
    {
        //Compute the current timestamp.
        timestamp = DateTime.Now - initial;

        // Sentence organization.
        if (position_offset.x != 0 || position_offset.y != 0 || position_offset.z != 0)
        {
            string toPrint =
                Math.Round(this.GetComponent<Camera>().transform.position.x, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.position.y, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.position.z, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.x, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.y, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.z, 2).ToString() +
                "," + Math.Round(calculate3dDistance(this.GetComponent<Camera>().transform.position, position_offset), 2).ToString() +
                "," + timestamp.ToString();
            File.AppendAllText(fileCoordsInfoPath, toPrint + Environment.NewLine);
            experienceMenuScript.changeDebugText1(toPrint);
        }
        else
        {
            string toPrint =
                Math.Round(this.GetComponent<Camera>().transform.position.x, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.position.y, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.position.z, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.x, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.y, 2).ToString() +
                "," + Math.Round(this.GetComponent<Camera>().transform.rotation.eulerAngles.z, 2).ToString() +
                "," + "0" +
                "," + timestamp.ToString();
            File.AppendAllText(fileCoordsInfoPath, toPrint + Environment.NewLine);
            experienceMenuScript.changeDebugText1(toPrint);
        }

        // Get updated augmented images for this frame.
        Session.GetTrackables<AugmentedImage>(
            _tempAugmentedImages, TrackableQueryFilter.Updated);

        resultsInteraction = false;

        // Verify if any marker in the database is being recognized.
        foreach (var image in _tempAugmentedImages)
        {
            if (image.TrackingState == TrackingState.Tracking && image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking)
            {
                resultsInteraction = true;
                string toPrint =
                    Math.Round(image.CenterPose.position.x, 2).ToString() +
                    "," + Math.Round(image.CenterPose.position.y, 2).ToString() +
                    "," + Math.Round(image.CenterPose.position.z, 2).ToString() +
                    "," + Math.Round(image.CenterPose.rotation.eulerAngles.x, 2).ToString() +
                    "," + Math.Round(image.CenterPose.rotation.eulerAngles.y, 2).ToString() +
                    "," + Math.Round(image.CenterPose.rotation.eulerAngles.z, 2).ToString() +
                    "," + timestamp.ToString();
                File.AppendAllText(fileImgInfoPath, toPrint + Environment.NewLine);
                experienceMenuScript.changeDebugText2(toPrint);
            }
        }
        
        // Change the result button interactivity based on the marker recognition.
        if (resultsInteraction)
        {
            experienceMenuScript.setInteractions(true);
        }
        else
        {
            experienceMenuScript.setInteractions(false);
        }

        // Write the sentence in the file.
        if(finalResultString != "")
        {
            experienceMenuScript.changeDebugTexts(finalResultString, fileFinalResultsCont + 3);
            File.AppendAllText(fileFinalResultsPath, finalResultString + Environment.NewLine);
            File.AppendAllText(fileFinalResultsPath, finalResultString2 + Environment.NewLine);
            fileFinalResultsCont += 1;
            finalResultString = "";
        }

        //Debug code -> deprecated.
        //ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/CameraDebug/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png");
        //ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString() + "/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png");
        //arcoreTry();
        //ScreenCapture_v1();
        //getCameraLowRestImgs();

    }

    /// <summary>
    /// Get the marker pose that is being recognized.
    /// </summary>
    public void getImagePose()
    {
        foreach (var image in _tempAugmentedImages)
        {
            if (image.TrackingState == TrackingState.Tracking && image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking)
            {
                /* Verify the files that exist and create a new one for the final results */
                fileFinalResultsPath = fileCoordsInfoPath.Replace(".csv","_") + fileFinalResultsCont.ToString() + ".csv";
                File.Create(fileFinalResultsPath);

                if (fileFinalResultsCont == 0)
                {
                    finalResultString =
                        Math.Round(image.CenterPose.position.x, 2).ToString() +
                        "," + Math.Round(image.CenterPose.position.y, 2).ToString() +
                        "," + Math.Round(image.CenterPose.position.z, 2).ToString() +
                        "," + Math.Round(image.CenterPose.rotation.eulerAngles.x, 2).ToString() +
                        "," + Math.Round(image.CenterPose.rotation.eulerAngles.y, 2).ToString() +
                        "," + Math.Round(image.CenterPose.rotation.eulerAngles.z, 2).ToString() +
                        "," + "0" +
                        "," + timestamp.ToString();
                    position_offset = image.CenterPose.position;
                    rotation_offset = new Vector3(image.CenterPose.rotation.eulerAngles.x, image.CenterPose.rotation.eulerAngles.y, image.CenterPose.rotation.eulerAngles.z);
                    //rotation_offset = convertTo180(image.CenterPose.rotation.eulerAngles.y);
                    //angle_offset = image.CenterPose.rotation.eulerAngles.y;
                }
                else
                {
                    Vector3 _position = image.CenterPose.position - position_offset;
                    Vector3 _rotation = new Vector3(image.CenterPose.rotation.eulerAngles.x, image.CenterPose.rotation.eulerAngles.y, image.CenterPose.rotation.eulerAngles.z)
                        - rotation_offset;
                    //angle = convertTo180(convertTo180(image.CenterPose.rotation.eulerAngles.y) - angle_offset);
                    //angle = image.CenterPose.rotation.eulerAngles.y - angle_offset;
                    finalResultString =
                        Math.Round(_position.x, 2).ToString() +
                        "," + Math.Round(_position.y, 2).ToString() +
                        "," + Math.Round(_position.z, 2).ToString() +
                        "," + Math.Round(_rotation.x, 2).ToString() +
                        "," + Math.Round(_rotation.y, 2).ToString() +
                        "," + Math.Round(_rotation.z, 2).ToString() +
                        "," + Math.Round(calculate3dDistance(image.CenterPose.position, position_offset), 2).ToString() +
                        "," + timestamp.ToString();

                    string[] real_marker_coord = lines[fileFinalResultsCont].Split(',');

                    finalResultString2 =
                        (Math.Round(_position.x, 2) - double.Parse(real_marker_coord[0])).ToString() +
                        "," + (Math.Round(_position.y, 2) - double.Parse(real_marker_coord[1])).ToString() +
                        "," + (Math.Round(_position.z, 2) - double.Parse(real_marker_coord[2])).ToString() +
                        "," + (Math.Round(_rotation.x, 2) - double.Parse(real_marker_coord[3])).ToString() +
                        "," + (Math.Round(_rotation.y, 2) - double.Parse(real_marker_coord[4])).ToString() +
                        "," + (Math.Round(_rotation.z, 2) - double.Parse(real_marker_coord[5])).ToString() +
                        "," + (Math.Round(calculate3dDistance(image.CenterPose.position, position_offset), 2) - double.Parse(real_marker_coord[6])).ToString() +
                        "," + (timestamp.ToString());
                }
            }
        }
    }

    /* Debug functions. -> Deprecated
    public void functionTimes()
    {
        //yield return new WaitForEndOfFrame();

        TimeSpan[] ts = new TimeSpan[10];
        DateTime t = DateTime.Now;
        Camera camera = GetComponent<Camera>();
        ts[0] = DateTime.Now - t;
        t = DateTime.Now;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        ts[1] = DateTime.Now - t;
        t = DateTime.Now;
        camera.targetTexture = rt;
        ts[2] = DateTime.Now - t;
        t = DateTime.Now;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        ts[3] = DateTime.Now - t;
        t = DateTime.Now;
        camera.Render();
        ts[4] = DateTime.Now - t;
        t = DateTime.Now;
        RenderTexture.active = rt;
        ts[5] = DateTime.Now - t;
        t = DateTime.Now;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        ts[6] = DateTime.Now - t;
        t = DateTime.Now;
        camera.targetTexture = null;
        ts[7] = DateTime.Now - t;
        t = DateTime.Now;
        RenderTexture.active = null; // JC: added to avoid errors
        ts[8] = DateTime.Now - t;
        t = DateTime.Now;
        Destroy(rt);
        ts[9] = DateTime.Now - t;
        TimeSpan bigger = ts[0];
        int biggerNum = 0;
        for(int u = 0; u < 8; u++)
        {
            int comp = TimeSpan.Compare(ts[u + 1], bigger);
            if (comp == 1)
            {
                bigger = ts[u + 1];
                biggerNum = u + 1;
            }
        }
        Debug.Log(biggerNum.ToString() + " : " + bigger.ToString());
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString() + "/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);
    }

    public void getCameraLowRestImgs()
    {
        Camera camera = GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString() + "/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);
    }

    public IEnumerator ScreenCapture_v1()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = new Texture2D(Screen.width, Screen.height);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();
        byte[] bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);
        File.WriteAllBytes(Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString() + "/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png", bytes);
    }

    public IEnumerator ScreenCapture_v2()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = new Texture2D(Screen.width, Screen.height);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();
        byte[] bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);
        File.Create(Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString() + "/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png");
        File.WriteAllBytes(Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString() + "/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png", bytes);
    }

    public IEnumerator ScreenCapture_v3()
    {
        yield return new WaitForEndOfFrame();

        ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString() + "/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png");
    }

    void CamCapture()
    {
        Camera Cam = GetComponent<Camera>();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;

        Cam.Render();

        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(Application.persistentDataPath + "/CameraDebug" + fileImgCont.ToString() + "/" + timestamp.ToString().Replace(":", "_").Replace(".", "_") + ".png", Bytes);
    }

    void arcoreTry()
    {
        using (var image = Frame.CameraImage.AcquireCameraImageBytes())
        {

        }

        //Texture2D _texture = new Texture2D(image.Width, image.Height, TextureFormat.RGBA32, false, false);

            //var bytes = _texture.EncodeToJPG();


    }
    */

    /// <summary>
    /// Convert the angle between 0 and 180 -> Deprecated.
    /// </summary>
    /// <param name="angle">Angle to be converted.</param>
    private double convertTo180(double angle)
    {
        double angle_converted = angle;
        if(angle > 180)
        {
            angle_converted = angle - 360;
        }
        else if(angle < -180)
        {
            angle_converted = angle + 360;
        }
        return angle_converted;
    }

    /* Deprecated
    public void calibrateOffsets()
    {
        foreach (var image in _tempAugmentedImages)
        {
            if (image.TrackingState == TrackingState.Tracking)
            {
                if(image.Name == "marker_blue")
                {
                    offset = new Vector3 (15,0,0) - image.CenterPose.position;
                    angle_offset = 0 - convertTo180(image.CenterPose.rotation.eulerAngles.y);
                }
                else if(image.Name == "marker_yellow")
                {

                }
            }
        }
    }*/

    /// <summary>
    /// Calculate the 3D distance between two points.
    /// </summary>
    /// <param name="g">First point.</param>
    /// <param name="h">Second point.</param>
    public double calculate3dDistance(Vector3 g, Vector3 h)
    {
        return Mathf.Sqrt(Mathf.Pow(g.x - h.x, 2) + Mathf.Pow(g.z - h.z, 2) + Mathf.Pow(g.y - h.y, 2));
    }
}