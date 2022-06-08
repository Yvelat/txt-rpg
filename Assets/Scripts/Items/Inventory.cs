using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ItemCategory { Item, CaptureDevice, Grimoire }

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> captureDeviceSlots;
    [SerializeField] List<ItemSlot> grimoireSlots;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, captureDeviceSlots, grimoireSlots };
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS", "CAPTURE DEVICES", "GRIMOIRES"
    };

    public List<ItemSlot> GetSlotsByCategory(int index)
    {
        return allSlots[index];
    }

    public bool IsCategoryEmpty(int index)
    {
        if (allSlots[index].Count == 0)
            return true;
        else
            return false;
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);

        return currentSlots[itemIndex].Item;

    }

    public ItemBase UseItem(int itemIndex, Monster selectedMonster, int selectedCategory)
    {

        var item = GetItem(itemIndex, selectedCategory);

        bool itemUsed = item.Use(selectedMonster);

        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item);

            return item;
        }

        return null;
    }

    public void AddItem(ItemBase item, int count=1)
    {
        int category = (int)GetCategoryFromItem(item);

        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);

        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();

    }

    public void RemoveItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);

        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;

        if (itemSlot.Count == 0) currentSlots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    public bool HasItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        return currentSlots.Exists(slot => slot.Item == item);
    }

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem || item is EvolutionItem)
            return ItemCategory.Item;
        else if (item is CaptureDeviceItem)
            return ItemCategory.CaptureDevice;
        else
            return ItemCategory.Grimoire;
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = slots.Select(slot => slot.GetSaveData()).ToList(),
            captureDevice = captureDeviceSlots.Select(slot => slot.GetSaveData()).ToList(),
            grimoires = grimoireSlots.Select(slot => slot.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        slots = saveData.items.Select(slot => new ItemSlot(slot)).ToList();
        captureDeviceSlots = saveData.captureDevice.Select(slot => new ItemSlot(slot)).ToList();
        grimoireSlots = saveData.grimoires.Select(slot => new ItemSlot(slot)).ToList();

        allSlots = new List<List<ItemSlot>>() { slots, captureDeviceSlots, grimoireSlots };

        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot()
    {

    }

    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.name);
        count = saveData.count;
    }

    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData() 
        { 
            name = item.name,
            count = count
        };

        return saveData;
    }

    public ItemBase Item
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
}

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
    public List<ItemSaveData> captureDevice;
    public List<ItemSaveData> grimoires;
}
