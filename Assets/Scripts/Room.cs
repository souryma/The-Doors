using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    private EmotionManager.EMOTION _emotionForOpening;
    private bool _isOpened = false;
    private GameObject _doorObject;
    private GameObject _leftCurtainsObject;
    private GameObject _rightCurtainsObject;
    private GameObject _blind000;
    [SerializeField] private TextMeshPro _emotionRoomText;
    [SerializeField] private GameObject _emotionImage;

    private int _musicNumber = 0;
    public bool IsOpened => _isOpened;
    public EmotionManager.EMOTION EmotionForOpening=> _emotionForOpening;
    public int DoorId { get; set; }


    private void Start()
    {
        // Get the door gameobject
        _doorObject = transform.Find("Door").gameObject;
        
        // Get the curtain gameobject
        _leftCurtainsObject = transform.Find("LeftCurtain").gameObject;
        _rightCurtainsObject = transform.Find("RightCurtain").gameObject;
        _blind000 = transform.Find("blind").gameObject;
        
        // Get emotion text UI
        // _emotionRoomText = transform.Find("EmotionText").GetComponent<TextMeshPro>();

        _emotionImage = transform.Find("EmotionImage").gameObject;
        int emotionCount = Enum.GetValues(typeof(EmotionManager.EMOTION)).Length;
        // Give a random emotion to te door
        int emotionIndex = Random.Range(0,emotionCount);
        _emotionForOpening = (EmotionManager.EMOTION) emotionIndex;

        
        //SetEmotionText();
        SetEmotionImage();
    }

    private async void SetEmotionImage()
    {
         Texture2D texture2D = await GameManager.Instance.GetFaceEmotion(_emotionForOpening);
         _emotionImage.GetComponent<MeshRenderer>().material.mainTexture = texture2D;

    }
    

    private void SetEmotionText()
    {
        string text = "";
        switch (_emotionForOpening)
        {
            case EmotionManager.EMOTION.Neutral:
                text = "Neutral";
                break;
            case EmotionManager.EMOTION.Anger:
                text = "Angry !";
                break;
            case EmotionManager.EMOTION.Happy:
                text = "Happy :)";
                break; 
            case EmotionManager.EMOTION.Surprise:
                text = "Surprised :0";
                break;
            case EmotionManager.EMOTION.Sadness:
                text = "Sad :'(";
                break;
        }
        _emotionRoomText.text = text;
    }

    public void DoorHasBeenTouched()
    {
        Debug.Log("The door "+ DoorId+" has been touched !");
        _doorObject.GetComponent<Renderer>().material.color = Color.black;

        GameManager.Instance.StopGame();
    }

    public void OpenDoor()
    {
        // Open door
        // _doorObject.transform.DOMoveX(1.8f, 1);
        _doorObject.SetActive(false);
        
        // Open curtain
        _leftCurtainsObject.transform.DOScaleX(-0.04f, 1);
        _rightCurtainsObject.transform.DOScaleX(-0.04f, 1);
        _blind000.transform.DOScaleZ(7f, 2);
        
        AudioManager.instance.Play("curtainOpen", 1f);

        // Debug.Log("The door is opened.");

        _isOpened = true;
    }

    public string getRoomMusic()
    {
        if (_musicNumber == 0)
        {
            _musicNumber = Random.Range(1, 4);
        }
        
        switch (this._emotionForOpening)
        {
            case EmotionManager.EMOTION.Anger:
                return "musicangry" + _musicNumber;
            case EmotionManager.EMOTION.Happy:
                return "musichappy" + _musicNumber;
            case EmotionManager.EMOTION.Surprise:
                return "musicsurprised" + _musicNumber;
            case EmotionManager.EMOTION.Neutral:
                return "musicneutral" + _musicNumber;
            case EmotionManager.EMOTION.Sadness:
                return "musicsad" + _musicNumber;
                
        }

        return "";
    }
}