using DG.Tweening;
using MoodMe;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    private GetEmotionValue.EmotionEnum _emotionForOpening;
    private bool _isOpened = false;
    private GameObject _doorObject;

    public bool IsOpened => _isOpened;
    public GetEmotionValue.EmotionEnum EmotionForOpening => _emotionForOpening;
    public int DoorId { get; set; }


    private void Start()
    {
        // Get the door gameobject
        _doorObject = transform.Find("Door").gameObject;

        // Give a random emotion to te door
        int emotion = Random.Range(0, 3);
        switch (emotion)
        {
            case 0:
                _emotionForOpening = GetEmotionValue.EmotionEnum.Neutral;
                break;
            case 1:
                _emotionForOpening = GetEmotionValue.EmotionEnum.Sad;
                break;
            case 2:
                _emotionForOpening = GetEmotionValue.EmotionEnum.Surprised;
                break;
        }
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