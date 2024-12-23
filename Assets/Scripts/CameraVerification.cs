using System.Collections;
using TMPro;
using UnityEngine;

public class CameraVerification : MonoBehaviour
{
    [SerializeField] private GameObject _dressingRoomPrefab;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _playButton;
    [SerializeField] private Material defaultMaterial;

    private GameObject _verificationRoom;
    private TextMeshPro _verificationRoomText;

    private MeshRenderer _happyQuad;
    private MeshRenderer _angryQuad;
    private MeshRenderer _sadQuad;
    private MeshRenderer _surprisedQuad;
    private MeshRenderer _neutralQuad;
    private MeshRenderer _mirror;

    private int _step = 0;
    private bool _stepDone = false;

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
        _sadQuad = _verificationRoom.transform.Find("SadShelf").transform.Find("SadQuad")
            .GetComponent<MeshRenderer>();
        _verificationRoomText = _verificationRoom.transform.Find("UpText").GetComponent<TextMeshPro>();

        _mirror = _verificationRoom.transform.Find("Mirror").GetComponent<MeshRenderer>();

        _playButton.SetActive(false);
        GameManager.Instance.GameSpeed = 0;

        GameManager.Instance.OpenCurtains();
    }

    private void Update()
    {
        _mirror.material.mainTexture = WebcamManager.instance.Webcam1;

        switch (_step)
        {
            case 0:
                if (_stepDone)
                    break;
                _verificationRoomText.text = "Prepare your acting talents";
                StartCoroutine(nameof(IncreaseStep));
                _stepDone = true;
                break;
            case 1:

                _verificationRoomText.text = "Make a neutral expression";
                GameManager.Instance.GetEmotionsFace(EmotionManager.EMOTION.Neutral);
                if (GameManager.Instance.HasNeutral)
                {
                    _neutralQuad.material.mainTexture = GameManager.Instance.NeutralFace;
                    if (_stepDone)
                        break;
                    StartCoroutine(nameof(IncreaseStep));
                    _stepDone = true;
                }

                break;
            case 2:
                _verificationRoomText.text = "Make a happy expression";
                GameManager.Instance.GetEmotionsFace(EmotionManager.EMOTION.Happy);
                if (GameManager.Instance.HasHappy)
                {
                    _happyQuad.material.mainTexture = GameManager.Instance.HappyFace;
                    if (_stepDone)
                        break;
                    StartCoroutine(nameof(IncreaseStep));
                    _stepDone = true;
                }

                break;
            case 3:
                _verificationRoomText.text = "Make a surprised expression";
                GameManager.Instance.GetEmotionsFace(EmotionManager.EMOTION.Surprise);
                if (GameManager.Instance.HasSurprised)
                {
                    _surprisedQuad.material.mainTexture = GameManager.Instance.SurprisedFace;
                    if (_stepDone)
                        break;
                    StartCoroutine(nameof(IncreaseStep));
                    _stepDone = true;
                }

                break;
            case 4:
                _verificationRoomText.text = "Make an angry expression";
                GameManager.Instance.GetEmotionsFace(EmotionManager.EMOTION.Anger);
                if (GameManager.Instance.HasAngry)
                {
                    _angryQuad.material.mainTexture = GameManager.Instance.AngryFace;
                    if (_stepDone)
                        break;
                    StartCoroutine(nameof(IncreaseStep));
                    _stepDone = true;
                }

                break;
            case 5:
                _verificationRoomText.text = "Make a sad expression";
                GameManager.Instance.GetEmotionsFace(EmotionManager.EMOTION.Sadness);
                if (GameManager.Instance.HasSad)
                {
                    _sadQuad.material.mainTexture = GameManager.Instance.SadFace;
                    if (_stepDone)
                        break;
                    StartCoroutine(nameof(IncreaseStep));
                    _stepDone = true;
                }

                break;
            case 6:
                _verificationRoomText.text = "Smile to start the game :)";
                StartCoroutine(nameof(IncreaseStep));
                break;
        }

        if (GameManager.Instance.AreAllEmotionsReady() && _step > 5 &&
            GameManager.Instance.CheckForEmotion(EmotionManager.EMOTION.Happy))
        {
            // _playButton.SetActive(true);
            OnVerificationValid();
        }
    }

    private IEnumerator IncreaseStep()
    {
        yield return new WaitForSeconds(1);

        _step += 1;
        _stepDone = false;
    }

    public void ResetVerification()
    {
        GameManager.Instance.IsVerificationDone = false;
        _step = 0;
        _verificationRoom.SetActive(true);
        gameObject.SetActive(true);
        _camera.transform.position = new Vector3(0, 40, 0);
        _angryQuad.material.mainTexture =
            _happyQuad.material.mainTexture =
                _surprisedQuad.material.mainTexture =
                    _neutralQuad.material.mainTexture =
                        _sadQuad.material.mainTexture = defaultMaterial.mainTexture;
    }

    public void OnVerificationValid()
    {
        _verificationRoom.SetActive(false);
        gameObject.SetActive(false);
        _camera.transform.position = new Vector3(0, 1.5f, 0);
        GameManager.Instance.IsVerificationDone = true;
    }
}