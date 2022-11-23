using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class BuildApp
{
    static string keystorePath = "C:\\KeyStore\\my-release-key.keystore";
    static string keystorePass = "q2w3e4r";
    static string keyaliasName = "main_key";

    static string androidSDKPath = "C:\\AndroidSDK";
    static string androidNDKPath = "C:\\android\\android-ndk-r16b";

    static void PrepareBuildAndroid()
    {
        //Keys
        PlayerSettings.Android.keystoreName = keystorePath;
        PlayerSettings.Android.keystorePass = keystorePass;
        PlayerSettings.Android.keyaliasName = keyaliasName;
        PlayerSettings.Android.keyaliasPass = keystorePass;

        EditorPrefs.SetString("AndroidSdkRoot", androidSDKPath);
        EditorPrefs.SetString("AndroidNdkRoot", androidNDKPath);
        EditorPrefs.SetString("AndroidNdkRootR16b", androidNDKPath);

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android); // don`t work in banchmode

        FaceSDKLoader.UpdateStreamingAssetsList();
    }

    public static void BuildAndroidAPK()
    {
        PrepareBuildAndroid();

        // Build .akp
        EditorUserBuildSettings.buildAppBundle = false;
        BuildPipeline.BuildPlayer(GetScenes(), Environment.GetCommandLineArgs().Last(), BuildTarget.Android, BuildOptions.None);
    }

    public static void BuildAndroidAAB()
    {
        PrepareBuildAndroid();

        // Build .abb
        EditorUserBuildSettings.buildAppBundle = true;
        BuildPipeline.BuildPlayer(GetScenes(), Environment.GetCommandLineArgs().Last(), BuildTarget.Android, BuildOptions.None);
    }

    public static void BuildStandalone()
    {
        BuildPipeline.BuildPlayer(GetScenes(), Environment.GetCommandLineArgs().Last(), BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [PostProcessBuild(100)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        switch(target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            // case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            // case BuildTarget.StandaloneLinuxUniversal:

                Debug.Log("BuildApp: Removing unnecessary files from the build.");

                // For Windows, the FaceSDK is used at the user-specified path, and you can remove the FaceSDK from the assembly.
                List<string> deletePaths = new List<string>()
                {
                    "FaceSDK",
                    "StreamingAssetsList.json"
                };

                string root = Path.Combine(Path.GetDirectoryName(pathToBuiltProject), Path.GetFileNameWithoutExtension(pathToBuiltProject) + "_Data");
                string streamingAssetsPath = Path.Combine(root, "StreamingAssets");

                foreach (string delPath in deletePaths)
                {
                    string fullDelPath = Path.Combine(streamingAssetsPath, delPath);

                    if (Path.GetExtension(delPath) == "")
                    {
                        if (Directory.Exists(fullDelPath))
                        {
                            Directory.Delete(fullDelPath, true);
                            Debug.Log(string.Format("BuildApp: Directory ({0}) was removed.", fullDelPath));
                        }
                        else
                            Debug.LogError(string.Format("BuildApp: Directory for delete ({0}) not found!", fullDelPath));
                    }
                    else
                    {
                        if (File.Exists(fullDelPath))
                        {
                            File.Delete(fullDelPath);
                            Debug.Log(string.Format("BuildApp: File ({0}) was removed.", fullDelPath));
                        }
                        else
                            Debug.LogError(string.Format("BuildApp: File for delete ({0}) not found!", fullDelPath));
                    }
                }

                break;
        }
    }

    static string[] GetScenes()
    {
        return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
    }
}