using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraVerification : MonoBehaviour
{
    private RawImage _imageObject;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.GameSpeed = 0;
        _imageObject = GetComponentInChildren<RawImage>();
    }

    public void OnVerificationValid()
    {
        
        gameObject.SetActive(false);
        GameManager.Instance.IsVerificationDone = true;
    }
}
