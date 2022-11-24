using TMPro;
using UnityEngine;
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
    private bool _isVerificationDone = false;
    private bool _gameHasStopped = false;
    private bool _musicIsStarted = false;

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
        if (_isVerificationDone == false || _gameHasStopped)
        {
            return;
        }
        if (!_musicIsStarted)
        {
            AudioManager.instance.Play(RoomManager.Instance.CurrentRoom.getRoomMusic());
            _musicIsStarted = true;
        }
        
        if (roomManager.CurrentRoom && !roomManager.CurrentRoom.IsOpened && CheckIfEmotionIsAttained())
        {
            roomManager.OpenCurrentDoor();
            MakeACapture();
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