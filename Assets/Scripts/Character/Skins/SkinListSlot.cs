using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinListSlot : MonoBehaviour
{
    [SerializeField] Image image;

    private Skin skin;

    public void SetData(Skin skin)
    {
        image.sprite = skin.FrontSprite;
        this.skin = skin;
    }

    public void SkinSelect()
    {
        FindObjectOfType<FirstBootHandler>().SetSkin(skin);
    }
}
