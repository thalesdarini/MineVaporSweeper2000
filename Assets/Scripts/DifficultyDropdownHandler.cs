using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyDropdownHandler : MonoBehaviour
{
    TMP_Dropdown m_Dropdown;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Dropdown = GetComponent<TMP_Dropdown>();

        m_Dropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(m_Dropdown);
        });
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        Difficulty difficulty;
        switch (dropdown.value)
        {
            case 0: // Easy
                difficulty = Difficulty.Easy;
                break;

            case 1: // Normal
                difficulty = Difficulty.Normal;
                break;

            case 2: // Hard
                difficulty = Difficulty.Hard;
                break;

            default:
                difficulty = Difficulty.Easy;
                break;
        }

        Minesweeper.instance.SetDifficulty(difficulty);
    }
}
