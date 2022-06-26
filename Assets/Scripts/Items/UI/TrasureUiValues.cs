using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrasureUiValues : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI text;

    public void SetData(Sprite sprite, int value, string valText)
    {
        icon.sprite = sprite;
        text.text = $"{value} {valText}";
    }
}
