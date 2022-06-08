using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Crea nuovo item di cattura")]
public class CaptureDeviceItem : ItemBase
{
    [SerializeField] float catchRateModifier = 1;

    public override bool Use(Monster monster)
    {
        return true;
    }

    public override bool CanUseOutsideBattle => false;

    public float CatchRateModifier => catchRateModifier;

}
