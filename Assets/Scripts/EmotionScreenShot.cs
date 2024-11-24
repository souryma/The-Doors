using UnityEngine;

public class EmotionScreenShot:MonoBehaviour
{
    public EmotionManager.EMOTION EmotionOnScreenTaken;
    // public Image screenShot;

    public EmotionScreenShot(EmotionManager.EMOTION emotionEnum)
    {
        EmotionOnScreenTaken = emotionEnum;
    }
}
