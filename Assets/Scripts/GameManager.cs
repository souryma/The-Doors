
using TMPro;
using UnityEngine;
using VDT.FaceRecognition.SDK;

public class GameManager : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    // Static instance of the GameManager
    private static GameManager _instance;
    // [SerializeField] private CameraManager camManager;
    // [SerializeField] private EmotionsManager emotionsManager;
    [SerializeField] private FaceManager faceManager;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private TextMeshProUGUI _gameOverUi;

    [Space] [SerializeField] private const float EmotionThreshold = 0.70f;

    // The speed of the doors (0 = no movement)
    [SerializeField] [Range(0, 0.1f)] private float _gameSpeed = 0.007f;
    [SerializeField] private Difficulty _gameDifficulty;
    private bool _isVerificationDone = false;

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

    public Difficulty GameDifficulty
    {
        get => _gameDifficulty;
        set => _gameDifficulty = value;
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

        switch (_gameDifficulty)
        {
            case Difficulty.Easy:
                _gameSpeed = 0.007f;
                break;
            case Difficulty.Medium:
                _gameSpeed = 0.02f;
                break;
            case Difficulty.Hard:
                _gameSpeed = 0.05f;
                break;
        }

        RoomManager.OnNewRoom += UpdateGameSpeed;
    }

    private void Update()
    {
        if (_isVerificationDone == false)
        {
            return;
        }
        
        if (roomManager.CurrentRoom && !roomManager.CurrentRoom.IsOpened && CheckIfEmotionIsAttained())
        {
            roomManager.OpenCurrentDoor();
            MakeACapture();
        }
    }

    private void UpdateGameSpeed()
    {
        // Debug.Log("NEW ROOM");
        // _gameSpeed += 0.001f;
    }

    public void StopGame()
    {
        _gameSpeed = 0;
        _gameOverUi.text = "GAME OVER";
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private bool CheckIfEmotionIsAttained()
    {
        bool checkEmotion = false;
        EmotionsEstimator.Emotion current_emotion = faceManager._emotionsController.get_current_emotion();
        if (current_emotion == roomManager.CurrentRoom.EmotionForOpening)
        {
            checkEmotion = true;
        }
        // switch (roomManager.CurrentRoom.EmotionForOpening)
        // {
        //     case GetEmotionValue.EmotionEnum.Scared:
        //         if (emotionsManager.Scared >= EmotionThreshold)
        //             checkEmotion = true;
        //         break;
        //     case GetEmotionValue.EmotionEnum.Angry:
        //         if (emotionsManager.Angry >= EmotionThreshold)
        //             checkEmotion = true;
        //         break;
        //     case GetEmotionValue.EmotionEnum.Surprised:
        //         if (emotionsManager.Surprised >= EmotionThreshold)
        //             checkEmotion = true;
        //         break;
        //     case GetEmotionValue.EmotionEnum.Sad:
        //         if (emotionsManager.Sad >= EmotionThreshold)
        //             checkEmotion = true;
        //         break;
        //     case GetEmotionValue.EmotionEnum.Neutral:
        //         if (emotionsManager.Neutral >= EmotionThreshold)
        //             checkEmotion = true;
        //         break;
        //     case GetEmotionValue.EmotionEnum.Disgust:
        //         if (emotionsManager.Disgust >= EmotionThreshold)
        //             checkEmotion = true;
        //         break;
        //     case GetEmotionValue.EmotionEnum.Happy:
        //         if (emotionsManager.Happy >= EmotionThreshold)
        //             checkEmotion = true;
        //         break;
        // }

        return checkEmotion;
    }

    private void MakeACapture()
    {
        // Texture2D texture2D = Instantiate(camManager.WebcamTexture);
    }
}