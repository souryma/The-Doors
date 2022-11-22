using UnityEngine;
using UnityEngine.Networking;

using System.IO;

#if UNITY_ANDROID
using UnityAndroidOpenUrl;
#endif

public class About : MonoBehaviour
{
    [SerializeField] string face3DiviLinc = "https://face.3divi.com";
    [SerializeField] string info3DiviLinc = "info@3divi.com";
    [SerializeField] string privacyPolicyFile = "Privacy_Policy_3DiVi_Face_Mask.pdf";

    #region Unity UI events

    public void Face3DiviLinc()
    {
        Application.OpenURL(face3DiviLinc);
    }

    public void Info3DiviLinc()
    {
        // Subject of the mail
        string subject = MyEscapeURL("FEEDBACK/SUGGESTION");

        // Body of the mail which consists of Device Model and its Operating System
        string body = MyEscapeURL("Please enter your message here\n\n\n\n" +
         "________" +
         "\n\nPlease do not modify this\n\n" +
         "Model: " + SystemInfo.deviceModel + "\n\n" +
            "OS: " + SystemInfo.operatingSystem + "\n\n" +
         "________");

        // Open the Default Mail App (if there is no mail application, the call is ignored)
        Application.OpenURL("mailto:" + info3DiviLinc + "?subject=" + subject + "&body=" + body);
    }

    public void PrivacyPolicyLinc()
    {
        string filePath = Path.Combine(FaceSDKLoader.AppDataPath, privacyPolicyFile);

#if UNITY_STANDALONE || UNITY_EDITOR
        Application.OpenURL(filePath);
#else
        AndroidOpenUrl.OpenFile(filePath);
#endif
    }

    #endregion

    string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }
}
