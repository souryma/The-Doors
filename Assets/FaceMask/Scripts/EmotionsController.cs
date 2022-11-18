using UnityEngine;

using System;
using System.Collections.Generic;

using Face = VDT.FaceRecognition.SDK;

public class EmotionsController : MonoBehaviour
{
    [SerializeField] List<Texture2D> emotionsTexture;
    [SerializeField] float speedTransform = 4f;
    [SerializeField] MeshRenderer meshRenderer;

    Dictionary<Face.EmotionsEstimator.Emotion, int> emotionsMap = new Dictionary<Face.EmotionsEstimator.Emotion, int>();
    Face.EmotionsEstimator.Emotion currentEmotion = Face.EmotionsEstimator.Emotion.EMOTION_NEUTRAL;
    bool textureTransfom = false;
    float blend = 0;

    [SerializeField] float toNeutralConfedence = 0.8f;
    [SerializeField] float toOtherConfedence = 0.5f;
    [SerializeField] float betweenOtherConfedence = 0.8f;

    bool waitTransform = false;
    [SerializeField] float waitTransformTime = 0.5f;
    float lastTick = 0;

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

        if (currentEmotion != newEmotion && !waitTransform)
        {
            lastTick = Time.time;
            waitTransform = true;
        }

        if (waitTransform && Time.time - lastTick > waitTransformTime && !textureTransfom)
        {
            blend = 0;

            int currentItem = emotionsMap[currentEmotion];
            int newItem = emotionsMap[newEmotion];

            meshRenderer.material.SetTexture("_MainTex", emotionsTexture[currentItem]);
            meshRenderer.material.SetTexture("_NewTex", emotionsTexture[newItem]);
            meshRenderer.material.SetFloat("_Blend", 0);

            currentEmotion = newEmotion;
            textureTransfom = true;
            waitTransform = false;
        }
    }

    void Awake()
    {
        foreach (Face.EmotionsEstimator.Emotion e in Enum.GetValues(typeof(Face.EmotionsEstimator.Emotion)))
            emotionsMap.Add(e, UserSettings.GetEmotionMapItem(e));

        EmotionCustomizer.OnEmotionValueChange += EmotionCustomizer_OnEmotionValueChange;
    }

    void OnDestroy()
    {
        EmotionCustomizer.OnEmotionValueChange -= EmotionCustomizer_OnEmotionValueChange;
    }

    void EmotionCustomizer_OnEmotionValueChange(Face.EmotionsEstimator.Emotion emotion, int newVal)
    {
        emotionsMap[emotion] = newVal;

        int currentItem = emotionsMap[currentEmotion];
        meshRenderer.material.SetTexture("_MainTex", emotionsTexture[currentItem]);
        meshRenderer.material.SetTexture("_NewTex", emotionsTexture[currentItem]);
    }

    void Update()
    {
        if(textureTransfom)
        {
            if (blend < 1)
            {
                blend = Mathf.MoveTowards(blend, 1, Time.deltaTime * speedTransform);
                meshRenderer.material.SetFloat("_Blend", blend);
            }
            else
                textureTransfom = false;
        }
    }
}