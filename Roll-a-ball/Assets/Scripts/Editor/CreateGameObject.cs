// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Transform the game into an Android Augmented Reality game using ARCore.
// SPECIAL NOTES:
// ===============================

using UnityEditor;
using UnityEngine;
using GoogleARCore;
using UnityEngine.SpatialTracking;
using UnityEngine.SceneManagement;

public class CreateGameObject : Editor
{

    // <summary>
    // New option on Unity Editor Menu for converting the game.
    // </summary>
    [MenuItem("ARCore game library/Create game for ARCore")]
    static void CreateARCoreHandler()
    {
        //Create a new GameObject to be the ARCore camera.
        GameObject arcoreHandler = new GameObject("arcoreHandler");

        //Turn the active camera off
        Camera camera = FindObjectOfType<Camera>();
        if (camera != null)
        {
            camera.enabled = false;
        }

        //Add Camera and ARCore components (Camera, TrackedPoseDriver, ARCoreSession, ARCoreBackgroudRenderer)
        arcoreHandler.AddComponent<Camera>();
        arcoreHandler.AddComponent<TrackedPoseDriver>();
        arcoreHandler.GetComponent<TrackedPoseDriver>().SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.ColorCamera);
        Object session_instance = ScriptableObject.CreateInstance("ARCoreSessionConfig");
        ARCoreSessionConfig session_config = session_instance as ARCoreSessionConfig;
        Object camera_filter_instance = ScriptableObject.CreateInstance("ARCoreCameraConfigFilter");
        ARCoreCameraConfigFilter camera_filter = camera_filter_instance as ARCoreCameraConfigFilter;
        arcoreHandler.AddComponent<ARCoreSession>();
        arcoreHandler.GetComponent<ARCoreSession>().SessionConfig = session_config;
        arcoreHandler.GetComponent<ARCoreSession>().CameraConfigFilter = camera_filter;
        Material arbackgroundmaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/GoogleARCore/SDK/Materials/ARBackground.mat");
        arcoreHandler.AddComponent<ARCoreBackgroundRenderer>().BackgroundMaterial = arbackgroundmaterial;

        // Attach the player (with "player" tag) to the android camera.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            arcoreHandler.transform.position = player.transform.position;
            arcoreHandler.transform.rotation = player.transform.rotation;
            player.transform.SetParent(arcoreHandler.transform);
        }
        arcoreHandler.AddComponent<ARCoreAttach>();

        //Adapt the player settings to support ARcore and Android.
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }
        if (PlayerSettings.applicationIdentifier == "com.Company.ProductName")
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com."+ SceneManager.GetActiveScene().name);
        }

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.ARCoreEnabled = true;
    }
}
