using UnityEngine;

namespace UnityAndroidOpenUrl
{
    public static class AndroidOpenUrl
    {
        public static void OpenFile(string url, string dataType = "application/pdf")
        {
            AndroidJavaObject clazz = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = clazz.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            intent.Call<AndroidJavaObject>("addFlags", intent.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION"));
            intent.Call<AndroidJavaObject>("setAction", intent.GetStatic<string>("ACTION_VIEW"));

            var apiLevel = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");

            AndroidJavaObject uri;
            if (apiLevel > 23)
            {
                AndroidJavaClass fileProvider = new AndroidJavaClass("android.support.v4.content.FileProvider");
                AndroidJavaObject file = new AndroidJavaObject("java.io.File", url);

                AndroidJavaObject unityContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                string packageName = unityContext.Call<string>("getPackageName");
                string authority = packageName + ".fileprovider";

                uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", unityContext, authority, file);
            }
            else
            {
                var uriClazz = new AndroidJavaClass("android.net.Uri");
                var file = new AndroidJavaObject("java.io.File", url);
                uri = uriClazz.CallStatic<AndroidJavaObject>("fromFile", file);
            }

            intent.Call<AndroidJavaObject>("setType", dataType);
            intent.Call<AndroidJavaObject>("setData", uri);

            currentActivity.Call("startActivity", intent);
        }
    }
}
