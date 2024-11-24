using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UltraFace;

public sealed class WebcamManager : MonoBehaviour
{
    public static WebcamManager instance;

    #region Private members
    
    
    [Header("New Image Processor")] 
    [SerializeField, Range(0, 1)]private float _threshold = 0.5f;
    [SerializeField]private ResourceSet _resources = null;
    private FaceDetector _detector;
    private bool _face1Detected = false;
    private Detection? _lastFace1Detection;
    
    [SerializeField] private int2 _cameraTextureResolutions = new int2(512, 512);

    private WebCamTexture _webcam1;

    private RenderTexture _face1Texture;
    private RenderTexture _face2Texture;
    
    private const float ErrorMarginX = 0.02f;
    private const float ErrorMarginY = 0.04f;
    
    #endregion

    #region Public members
    public RenderTexture Face1Texture => _face1Texture;

    public RenderTexture Face2Texture => _face2Texture;

    public WebCamTexture Webcam1 => _webcam1;

    public bool Face1Detected => _face1Detected;

    public Detection? LastFace1Detection => _lastFace1Detection;


    #endregion
    
   

    [HideInInspector] public bool isCameraSetup = false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        FaceDetectorInitializer();

     
  
            SetupCameras();
        
    }

  
    private void SetupCameras()
    {
        ConfirmCameraSelection();
        // _camera2Choice.options = _camerasNameList;
    }

    private void FaceDetectorInitializer()
    {
        _detector = new FaceDetector(_resources);
    }


    // Will select the camera names that are defined in the dropdowns
    public void ConfirmCameraSelection()
    {
        if (!PlayerPrefs.HasKey("camera"))
        {
            throw new NullReferenceException("No camera selected");
        }
        _webcam1 = new WebCamTexture(WebCamTexture.devices[PlayerPrefs.GetInt("camera")].name);
        
        _face1Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);
        _face2Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);
        
        _webcam1.Play();
        // _webcam2.Play();

        isCameraSetup = true;
    }

    private void OnDestroy()
    {
        if (ScenesManager.isSceneManagerLoaded)
            ScenesManager.instance.OnStartSceneLoaded -= SetupCameras;
        
        if (_webcam1 != null) Destroy(_webcam1);
        
        if (_face1Texture != null) Destroy(_face1Texture);
        if (_face2Texture != null) Destroy(_face2Texture);
        
        DestroyFaceDetector();
    }

    private void DestroyFaceDetector()
    {
        _detector?.Dispose();
    }
    
    private void FaceDetectorDetectFace()
    {
        _detector.ProcessImage(_webcam1, _threshold);

        _face1Detected = false;
        if (!_detector.Detections.Any())
        {
            _lastFace1Detection = null;
         
            return;
        }
        
       
     
        var centerP1 = new Vector2(0.5f, 0.5f);
        
        Detection? currentDetectionP1 = null;
        
        float smallestDistanceP1 = 1f;

        foreach (var detection in _detector.Detections)
        {
            
            Vector2 centerCurrentDetection = new Vector2(detection.GetCenterX(), detection.GetCenterY());
          
                float distance = Vector2.Distance(centerP1, centerCurrentDetection);
                if (distance < smallestDistanceP1)
                {
                    currentDetectionP1 = detection;
                }
                
        }

        if (_lastFace1Detection != null && currentDetectionP1 != null)
        {
            var centerSavedX = currentDetectionP1.Value.GetCenterX();
            var centerSavedY = currentDetectionP1.Value.GetCenterY();
            var centerLastX = _lastFace1Detection?.GetCenterX();
            var centerLastY = _lastFace1Detection?.GetCenterY();
             
            if ((centerLastX + ErrorMarginX > centerSavedX && centerLastX - ErrorMarginX < centerSavedX)
                && (centerLastY + ErrorMarginY > centerSavedY  && centerLastY - ErrorMarginY < centerSavedY))
            {
                currentDetectionP1 = _lastFace1Detection;
            }
            else
            {
                _lastFace1Detection = currentDetectionP1;
            }
                
        }
        else
        {
            _lastFace1Detection = currentDetectionP1;
        }

        if (currentDetectionP1 != null)
        {
            float myX2 = currentDetectionP1.Value.x2;
            float myX1 = currentDetectionP1.Value.x1; 
            float myY2 = currentDetectionP1.Value.y2; 
            float myY1 = currentDetectionP1.Value.y1; 
            Vector2 scale = new Vector2(myX2 - myX1,
                myY2 - myY1);
            Graphics.Blit(_webcam1, _face1Texture, scale, new Vector2(myX1, 1 - myY2));
            _face1Detected = true;
        }
        else
        {
            _face1Detected = true;
        }
      
    }

    private void LateUpdate()
    {
        if (!isCameraSetup) return;
        if (_webcam1 is not null && _webcam1.didUpdateThisFrame)
        {
            FaceDetectorDetectFace();

        }

        
    }

    public bool DoesCamera1DetectFace()
    {
        return _face1Detected;
    }

    
}