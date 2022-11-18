using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Face = VDT.FaceRecognition.SDK;


public class FaceManager : MonoBehaviour
{
    const string config = "fda_tracker_capturer_mesh.xml";
    const string emotionConfig = "emotions_estimator.xml";

    Face.Capturer capturer;
    Face.EmotionsEstimator emotions_estimator;

    ThreadedJob job;
    Face.RawImage ri_frame;
    List<Face.RawSample> samples;

    [Header("View")]
    [SerializeField] RawImage rawImage;
    [SerializeField] AspectRatioFitter aspectRatioFitter;
    // [SerializeField] new Camera camera;

    [Header("Camera device options")]
    [SerializeField] int targetWidth = 800;
    [SerializeField] int targetHeigh = 600;

    WebCamDevice currentDevice;
    WebCamTexture webcamTexture;

    int kernelIndex;
    uint x, y, z;
    RenderTexture convertedTexture;
    Texture2D rgbTexture;

    Rect rect;
    Vector3 meshScale;

    bool firstInit = false;

    [Header("Face object options")]
    // [SerializeField] List<GameObject> facePrefabs;
    // [SerializeField] Transform spawnTransform;

    // 
    [Space]
    // [SerializeField] ComputeShader convertShader;

    List<FaceController> faceControllers = new List<FaceController>();
    int currentFaceID;

    [Header("UI")]
    // [SerializeField] Toggle firstMaskToggle;
    [SerializeField] MessageBox messageBox;

    [Space]
    // [SerializeField] Slider opacitySlider;
    // [SerializeField] Text opacityVal;
    // [SerializeField] float opacity = 0.41f;

    [Space]
    [SerializeField] Slider lerpSlider;
    [SerializeField] Text lerpVal;

    [Space]
    [SerializeField] float minLerp = 16f;
    [SerializeField] float maxLerp = 86f;
    [SerializeField] float lerpFactor = 24f;

    [Space]
    [SerializeField] Dropdown resolutionDrop;

    [Space]
    [SerializeField] Dropdown deviceDrop;

    Dictionary<string, string> errorDecoding = new Dictionary<string, string>
    {
        { "ERROR - Could not find specified video device", "Deny access to the camera. Check the permission to access the camera in the settings." },
        { "Could not connect pins - RenderStream()", "The camera cannot be used because it is being used by another process.\nCheck the running applications or update the drivers on the device." },
        { "Could not pause pControl", "The camera cannot be used because it is being used by another process.\nCheck the running applications or update the drivers on the device." }
    };

    bool InitSevices()
    {
        // Initializing the Facerec Service and create Capturer
        try
        {
            Face.FacerecService service = Face.FacerecService.createService(FaceSDKLoader.FaceRecPath, FaceSDKLoader.LicensePath);
            Face.FacerecService.Config capturerConfig = new Face.FacerecService.Config(config);
            capturer = service.createCapturer(capturerConfig);

            emotions_estimator = service.createEmotionsEstimator(emotionConfig);

            return true;
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            messageBox.ShowMessage(e.Message + "/n Check SDK files. Start by launching the Awake scene (FaceSDKLoader).",
                "Failed to initialize FaceSDK.");
#else
            messageBox.ShowMessage(e.Message, "Failed to initialize FaceSDK.");
#endif

            return false;
        }
    }

    void Process()
    {
        samples = capturer.capture(ri_frame);
    }


    void Start()
    {
        Application.logMessageReceived += HandleLog;

        if (!SystemInfo.supportsComputeShaders)
        {
#if UNITY_EDITOR && !UNITY_STANDALONE
            messageBox.ShowMessage("Compute shaders are not supported for the Android platform in the editor. " +
                "Switch the platform to Standalone (this is not relevant for the assembled project).", "Compute shaders error");
#else
            messageBox.ShowMessage("This device does not support Compute Shaders", "Compute shaders error");
#endif
            return;
        }

        if (!InitSevices())
            return;

        if (WebCamTexture.devices.Length == 0)
        {
            messageBox.ShowMessage("Couldn't find the camera.", "Error starting camera");
            return;
        }

        job = new ThreadedJob(Process);

        firstMaskToggle.isOn = true;

        WebCamDevice device;

#if UNITY_STANDALONE || UNITY_EDITOR
        string deviceName = UserSettings.DeviceName;

        if (deviceName != null)
        {
            Dictionary<string, WebCamDevice> deviceDict = WebCamTexture.devices.ToDictionary(k => k.name, v => v);
            device = deviceDict.ContainsKey(deviceName) ? deviceDict[deviceName] : WebCamTexture.devices[0];
        }
        else
            device = WebCamTexture.devices[0];

#else
        device = WebCamTexture.devices[0];

        for (int i = 0; i < WebCamTexture.devices.Length; i++)
            if (WebCamTexture.devices[i].isFrontFacing)
            {
                device = WebCamTexture.devices[i];
                break;
            }

#endif

        InitDevice(device);
        InitUI();
        InitVisual();
        InitJob();

        firstInit = true;
    }

