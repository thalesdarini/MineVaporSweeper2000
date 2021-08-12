using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlagCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = "0";
    }

    public void UpdateText(int number)
    {
        text.text = number.ToString();
    }
}
