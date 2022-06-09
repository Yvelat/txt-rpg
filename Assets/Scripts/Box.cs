using System;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    List<List<BoxSlot>> boxs;

    int boxLimit = 40;

    private void Awake()
    {
        boxs = new List<List<BoxSlot>>();
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "Box1"
    };

    public List<BoxSlot> GetSlotsByCategory(int index)
    {
        return boxs[index];
    }

    public static Box GetBox()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Box>();
    }

    public bool IsPageEmpty(int index)
    {
        if (boxs[index].Count == 0)
            return true;
        else
            return false;
    }

    public bool PageExsist(int index)
    {
        if (boxs.Count < index)
            return false;
        else
            return true;
    }

    public Monster GetMonster(int monsterIndex, int pageIndex)
    {
        var currentSlots = GetSlotsByCategory(pageIndex);

        return currentSlots[monsterIndex].Monster;

    }
}

[Serializable]
public class BoxSlot
{
    [SerializeField] Monster monster;
    [SerializeField] int level;

    public BoxSlot()
    {

    }

    public BoxSlot(ItemSaveData saveData)
    {
        //monster = ItemDB.GetObjectByName(saveData.name);
        level = saveData.count;
    }

    /*public BoxSaveData GetSaveData()
    {
        var saveData = new BoxSaveData()
        {
            name = item.name,
            count = count
        };

        return saveData;
    }*/

    public Monster Monster
    {
        get => monster;
        set => monster = value;
    }
    public int Level
    {
        get => level;
        set => level = value;
    }
}
