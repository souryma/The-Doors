using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;

public class MainMenu : MonoBehaviour
{
    public GameObject _leftCurtainsObject;
    public GameObject _rightCurtainsObject;
    public GameObject _middleCurtainsObject;
    public GameObject PlayButton;
    public GameObject Quitbutton;
    public GameObject CreditButton;
    public GameObject Credits;
    public GameObject Title;
    public GameObject DropdownParent;
    public TMP_Dropdown Dropdown;

    private void Start()
    {
        List<TMP_Dropdown.OptionData> list= new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = WebCamTexture.devices[i].name;
            list.Add(data);
        }

        Dropdown.options = list;
        //
        // foreach (var dropdownOption in Dropdown.options)
        // {
        //     Debug.Log(WebCamTexture.devices[i].name);
        //     dropdownOption.text = WebCamTexture.devices[i].name;
        //     i++;
        // }
        //open curtains
        _leftCurtainsObject.transform.DOScaleY(13f, 1);
        _rightCurtainsObject.transform.DOScaleY(-13f, 1);
        _middleCurtainsObject.transform.DOScaleZ(1f, 1f);
    }

    public void PlayGame()
    {
        PlayButton.SetActive(false);
        Quitbutton.SetActive(false);
        CreditButton.SetActive(false);
        Title.SetActive(false);
        DropdownParent.SetActive(false);

        // Close curtain
        _leftCurtainsObject.transform.DOScaleY(32f, 1);
        _rightCurtainsObject.transform.DOScaleY(-32f, 1);
        _middleCurtainsObject.transform.DOScaleZ(53.6f, 1f);

        StartCoroutine(nameof(startGame));
    }

    public void ShowCredits()
    {
        PlayButton.SetActive(false);
        Quitbutton.SetActive(false);
        CreditButton.SetActive(false);
        Title.SetActive(false);
        DropdownParent.SetActive(false);
        // Close curtain
        _leftCurtainsObject.transform.DOScaleY(32f, 1);
        _rightCurtainsObject.transform.DOScaleY(-32f, 1);
        _middleCurtainsObject.transform.DOScaleZ(53.6f, 1f);

        StartCoroutine(nameof(displayCredits));
    }

    public void backToMenu()
    {
        Credits.SetActive(false);
        Title.SetActive(false);

        // Close curtain
        _leftCurtainsObject.transform.DOScaleY(32f, 1);
        _rightCurtainsObject.transform.DOScaleY(-32f, 1);
        _middleCurtainsObject.transform.DOScaleZ(53.6f, 1f);

        StartCoroutine(nameof(BackToMenu));
    }

    private IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(1);

        // Open curtain
        _leftCurtainsObject.transform.DOScaleY(13f, 1);
        _rightCurtainsObject.transform.DOScaleY(-13f, 1);
        _middleCurtainsObject.transform.DOScaleZ(1f, 1f);

        PlayButton.SetActive(true);
        Quitbutton.SetActive(true);
        CreditButton.SetActive(true);
        Title.SetActive(true);
        DropdownParent.SetActive(true);
    }

    private IEnumerator displayCredits()
    {
        yield return new WaitForSeconds(1);

        // Open curtain
        _leftCurtainsObject.transform.DOScaleY(13f, 1);
        _rightCurtainsObject.transform.DOScaleY(-13f, 1);
        _middleCurtainsObject.transform.DOScaleZ(1f, 1f);

        Credits.SetActive(true);
        Title.SetActive(true);
    }

    private IEnumerator startGame()
    {
        yield return new WaitForSeconds(1);
        PlayerPrefs.SetInt("camera", Dropdown.value);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Scenes/MainScene");
    }

    public void QuitGame()
    {
        Debug.LogWarning("Cannot quit in editor mode");
        Application.Quit();
    }
}