using UnityEngine;

using System;
using System.Collections.Generic;

using Face = VDT.FaceRecognition.SDK;

public class EmotionsController 
{
    

    Dictionary<Face.EmotionsEstimator.Emotion, int> emotionsMap = new Dictionary<Face.EmotionsEstimator.Emotion, int>();
    Face.EmotionsEstimator.Emotion currentEmotion = Face.EmotionsEstimator.Emotion.EMOTION_NEUTRAL;

    [SerializeField] float toNeutralConfedence = 0.8f;
    [SerializeField] float toOtherConfedence = 0.5f;
    [SerializeField] float betweenOtherConfedence = 0.8f;

    public EmotionsController()
    {
        foreach (Face.EmotionsEstimator.Emotion e in Enum.GetValues(typeof(Face.EmotionsEstimator.Emotion)))
            emotionsMap.Add(e, UserSettings.GetEmotionMapItem(e));
    }
    public void UpdateEmotion(List<Face.EmotionsEstimator.EmotionConfidence> emotions)
    {
        Face.EmotionsEstimator.Emotion newEmotion = currentEmotion;
        Face.EmotionsEstimator.EmotionConfidence emotion = emotions[0];

        if(emotion.emotion == Face.EmotionsEstimator.Emotion.EMOTION_NEUTRAL)
        {
            if(emotion.confidence > toNeutralConfedence)
                newEmotion = emotion.emotion;
        }
        else
        {
            if (currentEmotion == Face.EmotionsEstimator.Emotion.EMOTION_NEUTRAL && emotion.confidence > toOtherConfedence ||
                currentEmotion != Face.EmotionsEstimator.Emotion.EMOTION_NEUTRAL && emotion.confidence > betweenOtherConfedence)
                    newEmotion = emotion.emotion;
        }

        currentEmotion = newEmotion;
            
        
    }

  

    

    public Face.EmotionsEstimator.Emotion get_current_emotion()
    {
        return currentEmotion;
    }
}