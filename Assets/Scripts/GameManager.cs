using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using VDT.FaceRecognition.SDK;

public class GameManager : MonoBehaviour
{
    // Static instance of the GameManager
    private static GameManager _instance;

    // [SerializeField] private CameraManager camManager;
    // [SerializeField] private EmotionsManager emotionsManager;
    [SerializeField] public FaceManager faceManager;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private TextMeshProUGUI _gameOverUi;
    [SerializeField] private GameObject _gameoverObject;
    // [SerializeField] private CameraVerification _camera;

    [Space] [SerializeField] private const float EmotionThreshold = 0.70f;

    // The speed of the doors (0 = no movement)
    [SerializeField] [Range(0, 0.1f)] private float _gameSpeed = 1f;

    [SerializeField] private Texture _happyFace;
    [SerializeField] private Texture _neutralFace;
    [SerializeField] private Texture _surprisedFace;
    [SerializeField] private Texture _angryFace;
    
    
  

    public bool GameHasStopped => _gameHasStopped;

    private float timerForVerif = 0f;
    
    private bool _isVerificationDone = false;
    private bool _gameHasStopped = true;
    private bool _gameIsStarted = false;

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
        AudioManager.instance.Play("crowdmumbling");
    }



    private void Update()
    {
        
        timerForVerif += timerForVerif <= 1 ? Time.deltaTime : 0;
        if (!AreAllEmotionsReady() && timerForVerif > 1)
        {
            GetEmotionsFace();
        }

        if (_isVerificationDone == false)
        {
            return;
        }
        if (!_gameIsStarted)
        {
            LaunchGame();
        }

        int playingCough = Random.Range(0, 5000);
        if (playingCough < 5)
        {
            string coughType = playingCough > 2 ? "cough_male" : "cough_female";
            AudioManager.instance.Play(coughType);
        }
        if (!_gameHasStopped && roomManager.CurrentRoom && !roomManager.CurrentRoom.IsOpened && CheckIfEmotionIsAttained())
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
        
        _gameSpeed += 0.3f;
    }

    private void LaunchGame()
    {
        _gameIsStarted = true;
        AudioManager.instance.GetSound("crowdmumbling").source.DOFade(0.2f, 4f);
        AudioManager.instance.StopSound("cheers", 1f);

        AudioManager.instance.Play("threestrikes");
        // Debug.Log(RoomManager.Instance.CurrentRoom.getRoomMusic());
        AudioManager.instance.PlayAfterTime(RoomManager.Instance.CurrentRoom.getRoomMusic(), 7f);
        StartCoroutine(StartGameAfterTime());
    }

    public void StopGame()
    {
        _gameoverObject.GetComponent<Gameover>().SetGameoverBG(_surprisedFace);
        _gameHasStopped = true;
        _gameSpeed = 0;
        _gameoverObject.SetActive(true);
        _gameoverObject.GetComponent<Gameover>().SetScore(RoomManager.Instance.CurrentRoom.DoorId - 1);
        _gameOverUi.text = "GAME OVER";
        AudioManager.instance.GetSound("crowdmumbling").source.DOFade(1f, 4f);
        for (int i = 1; i < 4; i++)
        {
            AudioManager.instance.StopSound("musicangry" + i, 0f);
            AudioManager.instance.StopSound("musicneutral" + i, 0f);
            AudioManager.instance.StopSound("musicsurprised" + i, 0f);
            AudioManager.instance.StopSound("musichappy" + i, 0f);
        }
        
        AudioManager.instance.Play("cheers", 5f, 1f);
    }

    public void RestartGame()
    {
        // _gameIsStarted = false;
        RoomManager.Instance.RestartRoom();
        // _gameHasStopped = false;
        // _gameSpeed = 0.007f;
        LaunchGame();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void GetEmotionsFace()
    {
        EmotionsEstimator.Emotion current_emotion = faceManager._emotionsController.get_current_emotion();
        if (!_hasAngry && EmotionsEstimator.Emotion.EMOTION_ANGRY == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasAngry = true;
        }
        else if(!_hasHappy && EmotionsEstimator.Emotion.EMOTION_HAPPY == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasHappy = true;
        }
        else if(!_hasNeutral && EmotionsEstimator.Emotion.EMOTION_NEUTRAL == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasNeutral = true;
        }
        else if(!_hasSurprised && EmotionsEstimator.Emotion.EMOTION_SURPRISE == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasSurprised = true;
        }

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

   
    private IEnumerator StartGameAfterTime()
    {
        yield return new WaitForSeconds(7f);
        _gameHasStopped = false;
        _gameSpeed = 1f;
        
    }
    private void MakeACapture(EmotionsEstimator.Emotion emotion)
    {
        // Get a shot copy of the camera texture into our happy texture
        Texture2D copyTexture = new Texture2D(faceManager.rawImage.texture.width, faceManager.rawImage.texture.height);
        // copyTexture.SetPixels(((Texture2D)faceManager.rawImage.texture).GetPixels());
        copyTexture.SetPixels(faceManager.webcamTexture.GetPixels());
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