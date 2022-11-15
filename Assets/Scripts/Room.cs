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

        _emotionForOpening = (GetEmotionValue.EmotionEnum) Random.Range(0, 3);
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