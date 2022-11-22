using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultyDropDown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var dropdown = transform.GetComponent<TMP_Dropdown>();
        
        dropdown.options.Clear();

        List<string> items = new List<string>();
        items.Add("Easy");
        items.Add("Medium");
        items.Add("Hard");

        foreach (var item in items)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = item });
        }

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); }) ;
    }

    private void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        switch (dropdown.options[dropdown.value].text)
        {
            case "Easy":
                GameManager.Instance.GameDifficulty = GameManager.Difficulty.Easy;
                break;
            case "Medium":
                GameManager.Instance.GameDifficulty = GameManager.Difficulty.Medium;
                break;
            case "Hard":
                GameManager.Instance.GameDifficulty = GameManager.Difficulty.Hard;
                break;
        }
    }
}
