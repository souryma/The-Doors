using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameover : MonoBehaviour
{
    [SerializeField] private GameObject _dressingRoomPrefab;
    [SerializeField] private Camera _camera;

    private GameObject _gameOverRoom;
    private MeshRenderer _retryQuad;
    private MeshRenderer _restartQuad;
    private MeshRenderer _mirror;
    
    private TextMeshPro Score;
    // public GameObject BackGroundImage;

    private void Start()
    {
        _gameOverRoom = Instantiate(_dressingRoomPrefab);
        _gameOverRoom.transform.position = new Vector3(-2.26f, 33.8f, 14.37f);

        _retryQuad = _gameOverRoom.transform.Find("TryAgainGroup").transform.Find("TryAgainCanvas")
            .GetComponent<MeshRenderer>();
        _restartQuad = _gameOverRoom.transform.Find("RestartGroup").transform.Find("RestartCanvas")
            .GetComponent<MeshRenderer>();
        
        Score= _gameOverRoom.transform.Find("score").GetComponent<TextMeshPro>();

        _mirror = _gameOverRoom.transform.Find("Mirror").GetComponent<MeshRenderer>();
        
        _gameOverRoom.SetActive(false);
        // gameObject.SetActive(false);
    }

    
    public void SetScore(int score)
    {
        Score.text = "Score : " + score;
    }
    public void OnTryAgain()
    {
        _camera.transform.position = new Vector3(0, 1.5f, 0);
        _gameOverRoom.SetActive(false);
        gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }

    public void OnRestartGame()
    {
        // _camera.transform.position = new Vector3(0, 1.5f, 0);
        _gameOverRoom.SetActive(false);
        // gameObject.SetActive(false);
        GameManager.Instance.RestartGameAndVerif();
    }

    public void ShowGameOver()
    {
        _gameOverRoom.SetActive(true);
        // gameObject.SetActive(true);
        _retryQuad.material.mainTexture = GameManager.Instance.AngryFace;
        _restartQuad.material.mainTexture = GameManager.Instance.SurprisedFace;
        _mirror.material.mainTexture = GameManager.Instance.faceManager.webcamTexture;

        
        _camera.transform.position = new Vector3(0, 40, 0);

    }
    // public void SetGameoverBG(Texture image)
    // {
    //     BackGroundImage.GetComponent<RawImage>().texture = image;
    // }
}
