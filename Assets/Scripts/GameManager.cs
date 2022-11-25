using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using VDT.FaceRecognition.SDK;

public class GameManager : MonoBehaviour
{
    // Static instance of the GameManager
    private static GameManager _instance;

    // [SerializeField] private CameraManager camManager;
    // [SerializeField] private EmotionsManager emotionsManager;
    [SerializeField] private FaceManager faceManager;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private TextMeshProUGUI _gameOverUi;
    [SerializeField] private GameObject _gameoverObject;

    [Space] [SerializeField] private const float EmotionThreshold = 0.70f;

    // The speed of the doors (0 = no movement)
    [SerializeField] [Range(0, 0.1f)] private float _gameSpeed = 0.007f;

    [SerializeField] private Texture _happyFace;
    [SerializeField] private Texture _neutralFace;
    [SerializeField] private Texture _surprisedFace;
    [SerializeField] private Texture _angryFace;

    private bool _isVerificationDone = false;
    private bool _gameHasStopped = false;
    
    private bool _hasHappy = false;
    private bool _hasAngry = false;
    private bool _hasNeutral = false;
    private bool _hasSurprised = false;
    
    public bool HasHappy => _hasHappy;

    public bool HasAngry => _hasAngry;

    public bool HasNeutral => _hasNeutral;

    public bool HasSurprised => _hasSurprised;

    public static GameManager Instance => _instance;

    public bool IsVerificationDone
    {
        get => _isVerificationDone;
        set => _isVerificationDone = value;
    }

    public float GameSpeed
    {
        get => _gameSpeed;
        set => _gameSpeed = value;
    }

    public Texture HappyFace
    {
        get => _happyFace;
        set => _happyFace = value;
    }

    public Texture NeutralFace
    {
        get => _neutralFace;
        set => _neutralFace = value;
    }

    public Texture SurprisedFace
    {
        get => _surprisedFace;
        set => _surprisedFace = value;
    }

    public Texture AngryFace
    {
        get => _angryFace;
        set => _angryFace = value;
    }

    //public CameraManager CamManager => camManager;

    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _isVerificationDone = false;
        _instance = this;
    }

    

    private void Update()
    {
        if (!AreAllEmotionsReady())
        {
            if (!_hasHappy)
                _hasHappy = GetEmotionFace(EmotionsEstimator.Emotion.EMOTION_HAPPY);
            
            if (!_hasAngry)
                _hasAngry = GetEmotionFace(EmotionsEstimator.Emotion.EMOTION_ANGRY);
    
            if (!_hasNeutral)
                _hasNeutral = GetEmotionFace(EmotionsEstimator.Emotion.EMOTION_NEUTRAL);
    
            if (!_hasSurprised)
                _hasSurprised = GetEmotionFace(EmotionsEstimator.Emotion.EMOTION_SURPRISE);
        }

        if (_isVerificationDone == false || _gameHasStopped)
            return;

        if (roomManager.CurrentRoom && !roomManager.CurrentRoom.IsOpened && CheckIfEmotionIsAttained())
        {
            roomManager.OpenCurrentDoor();
            UpdateGameSpeed();
        }
    }

    private void UpdateGameSpeed()
    {
        // Don't increase speed if game is over
        if (_gameHasStopped)
            return;

        _gameSpeed += 0.01f;
    }

    public void StopGame()
    {
        _gameoverObject.GetComponent<Gameover>().SetGameoverBG(_surprisedFace);
        _gameHasStopped = true;
        _gameSpeed = 0;
        _gameoverObject.SetActive(true);
        _gameoverObject.GetComponent<Gameover>().SetScore(RoomManager.Instance.CurrentRoom.DoorId - 1);
        _gameOverUi.text = "GAME OVER";
    }

    public void RestartGame()
    {
        RoomManager.Instance.RestartRoom();
        _gameHasStopped = false;
        _gameSpeed = 0.007f;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private bool GetEmotionFace(EmotionsEstimator.Emotion emotionToGet)
    {
        bool checkEmotion = false;
        EmotionsEstimator.Emotion current_emotion = faceManager._emotionsController.get_current_emotion();
        if (current_emotion == emotionToGet)
        {
            checkEmotion = true;
            MakeACapture(current_emotion);
        }

        return checkEmotion;
    }

    public bool AreAllEmotionsReady()
    {
        return _hasAngry && _hasHappy && _hasNeutral && _hasSurprised;
    }

    private bool CheckIfEmotionIsAttained()
    {
        bool checkEmotion = false;
        EmotionsEstimator.Emotion current_emotion = faceManager._emotionsController.get_current_emotion();
        if (current_emotion == roomManager.CurrentRoom.EmotionForOpening)
        {
            checkEmotion = true;
        }

        return checkEmotion;
    }

    private void MakeACapture(EmotionsEstimator.Emotion emotion)
    {
        // Get a shot copy of the camera texture into our happy texture
        Texture2D copyTexture = new Texture2D(faceManager.rawImage.texture.width, faceManager.rawImage.texture.height);
        copyTexture.SetPixels(((Texture2D)faceManager.rawImage.texture).GetPixels());
        copyTexture.Apply();
        
        switch (emotion)
        {
            case EmotionsEstimator.Emotion.EMOTION_NEUTRAL:
                _neutralFace = copyTexture;
                break;
            case EmotionsEstimator.Emotion.EMOTION_ANGRY:
                _angryFace = copyTexture;
                break;
            case EmotionsEstimator.Emotion.EMOTION_HAPPY:
                _happyFace = copyTexture;
                break;
            case EmotionsEstimator.Emotion.EMOTION_SURPRISE:
                _surprisedFace = copyTexture;
                break;
        }
    }
}