using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    public string Name => itemName;
    public virtual string Description => description;
    public Sprite Icon => icon;

    public virtual bool Use(Monster monster)
    {
        return false;
    }

    public virtual bool IsReusable => false;
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}