    void OnDestroy()
    {
        job.Abort();
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
        {
            if (errorDecoding.ContainsKey(message))
                messageBox.ShowMessage(errorDecoding[message], "System error", true, MessageBox.ButtonAction.Close);
            else
                messageBox.ShowMessage(message, "Unknown error", true, MessageBox.ButtonAction.Close);
        }
    }

    #region INIT

    void InitDevice(WebCamDevice device)
    {
        currentDevice = device;
        // Start streaming from the camera
#if UNITY_STANDALONE || UNITY_EDITOR

        webcamTexture = new WebCamTexture(currentDevice.name);
        webcamTexture.Play();

#elif UNITY_ANDROID && !UNITY_EDITOR

        int resolutionItem = UserSettings.GetResolution(currentDevice.name);

        if (resolutionItem != -1)
        {
            Resolution resolution = currentDevice.availableResolutions[resolutionItem];
            webcamTexture = new WebCamTexture(currentDevice.name, resolution.width, resolution.height);
        }
        else
            webcamTexture = new WebCamTexture(currentDevice.name, targetWidth, targetHeigh);

        webcamTexture.Play();
#endif
    }

    void InitUI()
    {
        // Preparing parameters and UI

        float lerp = UserSettings.LerpFactor;
        lerpFactor = Mathf.Lerp(minLerp, maxLerp, lerp);
        lerpSlider.value = lerp;

#if UNITY_STANDALONE || UNITY_EDITOR

        resolutionDrop.interactable = false;
        deviceDrop.interactable = true;

        deviceDrop.ClearOptions();
        List<string> devices = (from device in WebCamTexture.devices select device.name).ToList();
        deviceDrop.AddOptions(devices);

        for (int i = 0; i < WebCamTexture.devices.Length; i++)
            if (currentDevice.name == WebCamTexture.devices[i].name)
            {
                deviceDrop.value = i;
                break;
            }

#elif UNITY_ANDROID && !UNITY_EDITOR

        deviceDrop.interactable = false;

        // Fill resolutions in resolutionDrop

        resolutionDrop.ClearOptions();
        List<string> options = (from res in currentDevice.availableResolutions select res.width + "x" + res.height).ToList();
        resolutionDrop.AddOptions(options);

        for (int i = 0; i < currentDevice.availableResolutions.Length; i++)
            if (currentDevice.availableResolutions[i].width == webcamTexture.width && currentDevice.availableResolutions[i].height == webcamTexture.height)
            {
                resolutionDrop.value = i;
                break;
            }

#endif
    }

    void InitVisual()
    {
        // Switch width and height if use portrait camera orientation (actual for Android devices)
        bool portraitCam = Application.platform == RuntimePlatform.Android;

        int width = portraitCam ? webcamTexture.height : webcamTexture.width;
        int height = portraitCam ? webcamTexture.width : webcamTexture.height;

        // Initializing RenderTexture to output the result of ComputeShader execution

        convertedTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        convertedTexture.enableRandomWrite = true;
        convertedTexture.Create();

        rawImage.texture = convertedTexture;

        rect = new Rect(0, 0, convertedTexture.width, convertedTexture.height);
        aspectRatioFitter.aspectRatio = (float)convertedTexture.width / convertedTexture.height;

        // Preparing ComputeShader
        // Compute Shader rotates the image and flips it horizontally to match the FaceSDK data format

        string kernelName = portraitCam ? "Portaint" : "Landscape";
        kernelName += currentDevice.isFrontFacing ? "FrontCamera" : "BackwardCamera";

        kernelIndex = convertShader.FindKernel(kernelName);
        convertShader.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);

        convertShader.SetTexture(kernelIndex, "ImageInput", webcamTexture);
        convertShader.SetTexture(kernelIndex, "Result", convertedTexture);

        convertShader.SetInt("textureWidth", webcamTexture.width);
        convertShader.SetInt("textureHeigth", webcamTexture.height);

        // Initializing Texture2D, the result from convertedTexture will be rewritten to It, 
        // since it is impossible to get an array of bytes from RenderTexture

        rgbTexture = new Texture2D(convertedTexture.width, convertedTexture.height, TextureFormat.RGB24, false);

        // Due to the different aspect ratio of the screen and camera, you need to adjust the Mesh scale and spawn position

        Rect riRect = rawImage.rectTransform.rect;

