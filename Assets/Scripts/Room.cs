using DG.Tweening;
using MoodMe;
using TMPro;
using UnityEngine;
using VDT.FaceRecognition.SDK;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    private EmotionsEstimator.Emotion _emotionForOpening;
    private bool _isOpened = false;
    private GameObject _doorObject;
    [SerializeField] private TextMeshPro _emotionRoomText;

    public bool IsOpened => _isOpened;
    public EmotionsEstimator.Emotion EmotionForOpening=> _emotionForOpening;
    public int DoorId { get; set; }


    private void Start()
    {
        // Get the door gameobject
        _doorObject = transform.Find("Door").gameObject;
        
        // Get emotion text UI
        _emotionRoomText = transform.Find("EmotionText").GetComponent<TextMeshPro>();

        // Give a random emotion to te door
        int emotionIndex = Random.Range(0, 4);
        _emotionForOpening = (EmotionsEstimator.Emotion) emotionIndex;
        // switch (emotion)
        // {
        //     case 0:
        //         _emotionForOpening = GetEmotionValue.EmotionEnum.Neutral;
        //         break;
        //     case 1:
        //         _emotionForOpening = GetEmotionValue.EmotionEnum.Sad;
        //         break;
        //     case 2:
        //         _emotionForOpening = GetEmotionValue.EmotionEnum.Surprised;
        //         break;
        // }
        
        SetEmotionText();
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
        _doorObject.transform.DOMoveX(1.8f, 1);

        Debug.Log("The door is opened.");

        _isOpened = true;
    }
}