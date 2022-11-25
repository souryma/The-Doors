using DG.Tweening;
using TMPro;
using UnityEngine;
using VDT.FaceRecognition.SDK;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    private EmotionsEstimator.Emotion _emotionForOpening;
    private bool _isOpened = false;
    private GameObject _doorObject;
    private GameObject _leftCurtainsObject;
    private GameObject _rightCurtainsObject;
    private GameObject _middleCurtainsObject;
    [SerializeField] private TextMeshPro _emotionRoomText;
    [SerializeField] private GameObject _emotionImage;

    public bool IsOpened => _isOpened;
    public EmotionsEstimator.Emotion EmotionForOpening=> _emotionForOpening;
    public int DoorId { get; set; }


    private void Start()
    {
        // Get the door gameobject
        _doorObject = transform.Find("Door").gameObject;
        
        // Get the curtain gameobject
        GameObject curtains = transform.Find("Curtain").gameObject;
        _leftCurtainsObject = curtains.transform.Find("LeftCurtain").gameObject;
        _rightCurtainsObject = curtains.transform.Find("RightCurtain").gameObject;
        _middleCurtainsObject = curtains.transform.Find("MiddleCurtain").gameObject;
        
        // Get emotion text UI
        _emotionRoomText = transform.Find("EmotionText").GetComponent<TextMeshPro>();

        _emotionImage = transform.Find("EmotionImage").gameObject;

        // Give a random emotion to te door
        int emotionIndex = Random.Range(0, 4);
        _emotionForOpening = (EmotionsEstimator.Emotion) emotionIndex;

        
        SetEmotionText();
        SetEmotionImage();
    }

    private void SetEmotionImage()
    {
        Texture text = new Texture2D(300, 300);
        switch (_emotionForOpening)
        {
            case EmotionsEstimator.Emotion.EMOTION_NEUTRAL:
                text = GameManager.Instance.NeutralFace;
                break;
            case EmotionsEstimator.Emotion.EMOTION_ANGRY:
                text = GameManager.Instance.AngryFace;
                break;
            case EmotionsEstimator.Emotion.EMOTION_HAPPY:
                text = GameManager.Instance.HappyFace;
                break; 
            case EmotionsEstimator.Emotion.EMOTION_SURPRISE:
                text = GameManager.Instance.SurprisedFace;
                break;
        }

        _emotionImage.GetComponent<MeshRenderer>().material.mainTexture = text;
    }

    private void SetEmotionText()
    {
        string text = "";
        switch (_emotionForOpening)
        {
            case EmotionsEstimator.Emotion.EMOTION_NEUTRAL:
                text = "Neutral";
                break;
            case EmotionsEstimator.Emotion.EMOTION_ANGRY:
                text = "Angry !";
                break;
            case EmotionsEstimator.Emotion.EMOTION_HAPPY:
                text = "Happy :)";
                break; 
            case EmotionsEstimator.Emotion.EMOTION_SURPRISE:
                text = "Surprised :0";
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
        _leftCurtainsObject.transform.DOScaleY(20f, 1);
        _rightCurtainsObject.transform.DOScaleY(-20f, 1);
        _middleCurtainsObject.transform.DOScaleZ(1f, 2f);
        
        AudioManager.instance.Play("CurtainOpen");

        // Debug.Log("The door is opened.");

        _isOpened = true;
    }
}