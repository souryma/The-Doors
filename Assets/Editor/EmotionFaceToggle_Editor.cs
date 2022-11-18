using UnityEditor;

[CustomEditor(typeof(EmotionFaceToggle), true)]
public class EmotionFaceToggle_Editor : FaceToggle_Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FaceToggle myScript = (FaceToggle)target;

        serializedObject.Update();

        EditorGUILayout.BeginVertical();

        SerializedProperty fcToolTipSP = serializedObject.FindProperty("firstClickInfo");

        EditorGUILayout.PropertyField(fcToolTipSP, true);
        serializedObject.ApplyModifiedProperties();


        // --------------- Events ---------------
        EditorGUILayout.LabelField("Second click");


        SerializedProperty eventSP = serializedObject.FindProperty("onHoldPress");

        EditorGUILayout.PropertyField(eventSP, true);
        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.EndVertical();
    }
}
