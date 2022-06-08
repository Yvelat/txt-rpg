using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Crea nuovo grimorio")]
public class GrimoireItem : ItemBase
{

    [SerializeField] MoveBase move;
    [SerializeField] bool isAncient;

    public override bool Use(Monster monster)
    {
        return monster.HasMove(move);
    }

    public bool CanBeTaught(Monster monster)
    {
        return monster.Base.LernableByItems.Contains(move);
    }

    public override string Description => $"Insegna {move.Name} ad un Monster";

    public override bool CanUseInBattle => false;
    public override bool IsReusable => isAncient;

    public MoveBase Move => move;
    public bool IsAncient => isAncient;
}
