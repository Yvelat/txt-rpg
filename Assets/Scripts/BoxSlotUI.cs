using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxSlotUI : MonoBehaviour
{
    [SerializeField] Image monsterIcon;
    [SerializeField] TextMeshProUGUI levelText;

    public int index = 0;

    Monster monster;

    RectTransform rectTransform;

    public void SetDetails()
    {
        //GetComponentInParent<MonsterBox>().UpdateDetails(item, index);
    }

    public Image MonsterIcon => monsterIcon;
    public TextMeshProUGUI LevelText => levelText;

    public float Height => rectTransform.rect.height;

    public void SetData(BoxSlot monsterSlot, int index)
    {
        rectTransform = GetComponent<RectTransform>();
        monsterIcon.sprite = monsterSlot.Monster.Base.BattleIcon;
        levelText.text = $"{monsterSlot.Level}";
        monster = monsterSlot.Monster;
        this.index = index;
    }
}
