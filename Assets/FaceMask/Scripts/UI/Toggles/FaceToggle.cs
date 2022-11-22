using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FaceToggle : Toggle
{
    [SerializeField] TimeLineAnimation timeAnimation;
    [SerializeField] float upScale = 1.2f;

    Vector3 defaultScale;

    Vector3 currentScale;
    Vector3 targetScale;

    protected override void Awake()
    {
        base.Awake();

        defaultScale = transform.localScale;

        onValueChanged.AddListener(new UnityAction<bool>(OnChange));
    }

    protected virtual void OnChange(bool isOn)
    {
        timeAnimation.Start();

        currentScale = transform.localScale;
        targetScale = isOn ? defaultScale * upScale : defaultScale;
    }

    virtual protected void Update()
    {
        if (timeAnimation.Update(Time.deltaTime))
            transform.localScale = Vector3.LerpUnclamped(currentScale, targetScale, timeAnimation.Value);
    }
}
