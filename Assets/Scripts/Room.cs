using System.Collections;
using DG.Tweening;
using MoodMe;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    public GetEmotionValue.EmotionEnum EmotionForOpening => _emotionForOpening;
    private GetEmotionValue.EmotionEnum _emotionForOpening;
    private bool _isOpened = false;
    public bool IsOpened => _isOpened;

    private GameObject _doorObject;

    private void Start()
    {
        // Get the door gameobject
        _doorObject = transform.Find("Door").gameObject;

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

    public void OpenDoor()
    {
        _doorObject.transform.DOMoveX(1.8f, 1);
        
        Debug.Log("The door is opened.");

        _isOpened = true;
    }
}