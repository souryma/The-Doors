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

    public EmotionsController _emotionsController;

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
    
    
    [Space]
    [SerializeField] ComputeShader convertShader;



    [Header("UI")]

    [Space]
    [SerializeField] float minLerp = 16f;
    [SerializeField] float maxLerp = 86f;
    [SerializeField] float lerpFactor = 24f;
    

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
            Debug.Log(e.Message + "/n Check SDK files. Start by launching the Awake scene (FaceSDKLoader). " +
                      "Failed to initialize FaceSDK.");
#else
             Debug.Log(e.Message + "Failed to initialize FaceSDK.");
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

     

        if (!InitSevices())
            return;

        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("Couldn't find the camera." + " Error starting camera");
            return;
        }

        job = new ThreadedJob(Process);

        // firstMaskToggle.isOn = true;

        WebCamDevice device;
        
        string deviceName = UserSettings.DeviceName;

        if (deviceName != null)
        {
            Dictionary<string, WebCamDevice> deviceDict = WebCamTexture.devices.ToDictionary(k => k.name, v => v);
            Debug.Log(deviceDict.Keys);
            device = deviceDict.ContainsKey(deviceName) ? deviceDict[deviceName] : WebCamTexture.devices[0];
        }
        else
            device = WebCamTexture.devices[0];

        

        InitDevice(device);
        InitVisual();
        InitJob();
        _emotionsController = new EmotionsController();
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
                Debug.Log(errorDecoding[message]);
            else
                Debug.Log(message);
        }
    }

    #region INIT

    void InitDevice(WebCamDevice device)
    {
        currentDevice = device;
        // Start streaming from the camera

        webcamTexture = new WebCamTexture(currentDevice.name);
        webcamTexture.Play();

        
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
    

            Face.RawSample sample = samples[i];

            List<Vector3> points = ToListVector3(sample.getLandmarks());
            List<Face.EmotionsEstimator.EmotionConfidence> emotions = emotions_estimator.estimateEmotions(sample);
            _emotionsController.UpdateEmotion(emotions);
     
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
            Debug.Log("Couldn't switch to another camera. Switch camera failed");
    }

    void ChangeCameraDevice(WebCamDevice device)
    {
        job.Wait();

        if (webcamTexture.isPlaying)
            webcamTexture.Stop();

        webcamTexture = null;

        InitDevice(device);
        InitVisual();

        InitJob();
    }

   
  
    #endregion
}