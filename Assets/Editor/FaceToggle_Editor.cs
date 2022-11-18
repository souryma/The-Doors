using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(FaceToggle), true)]
public class FaceToggle_Editor : ToggleEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FaceToggle myScript = (FaceToggle)target;

        serializedObject.Update();

        #region Time Animation

        EditorGUILayout.LabelField("Time Animation");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // --------------- Animation Curve ---------------

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Animation Curve");
        SerializedProperty acSP = serializedObject.FindProperty("timeAnimation.animationCurve");

        acSP.animationCurveValue = EditorGUILayout.CurveField(acSP.animationCurveValue);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndHorizontal();

        // -------------------- Speed --------------------

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Speed");
        SerializedProperty spSP = serializedObject.FindProperty("timeAnimation.speed");

        spSP.floatValue = EditorGUILayout.FloatField(spSP.floatValue);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        #endregion

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Up Scale");
        SerializedProperty scSP = serializedObject.FindProperty("upScale");
        scSP.floatValue = EditorGUILayout.FloatField(scSP.floatValue);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
    }
}
