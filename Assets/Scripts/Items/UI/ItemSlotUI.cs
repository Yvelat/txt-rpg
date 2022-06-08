using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI countText;

    public int index = 0;

    ItemBase item;

    RectTransform rectTransform;

    private void Awake()
    {
        
    }

    public void SetDetails()
    {
        GetComponentInParent<InventoryUI>().UpdateDetails(item, index);
    }

    public Image ItemIcon => itemIcon;
    public TextMeshProUGUI CountText => countText;

    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot, int index)
    {
        rectTransform = GetComponent<RectTransform>();
        itemIcon.sprite = itemSlot.Item.Icon;
        countText.text = $"x{itemSlot.Count}";
        item = itemSlot.Item;
        this.index = index;
    }
}
