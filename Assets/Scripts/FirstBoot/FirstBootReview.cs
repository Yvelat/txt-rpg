using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstBootReview : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image playerSkin;

    public void SetReview(string name, Skin skin)
    {
        nameText.text = name;
        playerSkin.sprite = skin.FrontSprite;
    }

    public void Confirm()
    {
        //TODO: switch scene and set data
    }

    public void Abort()
    {
        FindObjectOfType<FirstBootHandler>().Restart();
    }
}
