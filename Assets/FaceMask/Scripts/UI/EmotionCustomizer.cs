using UnityEngine;
using UnityEngine.UI;

using Face = VDT.FaceRecognition.SDK;

public class EmotionCustomizer : MonoBehaviour
{
    [SerializeField] Dropdown neutralDropDown;
    [SerializeField] Dropdown happyDropDown;
    [SerializeField] Dropdown angryDropDown;
    [SerializeField] Dropdown surpriseDropDown;

    public delegate void EmotionValueChange(Face.EmotionsEstimator.Emotion emotion, int newVal);
    public static event EmotionValueChange OnEmotionValueChange;

    void Awake()
    {
        neutralDropDown.value = UserSettings.GetEmotionMapItem(Face.EmotionsEstimator.Emotion.EMOTION_NEUTRAL);
        happyDropDown.value = UserSettings.GetEmotionMapItem(Face.EmotionsEstimator.Emotion.EMOTION_HAPPY);
        angryDropDown.value = UserSettings.GetEmotionMapItem(Face.EmotionsEstimator.Emotion.EMOTION_ANGRY);
        surpriseDropDown.value = UserSettings.GetEmotionMapItem(Face.EmotionsEstimator.Emotion.EMOTION_SURPRISE);

    }

    void SetEmotionItem(Face.EmotionsEstimator.Emotion emotion, int item)
    {
        UserSettings.SetEmotionMapItem(emotion, item);

        if(OnEmotionValueChange != null)
            OnEmotionValueChange(emotion, item);
    }

    public void SetNeutral(int item)
    {
        SetEmotionItem(Face.EmotionsEstimator.Emotion.EMOTION_NEUTRAL, item);
    }

    public void SetHappy(int item)
    {
        SetEmotionItem(Face.EmotionsEstimator.Emotion.EMOTION_HAPPY, item);
    }

    public void SetAngry(int item)
    {
        SetEmotionItem(Face.EmotionsEstimator.Emotion.EMOTION_ANGRY, item);
    }

    public void SetSurprise(int item)
    {
        SetEmotionItem(Face.EmotionsEstimator.Emotion.EMOTION_SURPRISE, item);
    }
}