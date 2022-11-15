using System.Collections;
using System.Collections.Generic;
using MoodMe;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GetEmotionValue.EmotionEnum EmotionForOpening => _emotionForOpening;
    private GetEmotionValue.EmotionEnum _emotionForOpening;
    private bool _isOpened = false;
    public bool IsOpened => _isOpened;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