        meshScale = new Vector3(riRect.width, riRect.height, (riRect.width + riRect.height) / 2);
        spawnTransform.localPosition = new Vector3(riRect.x + riRect.width / 2, riRect.y + riRect.height / 2, spawnTransform.localPosition.z);
    }

    void InitJob()
    {
        // Call to image processing in the Compute Shader (GPU)
        convertShader.Dispatch(kernelIndex, convertedTexture.width / (int)x, convertedTexture.height / (int)y, (int)z);

        // Transferring the result to rgb Texture
        RenderTexture.active = convertedTexture;
        rgbTexture.ReadPixels(rect, 0, 0, false);

        // Image preparation and processing
        ri_frame = new Face.RawImage(rgbTexture.width, rgbTexture.height, Face.RawImage.Format.FORMAT_RGB, rgbTexture.GetRawTextureData());

        job.Start();
    }

    #endregion

    FaceController CreateFaceController()
    {
        GameObject newFace = Instantiate(facePrefabs[currentFaceID], spawnTransform);
        newFace.transform.localScale = meshScale;

        FaceController faceController = newFace.GetComponent<FaceController>();

        faceController.Opacity = opacity;
        faceController.LerpFactor = lerpFactor;

        return faceController;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            job.Abort();

            capturer.Dispose();
            emotions_estimator.Dispose();

            if (webcamTexture.isPlaying)
                webcamTexture.Stop();

            Application.Quit();
        }

        if (webcamTexture == null || !webcamTexture.isPlaying || capturer == null || !job.IsDone)
            return;

        if (samples.Count > 0) // Sync updating RGB and mask from FaceSDK
        {
            // Update texture on GPU and apply to rawImage
            rgbTexture.Apply();
            rawImage.texture = rgbTexture;
        }
        else // Fast updating
            rawImage.texture = convertedTexture;

        for (int i = 0; i < samples.Count; i++)
        {
            if (i >= faceControllers.Count)
                faceControllers.Add(CreateFaceController());

            Face.RawSample sample = samples[i];

            List<Vector3> points = ToListVector3(sample.getLandmarks());

            if (faceControllers[i].EmotionVisible)
                faceControllers[i].UpdateFace(points, true, emotions_estimator.estimateEmotions(sample));
            else
                faceControllers[i].UpdateFace(points, true);
        }

        for (int i = samples.Count; i < faceControllers.Count; i++)
        {
            Destroy(faceControllers[i].gameObject);
            faceControllers.Remove(faceControllers[i]);
        }

        InitJob();
    }

    List<Vector3> ToListVector3(List<Face.Point> points)
    {
        List<Vector3> outPoints = new List<Vector3>(points.Count);

        int width = rgbTexture.width;
        int height = rgbTexture.height;
        int depth = (rgbTexture.width + rgbTexture.height) / 2;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 uVector = new Vector3((width - points[i].x) / width - 0.5f, (height - points[i].y) / height - 0.5f, points[i].z / depth);
            outPoints.Add(uVector);
        }
        return outPoints;
    }

    #region UI controller

    public void SetAvatar(int id)
    {
        if (!firstInit)
            return;

        currentFaceID = id;

        float opacity = UserSettings.GetOpacity(currentFaceID, 0.47f);
        opacitySlider.value = opacity;

        List<FaceController> lastFaceControllers = faceControllers;
        faceControllers = new List<FaceController>();

        foreach (FaceController fc in lastFaceControllers)
        {
            FaceController faceController = CreateFaceController();
            faceControllers.Add(faceController);

            faceController.UpdateFace(fc.Points, true, fc.Emotions);

            Destroy(fc.gameObject);
        }
    }

    public void SetResolution(int item)
    {
        if (!firstInit)
            return;

        job.Wait();

        if (webcamTexture.isPlaying)
            webcamTexture.Stop();

        webcamTexture = null;

        UserSettings.SetResolution(currentDevice.name, item);
        Resolution res = currentDevice.availableResolutions[item];

        webcamTexture = new WebCamTexture(currentDevice.name, res.width, res.height);
        webcamTexture.Play();

        InitVisual();
        InitJob();
    }

    public void SetCameraDevice(int deviceID)
    {
        if (!firstInit)
            return;

        WebCamDevice device = WebCamTexture.devices[deviceID];
        UserSettings.DeviceName = device.name;

        ChangeCameraDevice(device);
    }

    public void SwitchCamera()
    {
        if (!firstInit)
            return;

        WebCamDevice device = currentDevice;

        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            if (WebCamTexture.devices[i].isFrontFacing != currentDevice.isFrontFacing)
            {
                device = WebCamTexture.devices[i];
                break;
            }
        }

        if (device.name != currentDevice.name)
            ChangeCameraDevice(device);
        else
            messageBox.ShowMessage("Couldn't switch to another camera.", "Switch camera failed", buttonAction: MessageBox.ButtonAction.Close);
    }

    void ChangeCameraDevice(WebCamDevice device)
    {
        job.Wait();

        if (webcamTexture.isPlaying)
            webcamTexture.Stop();

        webcamTexture = null;

        InitDevice(device);
        InitUI();
        InitVisual();

        InitJob();
    }

    public void SetOpacity(float val)
    {
        if (!firstInit)
            return;

        UserSettings.SetOpacity(currentFaceID, val);

        opacity = val;
        opacityVal.text = opacity.ToString("F2");

        foreach (FaceController fc in faceControllers)
            fc.Opacity = opacity;
    }

    public void SetLerpFactor(float val)
    {
        if (!firstInit)
            return;

        UserSettings.LerpFactor = val;

        float realVal = 1 - val;

        lerpVal.text = val.ToString("F2");
        lerpFactor = Mathf.Lerp(minLerp, maxLerp, realVal);

        foreach (FaceController fc in faceControllers)
            fc.LerpFactor = lerpFactor;
    }

    #endregion
}