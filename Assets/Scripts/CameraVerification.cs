using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class CameraVerification : MonoBehaviour
{
    [SerializeField] private GameObject _playButton;
    [SerializeField] private RawImage _happyImage;
    [SerializeField] private RawImage _neutralImage;
    [SerializeField] private RawImage _angryImage;
    [SerializeField] private RawImage _surpriseImage;

    // Start is called before the first frame update
    void Start()
    {
        _playButton.SetActive(false);
        GameManager.Instance.GameSpeed = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.HasHappy)
            _happyImage.texture = GameManager.Instance.HappyFace;
        if (GameManager.Instance.HasAngry)
            _angryImage.texture = GameManager.Instance.AngryFace;
        if (GameManager.Instance.HasNeutral)
            _neutralImage.texture = GameManager.Instance.NeutralFace;
        if (GameManager.Instance.HasSurprised)
            _surpriseImage.texture = GameManager.Instance.SurprisedFace;

        if (GameManager.Instance.AreAllEmotionsReady())
        {
            _playButton.SetActive(true);
        }
    }


    public void OnVerificationValid()
    {
        GameManager.Instance.GameSpeed = 0.007f;
        gameObject.SetActive(false);
        GameManager.Instance.IsVerificationDone = true;
    }
}