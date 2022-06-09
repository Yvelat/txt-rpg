using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMeshAnimation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enemyText;
    [SerializeField] float minXOffset;
    [SerializeField] float maxXOffset;

    bool awake = false;

    float offset;

    private void OnEnable()
    {
        offset = minXOffset;
        awake = true;
    }

    private void OnDisable()
    {
        offset = minXOffset;
        awake = false;
    }

    private void Update()
    {

        if (awake)
        {
            offset += 0.05f * Time.deltaTime;

            if(offset > maxXOffset) offset = maxXOffset;

            //enemyText.fontMaterial.mainTextureOffset = new Vector2(offset, enemyText.fontMaterial.mainTextureOffset.y);
            enemyText.fontSharedMaterial.SetTextureOffset("_FaceTex", new Vector2(offset, 0));
        }
    }
}
