using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Gameover : MonoBehaviour
{
    public TextMeshProUGUI Score;
    public GameObject BackGroundImage;

    public void SetScore(int score)
    {
        Score.text = "Number of opened doors : " + score;
    }
    public void OnTryAgain()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }

    public void SetGameoverBG(Texture image)
    {
        BackGroundImage.GetComponent<RawImage>().texture = image;
    }
}
