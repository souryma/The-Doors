using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject _leftCurtainsObject;
    public GameObject _rightCurtainsObject;
    public GameObject _middleCurtainsObject;
    public GameObject PlayButton;
    public GameObject Quitbutton;
    public GameObject CreditButton;

    public void PlayGame()
    {
        PlayButton.SetActive(false);
        Quitbutton.SetActive(false);
        CreditButton.SetActive(false);
        
        // // Open curtain
        // _leftCurtainsObject.transform.DOScaleY(13f, 1);
        // _rightCurtainsObject.transform.DOScaleY(-13f, 1);
        // _middleCurtainsObject.transform.DOScaleZ(1f, 1f);

        StartCoroutine("startGame");
    }

    public void ShowCredits()
    {
        Debug.Log("CREDITS");
    }

    private IEnumerator startGame()
    {
        yield return new WaitForSeconds(1);
        
        SceneManager.LoadScene("Scenes/MainScene");
    }

    public void QuitGame()
    {
        Debug.LogWarning("Cannot quit in editor mode");
        Application.Quit();
    }
}
