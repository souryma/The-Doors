using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

[CustomEditor(typeof(FaceSDKLoader), true)]
public class FaceSDKLoader_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FaceSDKLoader myScript = (FaceSDKLoader)target;

        List<string> saListFiles = FaceSDKLoader.StreamingAssetsFiles;

        if (saListFiles == null)
        {
            GUI.color = new Color(1f, 0.5f, 0.5f, 1);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("The file with the list of StreamingAssets content was not created.");
            EditorGUILayout.LabelField("Click \"Update SDK files list\"");

            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
        }
        else
        {
            for (int i = 0; i < saListFiles.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField((i + 1).ToString() + ". \t" + saListFiles[i]);
                EditorGUILayout.EndVertical();
            }

        }

        GUI.color = new Color(0.5f, 1, 0.5f, 1);
        if (GUILayout.Button("Update SDK files list"))
            myScript.FileFilesList();
    }
}
