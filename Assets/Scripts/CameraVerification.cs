using UnityEngine;

public class CameraVerification : MonoBehaviour
{
    [SerializeField] private GameObject _dressingRoomPrefab;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _playButton;

    private GameObject _verificationRoom;
    private MeshRenderer _happyQuad;
    private MeshRenderer _angryQuad;
    private MeshRenderer _surprisedQuad;
    private MeshRenderer _neutralQuad;
    private MeshRenderer _mirror;

    void Start()
    {
        _camera.transform.position = new Vector3(0, 40, 0);
        _verificationRoom = Instantiate(_dressingRoomPrefab);
        _verificationRoom.transform.position = new Vector3(-2.26f, 33.8f, 14.37f);

        _happyQuad = _verificationRoom.transform.Find("HappyShelf").transform.Find("HappyQuad")
            .GetComponent<MeshRenderer>();
        _angryQuad = _verificationRoom.transform.Find("AngryShelf").transform.Find("AngryQuad")
            .GetComponent<MeshRenderer>();
        _surprisedQuad = _verificationRoom.transform.Find("SurprisedShelf").transform.Find("SurprisedQuad")
            .GetComponent<MeshRenderer>();
        _neutralQuad = _verificationRoom.transform.Find("NeutralShelf").transform.Find("NeutralQuad")
            .GetComponent<MeshRenderer>();

        _mirror = _verificationRoom.transform.Find("Mirror").GetComponent<MeshRenderer>();

        _playButton.SetActive(false);
        GameManager.Instance.GameSpeed = 0;
        
        GameManager.Instance.OpenCurtains();
    }

    private void Update()
    {
        if (GameManager.Instance.HasHappy)
            _happyQuad.material.mainTexture = GameManager.Instance.HappyFace;
        if (GameManager.Instance.HasAngry)
            _angryQuad.material.mainTexture = GameManager.Instance.AngryFace;
        if (GameManager.Instance.HasNeutral)
            _neutralQuad.material.mainTexture = GameManager.Instance.NeutralFace;
        if (GameManager.Instance.HasSurprised)
            _surprisedQuad.material.mainTexture = GameManager.Instance.SurprisedFace;

        _mirror.material.mainTexture = GameManager.Instance.faceManager.webcamTexture;

        if (GameManager.Instance.AreAllEmotionsReady())
        {
            _playButton.SetActive(true);
        }
    }

    public void OnVerificationValid()
    {
        _verificationRoom.SetActive(false);
        gameObject.SetActive(false);
        _camera.transform.position = new Vector3(0, 1.5f, 0);
        GameManager.Instance.IsVerificationDone = true;
    }
}