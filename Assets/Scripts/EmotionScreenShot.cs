using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDT.FaceRecognition.SDK;

public class EmotionScreenShot:MonoBehaviour
{
    public EmotionsEstimator.Emotion EmotionOnScreenTaken;
    // public Image screenShot;

    public EmotionScreenShot(EmotionsEstimator.Emotion emotionEnum)
    {
        EmotionOnScreenTaken = emotionEnum;
    }
}
