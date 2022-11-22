using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID || UNITY_EDITOR
using UnityEngine.Android;
using UnityEngine.Networking;
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
using Windows = System.Windows.Forms;
#endif

/// <summary>
/// For Android, this class copies the contents of StreamingAssets to the application's system directory.
/// 
/// For Windows, you need to set the environment variable. 
/// This class will require you to specify the path to the FaceSDK on your computer and set the environment variable.
/// </summary>
public class FaceSDKLoader : MonoBehaviour
{
    [SerializeField]private MessageBox messageBox;
    [SerializeField]private string sceneToLoad;

#if UNITY_EDITOR

    [Serializable]
    public class StreamingAssetsInfo
    {
        public List<string> listFiles = new List<string>();
    }

    static string sourceListFiles = "StreamingAssetsList.json";

    readonly List<string> permissonsList = new List<string>
    {
        Permission.ExternalStorageWrite,
        Permission.ExternalStorageRead,
        Permission.Camera
    };

    bool permissionSucces = false;

#endif

    bool sdkLoaded = false;
    bool animationComplate = false;

    public static string AppDataPath
    {
        get
        {
#if UNITY_ANDROID
            return Application.persistentDataPath;
#else
            return Application.streamingAssetsPath;
#endif
        }
    }

    public static string SDKPath
    {
        get
        {
#if UNITY_ANDROID
            return Path.Combine(Application.persistentDataPath, "FaceSDK");
#else
            if (SDKEnvironmentPath != null)
                return Directory.GetParent(SDKEnvironmentPath).FullName;
            else
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
        }
    }

    public static string FaceRecPath
    {
        get
        {
            return Path.Combine(SDKPath, "conf", "facerec");
        }
    }

    public static string LicensePath
    {
        get
        {
            return Path.Combine(SDKPath, "license");
        }
    }

    void Start()
    {
#if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StartCoroutine(WaitPermissions());
#else
        InitializeSDKEnvironment();
        StartCoroutine(WaitAnimationComplate());
#endif
    }

#if UNITY_STANDALONE || UNITY_EDITOR

    static string SDKEnvironmentPath
    {
        get
        {
            string path_env = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);

            if (path_env == null)
                return null;

            if (IsSDKBinFolder(path_env))
                return path_env;

            // Case with a set of multiple values
            if (path_env.Contains(";"))
            {
                string[] paths = path_env.Split(';');

                foreach (string path in paths)
                    if (IsSDKBinFolder(path))
                        return path;
            }

            return null;
        }
        set
        {
            string path_env = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);

            // Case with a set of multiple values
            if (path_env != null)
                Environment.SetEnvironmentVariable("PATH", string.Format("{0};{1}", path_env, value), EnvironmentVariableTarget.User);
            else
                Environment.SetEnvironmentVariable("PATH", value, EnvironmentVariableTarget.User);
        }
    }

    static bool IsSDKBinFolder(string sdkBinPath)
    {
        return File.Exists(Path.Combine(sdkBinPath, "facerec.dll")) && File.Exists(Path.Combine(sdkBinPath, "tensorflow.dll"));
    }

    void InitializeSDKEnvironment()
    {
        // For Windows, you need to set the environment variable.
        while (!sdkLoaded)
        {
            if (SDKEnvironmentPath == null)
            {
                Windows.FolderBrowserDialog ofd = new Windows.FolderBrowserDialog
                {
                    Description = "Select FaceSDK folder",
                    ShowNewFolderButton = false
                };

                Windows.DialogResult dialogResult = ofd.ShowDialog();

                if (dialogResult == Windows.DialogResult.OK)
                {
                    // Checking for the presence of the main directories in the specified FaceSDK location
                    if (IsSDKBinFolder(Path.Combine(ofd.SelectedPath, "bin")))
                    {
                        // Setting the environment variable
                        SDKEnvironmentPath = Path.Combine(ofd.SelectedPath, "bin");
                        sdkLoaded = true;

                        Windows.MessageBox.Show("To continue working, the application must be restarted.",
                            "Initializing FaceSDK",
                            Windows.MessageBoxButtons.OK,
                            Windows.MessageBoxIcon.Information);

                        Application.Quit();
                    }
                    else
                    {
                        Windows.MessageBox.Show("FaceSDK not found, specify the correct path.",
                            "Initializing FaceSDK",
                            Windows.MessageBoxButtons.OK,
                            Windows.MessageBoxIcon.Warning);
                    }
                }
                else if (dialogResult == Windows.DialogResult.Cancel)
                {
                    Application.Quit();
                    return;
                }
            }
            else
                sdkLoaded = true;
        }
    }

#endif

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

