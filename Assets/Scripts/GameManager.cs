using MoodMe;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static instance of the GameManager
    private static GameManager _instance;
    [SerializeField] private CameraManager camManager;
    [SerializeField] private EmotionsManager emotionsManager;
    [SerializeField] private RoomManager roomManager;
    [Space]
    [SerializeField] private const float EmotionThreshold = 0.70f;
    // The speed of the doors (0 = no movement)
    [SerializeField] [Range(0, 0.1f)] private float _gameSpeed = 0.007f;

    public static GameManager Instance => _instance;
    public float GameSpeed
    {
        get => _gameSpeed;
        set => _gameSpeed = value;
    }

    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Update()
    {
        if (roomManager.CurrentRoom && !roomManager.CurrentRoom.IsOpened && CheckIfEmotionIsAttained())
        {
            roomManager.OpenCurrentDoor();
            MakeACapture();
        }

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
        switch (roomManager.CurrentRoom.EmotionForOpening)
        {
            case GetEmotionValue.EmotionEnum.Scared:
                if (emotionsManager.Scared >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Angry:
                if (emotionsManager.Angry >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Surprised:
                if (emotionsManager.Surprised >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Sad:
                if (emotionsManager.Sad >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Neutral:
                if (emotionsManager.Neutral >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Disgust:
                if (emotionsManager.Disgust >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Happy:
                if (emotionsManager.Happy >= EmotionThreshold)
                    checkEmotion = true;
                break;
        }

        return checkEmotion;
    }

    private void MakeACapture()
    {
        Texture2D texture2D = Instantiate(camManager.WebcamTexture);
    }
}
