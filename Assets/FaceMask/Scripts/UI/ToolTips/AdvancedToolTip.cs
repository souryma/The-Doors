using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class AdvancedToolTip : ToolTip
{
    [SerializeField] Text messageText;
    [SerializeField] List<string> messagesList;
    [SerializeField] float autoHideTime = 4f;

    float visibleT = 0;
    int idMessage = 0;

    protected override void Awake()
    {
        base.Awake();
        idMessage = Random.Range(0, messagesList.Count - 1);
    }

    protected override void Update()
    {
        base.Update();

        if(Visible)
        {
            if (visibleT < autoHideTime)
                visibleT += Time.deltaTime;
            else
                Hide();
        }
    }

    public override void Show()
    {
        if(!Visible)
        {
            if (messagesList.Count > 0)
            {
                messageText.text = messagesList[idMessage];
                idMessage = (idMessage + 1) % messagesList.Count;
            }
            visibleT = 0;
        }

        base.Show();
    }
}
