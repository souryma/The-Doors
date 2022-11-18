using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ToolTip), true)]
public class ToolTip_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ToolTip myScript = (ToolTip)target;

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        GUI.color = new Color(0.5f, 1f, 0.5f, 1);
        if (GUILayout.Button("Show"))
        {
            Undo.RecordObject(myScript, "Show " + myScript.gameObject.name + " tool tip");
            myScript.ForcedShow();
        }

        GUI.color = new Color(1f, 0.5f, 0.5f, 1);
        if (GUILayout.Button("Hide"))
        {
            Undo.RecordObject(myScript, "Hide " + myScript.gameObject.name + " tool tip");
            myScript.ForcedHide();
        }

        EditorGUILayout.EndHorizontal();

        GUI.color = Color.white;

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUI.color = new Color(0.5f, 0.5f, 1f, 1);
        if (GUILayout.Button("AutoFill"))
        {
            Undo.RecordObject(myScript, "Auto fill " + myScript.gameObject.name + " tool tip");
            myScript.AutoFill();
        }
        EditorGUILayout.EndHorizontal();
    }
}
