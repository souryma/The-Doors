using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public enum ButtonAction
    {
        Quit = 0,
        Close = 1,
        Count = 2
    }

    [SerializeField] GameObject body;
    [SerializeField] Text headerText;
    [SerializeField] Text messageText;

    [SerializeField] Text buttonText;

    [SerializeField] string defaultHeader = "Error";

    ButtonAction m_buttonAction;

    public void ShowMessage(string message, string header = null, bool printToLog = true, ButtonAction buttonAction = ButtonAction.Quit)
    {
        Debug.LogError((header == null ? "" : header + "\n") + message);

        messageText.text = message;
        headerText.text = header != null ? header : defaultHeader;

        m_buttonAction = buttonAction;
        buttonText.text = m_buttonAction.ToString();

        body.SetActive(true);
    }

    public void Action()
    {
        switch(m_buttonAction)
        {
            case ButtonAction.Quit:
                Application.Quit();
                break;

            case ButtonAction.Close:
            default:
                body.SetActive(false);
                break;
        }
    }

}