#if UNITY_ANDROID || UNITY_EDITOR

    IEnumerator WaitPermissions()
    {
        while (!permissionSucces)
        {
            permissionSucces = true;

            // Android requires you to force permission requests
            foreach (string permission in permissonsList)
                if (!Permission.HasUserAuthorizedPermission(permission))
                {
                    Permission.RequestUserPermission(permission);
                    yield return null;
                }

            foreach (string permission in permissonsList)
                permissionSucces &= Permission.HasUserAuthorizedPermission(permission);
        }

        StartCoroutine(CopyStreamingAssets());
    }

    IEnumerator CopyStreamingAssets()
    {
        string listFilesPath = Path.Combine(Application.streamingAssetsPath, sourceListFiles);
        UnityWebRequest listFilesUWR = new UnityWebRequest(listFilesPath) { downloadHandler = new DownloadHandlerBuffer() };

        yield return listFilesUWR.SendWebRequest();

        StreamingAssetsInfo streamingAssetsInfo = JsonUtility.FromJson<StreamingAssetsInfo>(listFilesUWR.downloadHandler.text);

        bool succes = true;

        foreach (string filePath in streamingAssetsInfo.listFiles)
        {
            string currentFile = Path.Combine(Application.streamingAssetsPath, filePath);
            string targetFile = Path.Combine(Application.persistentDataPath, filePath);

            Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

            // Since Unity stores all the contents of StreamingAssets inside the. apk for the Android platform, 
            // you need to extract the SDK to the system directory of the app when you launch it. 
            // If you delete the app from the device, the SDK will also be deleted

            UnityWebRequest uwr = new UnityWebRequest(currentFile) { downloadHandler = new DownloadHandlerBuffer() };

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                succes = false;
                messageBox.ShowMessage(uwr.error);
                break;
            }
            else
                File.WriteAllBytes(targetFile, uwr.downloadHandler.data);

        }

        sdkLoaded = succes;

        if (sdkLoaded)
        {
            Debug.Log("Face SKD copied in to: " + Application.persistentDataPath);
            StartCoroutine(WaitAnimationComplate());
        }
    }

#endif

    IEnumerator WaitAnimationComplate()
    {
        while (!animationComplate)
            yield return null;

        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
    }

#region Animation events (see Animation time line)

    public void OnAnimationComplate()
    {
        animationComplate = true;
    }

#endregion

#if UNITY_EDITOR

    // Methods to use from context menu or editor (FaceSDKLoader_Editor)
    [ContextMenu("Fill SDK files list")]
    public void FileFilesList()
    {
        UpdateStreamingAssetsList();
    }

    public static List<string> StreamingAssetsFiles
    {
        get
        {
            string listFilesPath = Path.Combine(Application.streamingAssetsPath, sourceListFiles);

            if (File.Exists(listFilesPath))
            {
                string jsonData = File.ReadAllText(listFilesPath);
                StreamingAssetsInfo streamingAssetsInfo = JsonUtility.FromJson<StreamingAssetsInfo>(jsonData);
                return streamingAssetsInfo.listFiles;
            }
            else
                return null;
        }
    }

    /// <summary>
    /// Call this method when building the project automatically
    /// </summary>
    public static void UpdateStreamingAssetsList()
    {
        StreamingAssetsInfo streamingAssetsInfo = new StreamingAssetsInfo();
        FillListFiles(Application.streamingAssetsPath, ref streamingAssetsInfo.listFiles);

        string jsonData = JsonUtility.ToJson(streamingAssetsInfo);
        string jsonPath = Path.Combine(Application.streamingAssetsPath, sourceListFiles);

        File.WriteAllText(jsonPath, jsonData);

        string saFilesPath = "";
        foreach (string filePath in streamingAssetsInfo.listFiles)
            saFilesPath += filePath + "\n";

        Debug.Log(string.Format("The list of StreamingAssets has been updated. Files ({0}) in the list:\n{1}", streamingAssetsInfo.listFiles.Count, saFilesPath));
    }

    static void FillListFiles(string currentDir, ref List<string> streamingAssetsList)
    {
        foreach (string dir in Directory.GetDirectories(currentDir))
            FillListFiles(dir, ref streamingAssetsList);

        foreach (string filePath in Directory.GetFiles(currentDir))
            if (Path.GetExtension(filePath) != ".meta" && !filePath.Contains(sourceListFiles))
            {
                string subPath = filePath.Substring(Application.streamingAssetsPath.Length + 1);
                string replaceSlash = subPath.Replace(Path.DirectorySeparatorChar, '/');

                streamingAssetsList.Add(replaceSlash);
            }
    }

#endif
}
