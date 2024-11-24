using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ImageLoaderSaver:MonoBehaviour
{
    private Dictionary<EmotionManager.EMOTION,List<Texture2D>> _faces = new ();
    private Dictionary<EmotionManager.EMOTION, List<string>> _imagesList = new Dictionary<EmotionManager.EMOTION, List<string>>();

    private void Start()
    {
      
        foreach (EmotionManager.EMOTION emotion in Enum.GetValues(typeof(EmotionManager.EMOTION)))
        {
            string pathToFolder = GetPicturesFolderPath(emotion);
            if(!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }
            _imagesList.Add(emotion, Directory.GetFiles(pathToFolder, "*.png").ToList());
        }
    }

    public void SavePictureToGallery( Texture2D texture2D, EmotionManager.EMOTION pictureEmotion )
    {
        string filename = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-f");
        string pathToFolder = GetPicturesFolderPath(pictureEmotion);
        byte[] bytes = ImageConversion.EncodeToPNG(texture2D);
        if( !filename.EndsWith( ".png" ) )
            filename += ".png";
        string path = Path.Combine(pathToFolder, filename );
    

        // Debug.Log( "Saving to: " + path );
        StartCoroutine(SaveImage(path, bytes));
       
        _imagesList[pictureEmotion].Add(path);

    }

    public IEnumerator SaveImage(string path, byte[] bytes)
    {
        File.WriteAllBytes( path, bytes );
        yield return 0;
    }
    public async Task<Texture2D> LoadPictureFromGallery(EmotionManager.EMOTION emotion)
    {


        byte[] bytes = await File.ReadAllBytesAsync(
            _imagesList[emotion][Random.Range(0, _imagesList[emotion].Count)]);
            
        Texture2D texture = new Texture2D(1920,1080);
        texture.LoadImage(bytes);
        
        return texture;
    }
    public static string GetPicturesFolderPath(EmotionManager.EMOTION emotion)
    {
        string emotionFolder = "";
        switch (emotion)
        {
            case EmotionManager.EMOTION.Anger:
                emotionFolder = "/screens/angry/";
                break;
            case EmotionManager.EMOTION.Happy:
                emotionFolder = "/screens/happy/";
                break;
            case EmotionManager.EMOTION.Surprise:
                emotionFolder = "/screens/surprised/";
                break;
            case EmotionManager.EMOTION.Neutral:
                emotionFolder = "/screens/neutral/";
                break;
            case EmotionManager.EMOTION.Sadness:
                emotionFolder = "/screens/sad/";
                break;
        }

        return Application.dataPath + emotionFolder;

    }
}