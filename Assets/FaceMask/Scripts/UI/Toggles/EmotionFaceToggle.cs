using UnityEngine;

public class EmotionFaceToggle : FaceToggle
{
    bool emotionMenuVisible = false;
    bool isActive = false;

    [SerializeField] AdvancedToolTip firstClickInfo;
    [SerializeField] ToggleEvent onHoldPress;

    protected override void Awake()
    {
        base.Awake();
        isActive = isOn;
    }

    protected override void OnChange(bool m_isOn)
    {
        if(m_isOn)
        {
            if(isActive)
            {
                emotionMenuVisible = !emotionMenuVisible;

                animator.SetBool("Visible", !emotionMenuVisible);
                onHoldPress.Invoke(emotionMenuVisible);

                firstClickInfo.Hide();
            }
            else
            {
                animator.SetBool("Visible", true);
                
                if (!UserSettings.ShownEmotionToolTip)
                {
                    firstClickInfo.Show();
                    UserSettings.ShownEmotionToolTip = true;
                }

                isActive = true;
                base.OnChange(true);
            }
        }
        else
        {
            isActive = false;
            emotionMenuVisible = false;

            animator.SetBool("Visible", false);
            firstClickInfo.Hide();

            onHoldPress.Invoke(false);
            base.OnChange(false);
        }
    }
}