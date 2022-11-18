using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;

[ExecuteInEditMode]
public class UIAspectRatioSwitcher : MonoBehaviour
{
#if UNITY_EDITOR

    [Tooltip ("Specify a value other than AutoRotation if you want to force the layout switch (Only relevant in the editor.).")]
    [SerializeField] ScreenOrientation forceOrientation = ScreenOrientation.AutoRotation;

#endif

    [Header ("Rect transform objects")]
    [SerializeField] List<UIOrientationLayer> uiOrientationLayers = new List<UIOrientationLayer>();

    [Header("Game objects")]
    [SerializeField] List<GameObjectOrientationLayer> goOrientationLayers = new List<GameObjectOrientationLayer>();

    ScreenOrientation GetOrientation()
    {
#if UNITY_EDITOR
        if (forceOrientation != ScreenOrientation.AutoRotation)
            return forceOrientation;
        else
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;
#else
        return Application.platform == RuntimePlatform.Android ? ScreenOrientation.Portrait : ScreenOrientation.Landscape;
#endif
    }

    void Awake()
    {
        SwitchOrientation(GetOrientation());
    }

    void SwitchOrientation(ScreenOrientation screenOrientation)
    {
        foreach (UIOrientationLayer layer in uiOrientationLayers)
            layer.SetOrientation(screenOrientation);

        foreach (GameObjectOrientationLayer layer in goOrientationLayers)
            layer.SetOrientation(screenOrientation);
    }

#if UNITY_EDITOR

    ScreenOrientation orientation = ScreenOrientation.AutoRotation;

    void Update()
    {
        ScreenOrientation newOrientation = GetOrientation();

        if (orientation != newOrientation)
        {
            SwitchOrientation(newOrientation);
            orientation = newOrientation;
        }
    }

#endif
}

[System.Serializable]
public class UIOrientationLayer
{
    [SerializeField] RectTransform target;

    [Header("Orientation parents")]
    [SerializeField] RectTransform portaintOrientation;
    [SerializeField] RectTransform landscapeOrientation;

    public void SetOrientation(ScreenOrientation orientation)
    {
        RectTransform newParent = orientation == ScreenOrientation.LandscapeLeft ? landscapeOrientation : portaintOrientation;

        target.SetParent(newParent, false);
        target.anchoredPosition = Vector2.zero;
        target.sizeDelta = Vector2.zero;
    }
}

[System.Serializable]
public class GameObjectOrientationLayer
{
    [SerializeField] GameObject target;

    [Header("Orientation visibles")]
    [SerializeField] bool portaintVisible;
    [SerializeField] bool landscapeVisible;

    public void SetOrientation(ScreenOrientation orientation)
    {
        target.SetActive(orientation == ScreenOrientation.LandscapeLeft ? landscapeVisible : portaintVisible);
    }
}