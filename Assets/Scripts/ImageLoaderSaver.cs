using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using VDT.FaceRecognition.SDK;
using Random = UnityEngine.Random;

public class ImageLoaderSaver:MonoBehaviour
{
    private Dictionary<EmotionsEstimator.Emotion,List<Texture2D>> _faces = new ();


    private void Start()
    {
      
        foreach (EmotionsEstimator.Emotion emotion in Enum.GetValues(typeof(EmotionsEstimator.Emotion)))
        {
            string pathToFolder = GetPicturesFolderPath(emotion);
            if(!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }
            string[] files = Directory.GetFiles(pathToFolder, "*.png");
            // Debug.Log(string.Join(",", files));
            // Debug.Log(emotion);
            List<Texture2D> texture2Ds = new List<Texture2D>();
          
            foreach (string file in files)
            {
                Texture2D texture = new Texture2D(1920,1080);
                byte[] bytes = File.ReadAllBytes(file);
                ImageConversion.LoadImage(texture, bytes);
                texture2Ds.Add(texture);
            }
            _faces.Add(emotion, texture2Ds);
        }
    }

    public void SavePictureToGallery( Texture2D texture2D, EmotionsEstimator.Emotion pictureEmotion )
    {
        string filename = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-f");
        string pathToFolder = GetPicturesFolderPath(pictureEmotion);
        byte[] bytes = ImageConversion.EncodeToPNG(texture2D);
       
        
        string path = Path.Combine(pathToFolder, filename );
        if( !filename.EndsWith( ".png" ) )
            path += ".png";

        // Debug.Log( "Saving to: " + path );
        StartCoroutine(SaveImage(path, bytes));
       
        _faces[pictureEmotion].Add(texture2D);

    }

    public IEnumerator SaveImage(string path, byte[] bytes)
    {
        File.WriteAllBytes( path, bytes );
        yield return 0;
    }
    public Texture2D LoadPictureFromGallery(EmotionsEstimator.Emotion emotion)
    {


        return _faces[emotion][Random.Range(0, _faces[emotion].Count)];
        // byte[] bytes = File.ReadAllBytes(file);
        // var fileInfo = new System.IO.FileInfo(file);
        // UnityWebRequest test = new UnityWebRequest(file);
        // test.
        // var www = UnityWebRequestTexture.GetTexture("file://" + file);
        // www.SendWebRequest();
        //texture loaded
        // Texture2D texture = DownloadHandlerTexture.GetContent(www);
        // Texture2D texture = new Texture2D(1920,1080);
        // ImageConversion.LoadImage(texture, bytes);
        // texture.LoadRawTextureData(bytes);
        // return texture;
    }
    public static string GetPicturesFolderPath(EmotionsEstimator.Emotion emotion)
    {
        string emotionFolder = "";
        switch (emotion)
        {
            case EmotionsEstimator.Emotion.EMOTION_ANGRY:
                emotionFolder = "/screens/angry/";
                break;
            case EmotionsEstimator.Emotion.EMOTION_HAPPY:
                emotionFolder = "/screens/happy/";
                break;
            case EmotionsEstimator.Emotion.EMOTION_SURPRISE:
                emotionFolder = "/screens/surprised/";
                break;
            case EmotionsEstimator.Emotion.EMOTION_NEUTRAL:
                emotionFolder = "/screens/neutral/";
                break;
        }
    #if UNITY_EDITOR
        return System.Environment.GetFolderPath( System.Environment.SpecialFolder.DesktopDirectory ) + emotionFolder;
  
    #else
        return Application.persistentDataPath + emotionFolder;
    #endif
    }
}
