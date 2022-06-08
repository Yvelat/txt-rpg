using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Crea nuovo Item Evoluzione")]
public class EvolutionItem : ItemBase
{
    public override bool Use(Monster monster)
    {
        return true;
    }
}
