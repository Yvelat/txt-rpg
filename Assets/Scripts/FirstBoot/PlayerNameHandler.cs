using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    public void Confirm()
    {
        FindObjectOfType<FirstBootHandler>().SetName(inputField.text);
    }
}
