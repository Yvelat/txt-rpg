using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBoxUiSlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI level;

    private MonsterBoxItem monsterBoxItem;

    public Vector2Int index;

    public void SetData(MonsterBoxItem mItem, Vector2Int index)
    {
        this.index = index;
        monsterBoxItem = mItem;
        level.text = $"{monsterBoxItem.level}";
        icon.sprite = monsterBoxItem.monster.FrontSprite;
    }
}
