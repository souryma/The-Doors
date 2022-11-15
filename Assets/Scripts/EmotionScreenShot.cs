using System.Collections;
using System.Collections.Generic;
using MoodMe;
using UnityEngine;

public class EmotionScreenShot:MonoBehaviour
{
    public GetEmotionValue.EmotionEnum EmotionOnScreenTaken;
    // public Image screenShot;

    public EmotionScreenShot(GetEmotionValue.EmotionEnum emotionEnum)
    {
        EmotionOnScreenTaken = emotionEnum;
    }
}
