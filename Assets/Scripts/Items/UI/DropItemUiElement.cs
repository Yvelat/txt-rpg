using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropItemUiElement : MonoBehaviour
{
    [SerializeField] Image ItemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemCount;

    public void SetData(DropTableElement drop)
    {
        ItemIcon.sprite = drop.drop.Item.Icon;
        itemName.text = drop.drop.Item.Name;
        itemCount.text = $"x{drop.count}";
    }
}
