using UnityEngine;

using Face = VDT.FaceRecognition.SDK;

/// <summary>
/// Wrapper for PlayerPrefs (use this class to add new values to maintain the interface and convenience)
/// </summary>
public static class UserSettings
{
    public static float LerpFactor
    {
        get
        {
            return PlayerPrefs.GetFloat("LerpFactor", 0.5f);
        }
        set
        {
            PlayerPrefs.SetFloat("LerpFactor", value);
            PlayerPrefs.Save();
        }
    }

    public static bool ShownEmotionToolTip
    {
        get
        {
            return PlayerPrefs.GetInt("FirstEmotionalToolTip", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("FirstEmotionalToolTip", value ? 1 : 0);
        }
    }


    public static string DeviceName
    {
        get
        {
            return PlayerPrefs.GetString("DeviceName", null);
        }
        set
        {
            PlayerPrefs.SetString("DeviceName", value);
            PlayerPrefs.Save();
        }
    }

    public static int GetEmotionMapItem(Face.EmotionsEstimator.Emotion emotion)
    {
        return PlayerPrefs.GetInt("MixFaceMask:" + emotion.ToString(), (int)emotion);
    }

    public static void SetEmotionMapItem(Face.EmotionsEstimator.Emotion emotion, int item)
    {
        PlayerPrefs.SetInt("MixFaceMask:" + emotion.ToString(), item);
        PlayerPrefs.Save();
    }
   

    public static float GetOpacity(int faceID, float defaultVal)
    {
        return PlayerPrefs.GetFloat("FaceOpacity_" + faceID, defaultVal);
    }

    public static void SetOpacity(int faceID, float val)
    {
        PlayerPrefs.SetFloat("FaceOpacity_" + faceID, val);
        PlayerPrefs.Save();
    }


    public static int GetResolution(string deviceName)
    {
        return PlayerPrefs.GetInt("Resolution_" + deviceName, -1);
    }

    public static void SetResolution(string deviceName, int item)
    {
        PlayerPrefs.SetInt("Resolution_" + deviceName, item);
        PlayerPrefs.Save();
    }
}
