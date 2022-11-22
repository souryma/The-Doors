using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TimeLineAnimation timeAnimatiom;

    float currentAlpha;
    float targetAlpha;

    public bool Visible
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        Visible = gameObject.activeInHierarchy;
        currentAlpha = canvasGroup.alpha;
    }

    protected virtual void Update()
    {
        if (timeAnimatiom.IsRuning)
        {
            if(timeAnimatiom.Update(Time.deltaTime))
                canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, timeAnimatiom.Value);
            else
            {
                if(!Visible)
                    gameObject.SetActive(false);
            }
        }
    }

    public virtual void Show()
    {
        if (!Visible)
        {
            gameObject.SetActive(true);
            Visible = true;

            currentAlpha = canvasGroup.alpha;
            targetAlpha = 1;

            timeAnimatiom.Start();
        }
    }

    public virtual void Hide()
    {
        if (Visible)
        {
            Visible = false;
            currentAlpha = canvasGroup.alpha;
            targetAlpha = 0;

            timeAnimatiom.Start();
        }
    }

    public void Display(bool visible)
    {
        if (visible)
            Show();
        else
            Hide();
    }

    public void UnDisplay(bool visible)
    {
        if (visible)
            Hide();
        else
            Show(); 
    }

#if UNITY_EDITOR

    // Methods to use from context menu or editor (ToolTip_Editor)

    [ContextMenu ("Auto fill")]
    public void AutoFill()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    [ContextMenu("Show")]
    public void ForcedShow()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
    }

    [ContextMenu("Hide")]
    public void ForcedHide()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0;
    }

#endif
}
