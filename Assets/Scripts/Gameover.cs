using TMPro;
using UnityEngine;

public class Gameover : MonoBehaviour
{
    public TextMeshProUGUI Score;

    public void SetScore(int score)
    {
        Score.text = "Number of opened doors : " + score;
    }
    public void OnTryAgain()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }
}
