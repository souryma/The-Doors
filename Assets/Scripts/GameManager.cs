using System.Collections;
using System.Collections.Generic;
using MoodMe;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CameraManager camManager;
    [SerializeField] private EmotionsManager emotionsManager;
    [SerializeField] private RoomManager roomManager;
    [Space]
    [SerializeField] private const float EmotionThreshold = 70f;
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (roomManager.CurrentRoom && !roomManager.CurrentRoom.IsOpened && CheckIfEmotionIsAttained())
        {
            roomManager.OpenCurrentDoor();
            MakeACapture();
        }

    }

    private bool CheckIfEmotionIsAttained()
    {
        bool checkEmotion = false;
        switch (roomManager.CurrentRoom.EmotionForOpening)
        {
            case GetEmotionValue.EmotionEnum.Scared:
                if (emotionsManager.Scared >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Angry:
                if (emotionsManager.Angry >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Surprised:
                if (emotionsManager.Surprised >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Sad:
                if (emotionsManager.Sad >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Neutral:
                if (emotionsManager.Neutral >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Disgust:
                if (emotionsManager.Disgust >= EmotionThreshold)
                    checkEmotion = true;
                break;
            case GetEmotionValue.EmotionEnum.Happy:
                if (emotionsManager.Happy >= EmotionThreshold)
                    checkEmotion = true;
                break;
        }

        return checkEmotion;
    }

    private void MakeACapture()
    {
        Texture2D texture2D = Instantiate(camManager.WebcamTexture);
    }
}
