using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    // Static instance of the GameManager
    private static GameManager _instance;
    
    [FormerlySerializedAs("faceManager")] [SerializeField] public EmotionManager emotionManager;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private TextMeshProUGUI _gameOverUi;
    [SerializeField] private Gameover _gameoverObject;
    [SerializeField] private CameraVerification _cameraVerif;
    [SerializeField] private GameObject _transitionCurtains;

    [Space] [SerializeField] private const float EmotionThreshold = 0.70f;

    // The speed of the doors (0 = no movement)
    [SerializeField] [Range(0, 0.1f)] private float _gameSpeed = 1f;

    [SerializeField] private Texture _happyFace;
    [SerializeField] private Texture _neutralFace;
    [SerializeField] private Texture _surprisedFace;
    [SerializeField] private Texture _angryFace;
    [SerializeField] private Texture _sadFace;

    private ImageLoaderSaver _imageLoaderSaver;
    
  

    private GameObject _leftTransitionCurtains;
    private GameObject _rightTransitionCurtains;
    private GameObject _middleTransitionCurtains;

    public bool GameHasStopped => _gameHasStopped;

    private bool _isVerificationDone = false;
    private bool _gameHasStopped = true;
    public bool isGameOver = false;
    // public bool IsGameOver => _isGameOver;
    private bool _gameIsStarted = false;

    private bool _hasHappy = false;
    private bool _hasAngry = false;
    private bool _hasNeutral = false;
    private bool _hasSurprised = false;
    private bool _hasSad = false;

    public bool HasHappy => _hasHappy;

    public bool HasAngry => _hasAngry;
    public bool HasSad => _hasSad;

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
    
    public Texture SadFace
    {
        get => _sadFace;
        set => _sadFace = value;
    }

    public Task<Texture2D> GetFaceEmotion(EmotionManager.EMOTION emotion)
    {
        return _imageLoaderSaver.LoadPictureFromGallery(emotion);
    }
    //public CameraManager CamManager => camManager;

    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _imageLoaderSaver = gameObject.AddComponent<ImageLoaderSaver>();
        _leftTransitionCurtains = _transitionCurtains.transform.Find("LeftCurtain").gameObject;
        _rightTransitionCurtains = _transitionCurtains.transform.Find("RightCurtain").gameObject;
        _middleTransitionCurtains = _transitionCurtains.transform.Find("MiddleCurtain").gameObject;

        _isVerificationDone = false;
        _instance = this;
        AudioManager.instance.Play("crowdmumbling");
    }


    private void Update()
    {
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
        if (!_gameHasStopped && roomManager.CurrentRoom && !roomManager.CurrentRoom.IsOpened && 
            CheckIfEmotionIsAttained(roomManager.CurrentRoom.EmotionForOpening))
        {
            MakeACapture(roomManager.CurrentRoom.EmotionForOpening);
            roomManager.OpenCurrentDoor();
            UpdateGameSpeed();
        }
        //
        // if (_isGameOver)
        // {
        //     if (CheckIfEmotionIsAttained(EmotionManager.EMOTION.EMOTION_ANGRY))
        //     {
        //         _gameoverObject.OnTryAgain();
        //         _isGameOver = false;
        //     }
        //     if (CheckIfEmotionIsAttained(EmotionManager.EMOTION.EMOTION_SURPRISE))
        //     {
        //         _gameoverObject.OnRestartGame();
        //         _isGameOver = false;
        //     }
        // }
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
        AudioManager.instance.PlayAfterTime(RoomManager.Instance.CurrentRoom.getRoomMusic(), 6f);
        StartCoroutine(StartGameAfterTime());
    }

    public void StopGame()
    {
        StartCoroutine(SwitchToGameOverSearch());
        _gameHasStopped = true;
        // _gameoverObject.gameObject.SetActive(true);
        _gameoverObject.ShowGameOver();
       
        _gameSpeed = 0;
        // _gameoverObject.SetActive(true);
        _gameoverObject.SetScore(RoomManager.Instance.CurrentRoom.DoorId - 1);
        _gameOverUi.text = "GAME OVER";
        AudioManager.instance.GetSound("crowdmumbling").source.DOFade(1f, 4f);
        for (int i = 1; i < 4; i++)
        {
            AudioManager.instance.StopSound("musicangry" + i, 0f);
            AudioManager.instance.StopSound("musicneutral" + i, 0f);
            AudioManager.instance.StopSound("musicsurprised" + i, 0f);
            AudioManager.instance.StopSound("musichappy" + i, 0f);
        }
        
        AudioManager.instance.Play("cheers", 6f, 1f);
    }

    private IEnumerator SwitchToGameOverSearch()
    {
        yield return new WaitForSeconds(10f);
        isGameOver = true;
       
    }

    public void RestartGame()
    {
        for (int i = 1; i < 4; i++)
        {
            AudioManager.instance.StopSound("musicangry" + i, 0f);
            AudioManager.instance.StopSound("musicneutral" + i, 0f);
            AudioManager.instance.StopSound("musicsurprised" + i, 0f);
            AudioManager.instance.StopSound("musichappy" + i, 0f);
        }
        // _gameIsStarted = false;
        RoomManager.Instance.RestartRoom();
        // _gameHasStopped = false;
        // _gameSpeed = 0.007f;
        LaunchGame();
    }

    public void RestartGameAndVerif()
    {
        CloseCurtains();
        for (int i = 1; i < 4; i++)
        {
            AudioManager.instance.StopSound("musicangry" + i, 0f);
            AudioManager.instance.StopSound("musicneutral" + i, 0f);
            AudioManager.instance.StopSound("musicsurprised" + i, 0f);
            AudioManager.instance.StopSound("musichappy" + i, 0f);
        }
        RoomManager.Instance.RestartRoom();
        _neutralFace =_angryFace = _happyFace= _surprisedFace = null;
        _hasAngry = _hasHappy = _hasNeutral = _hasSurprised = false;
        _cameraVerif.ResetVerification();
        _gameIsStarted = false;
        
        // _isGameOver = false

    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void OpenCurtains()
    {
        _transitionCurtains.SetActive(true);

        // Open curtain
        _leftTransitionCurtains.transform.DOScaleY(13f, 1);
        _rightTransitionCurtains.transform.DOScaleY(-13f, 1);
        _middleTransitionCurtains.transform.DOScaleZ(1f, 1f);

        StartCoroutine("HideCurtains");
    }

    public void CloseCurtains()
    {
        _transitionCurtains.SetActive(true);

        // Close curtain
        _leftTransitionCurtains.transform.DOScaleY(32f, 1);
        _rightTransitionCurtains.transform.DOScaleY(-32f, 1);
        _middleTransitionCurtains.transform.DOScaleZ(53.6f, 1f);

        StartCoroutine("OpenCurtainsAfterTime");
    }

    public IEnumerator OpenCurtainsAfterTime()
    {
        yield return new WaitForSeconds(1f);
        OpenCurtains();
    }
    private IEnumerator HideCurtains()
    {
        yield return new WaitForSeconds(1);

        _transitionCurtains.SetActive(false);
    }

    public bool CheckForEmotion(EmotionManager.EMOTION emotion)
    {
        return emotion == emotionManager.GetPlayer1Emotion();
    }

    public void GetEmotionsFace(EmotionManager.EMOTION emotion)
    {
        EmotionManager.EMOTION current_emotion = emotionManager.GetPlayer1Emotion();

        if (current_emotion != emotion)
            return;

        if (!_hasAngry && EmotionManager.EMOTION.Anger == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasAngry = true;
        }
        else if (!_hasHappy && EmotionManager.EMOTION.Happy == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasHappy = true;
        }
        else if (!_hasNeutral && EmotionManager.EMOTION.Neutral == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasNeutral = true;
        }
        else if (!_hasSurprised && EmotionManager.EMOTION.Surprise == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasSurprised = true;
        }
        else if (!_hasSad && EmotionManager.EMOTION.Sadness == current_emotion)
        {
            MakeACapture(current_emotion);
            _hasSad = true;
        }
    }

    public bool AreAllEmotionsReady()
    {
        return _hasAngry && _hasHappy && _hasNeutral && _hasSurprised;
    }

    private bool CheckIfEmotionIsAttained(EmotionManager.EMOTION emotionToTest)
    {
      
        EmotionManager.EMOTION current_emotion = emotionManager.GetPlayer1Emotion();

        return current_emotion == emotionToTest;
    }
    


    private IEnumerator StartGameAfterTime()
    {
        yield return new WaitForSeconds(7f);
        _gameHasStopped = false;
        _gameSpeed = 1f;
    }

    private void MakeACapture(EmotionManager.EMOTION emotion)
    {
        // Get a shot copy of the camera texture into our happy texture
        Texture2D copyTexture = new Texture2D(WebcamManager.instance.Webcam1.width, WebcamManager.instance.Webcam1.height);
        // copyTexture.SetPixels(((Texture2D)faceManager.rawImage.texture).GetPixels());
        copyTexture.SetPixels(WebcamManager.instance.Webcam1.GetPixels());
        copyTexture.Apply();

        _imageLoaderSaver.SavePictureToGallery(copyTexture, emotion);
        switch (emotion)
        {
            case EmotionManager.EMOTION.Neutral:
                _neutralFace = copyTexture;
                break;
            case EmotionManager.EMOTION.Anger:
                _angryFace = copyTexture;
                break;
            case EmotionManager.EMOTION.Happy:
                _happyFace = copyTexture;
                break;
            case EmotionManager.EMOTION.Surprise:
                _surprisedFace = copyTexture;
                break;
            case EmotionManager.EMOTION.Sadness:
                _sadFace = copyTexture;
                break;
        }
    }
}
