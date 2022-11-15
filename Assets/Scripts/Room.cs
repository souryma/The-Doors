using System.Collections;
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
        IEnumerator coroutine = MoveDoor(_doorObject.transform);
        StartCoroutine(coroutine);

        _isOpened = true;
    }

    IEnumerator MoveDoor(Transform tf)
    {
        for (int i = 0; i < 100; i++)
        {
            tf.position += tf.right * 0.1f;
        }

        Debug.Log("The door is opened.");

        yield return null;
    }
}