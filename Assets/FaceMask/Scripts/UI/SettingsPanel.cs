using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsPanel : Selectable, IDragHandler, IPointerClickHandler
{
    [Header("Main")]
    [SerializeField] RectTransform parent;
    [SerializeField] RectTransform rectTransform;

    [Header("Positions")]
    [SerializeField] Vector2 defaultPosition;
    [SerializeField] Vector2 altPosition;

    [Header("Animation")]
    [SerializeField] float deltaSwipe = 1f;

    [SerializeField] TimeLineAnimation timeAnimation;
    [SerializeField] RectTransform arrow;

    Vector2 deltaDown;
    Vector2 lastFramePostion;
    bool dragging = false;

    Vector2 startAnimationPosition;
    float startYScale = 1;

    bool upMove = false;

    [Header ("StartAnimation")]
    [SerializeField] TimeLineAnimation startAnimation;

    protected override void Start()
    {
        base.Start();

        if(Application.isPlaying)
            startAnimation.Start();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        deltaDown = rectTransform.anchoredPosition - ScreenPoint(eventData);

        startAnimation.Stop();
        timeAnimation.Stop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (dragging)
        {
            Vector2 velocity = rectTransform.anchoredPosition - lastFramePostion;

            if (velocity.sqrMagnitude > deltaSwipe * deltaSwipe)
                upMove = velocity.y > 0;
            else
                upMove = Mathf.InverseLerp(defaultPosition.y, altPosition.y, rectTransform.anchoredPosition.y) > 0.5f;

            startAnimationPosition = rectTransform.anchoredPosition;
            startYScale = arrow.localScale.y;

            timeAnimation.Start();

            dragging = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!timeAnimation.IsRuning)
        {
            timeAnimation.Start();

            upMove = !upMove;

            startAnimationPosition = rectTransform.anchoredPosition;
            startYScale = arrow.localScale.y;
        }

        dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragging = true;
        lastFramePostion = rectTransform.anchoredPosition;

        Vector2 deltaPos = deltaDown + ScreenPoint(eventData);
        float x = Mathf.Clamp(deltaPos.x, defaultPosition.x, altPosition.x);
        float y = Mathf.Clamp(deltaPos.y, defaultPosition.y, altPosition.y);

        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    Vector2 ScreenPoint(PointerEventData eventData)
    {
        return eventData.position * (parent.sizeDelta.x / Screen.width);
    }

    void Update()
    {
        if(startAnimation.Update(Time.deltaTime))
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(defaultPosition, altPosition, startAnimation.Value);

        if (!dragging && timeAnimation.Update(Time.deltaTime))
        {
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startAnimationPosition, upMove ? altPosition : defaultPosition, timeAnimation.Value);
            arrow.localScale = new Vector3(1, Mathf.Lerp(startYScale, upMove ? -1 : 1, timeAnimation.Value), 1);
        }
    }

#if UNITY_EDITOR

    // Methods to use from context menu

    [ContextMenu ("SAVE DEFAULT position")]
    public void SaveDefault()
    {
        UnityEditor.Undo.RecordObject(this, "Reset save default anchored position");
        defaultPosition = rectTransform.anchoredPosition;
    }

    [ContextMenu("SAVE ALT position")]
    public void SaveAlt()
    {
        UnityEditor.Undo.RecordObject(this, "Reset save alt anchored position");
        altPosition = rectTransform.anchoredPosition;
    }

    [ContextMenu("Get DEFAULT position")]
    public void GetDefault()
    {
        UnityEditor.Undo.RecordObject(rectTransform, "Get save default anchored position");
        rectTransform.anchoredPosition = defaultPosition;
    }

    [ContextMenu("Get ALT position")]
    public void GetAlt()
    {
        UnityEditor.Undo.RecordObject(rectTransform, "Get save alt anchored position");
        rectTransform.anchoredPosition = altPosition;
    }

#endif
}
