using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinListSlot : MonoBehaviour
{
    [SerializeField] Image image;

    private int skinID;

    public void SetData(Skin skin)
    {
        image.sprite = skin.FrontSprite;
        skinID = skin.ID;
    }

    public void SkinSelect()
    {

    }
}
