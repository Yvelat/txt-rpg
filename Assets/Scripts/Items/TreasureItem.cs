using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Crea nuovo item Tesoro")]
public class TreasureItem : ItemBase
{
    [Header("Coins")]
    [SerializeField] Vector2Int coinsRange;

    [Header("Gems")]
    [SerializeField] Vector2Int gemRange;

    [Header("Drop")]
    [SerializeField] List<Drop> drops;

    [HideInInspector]
    [SerializeField] int totalChance = 0;

    private void OnValidate()
    {
        //TODO: aggiungere controllo percentuale?
        totalChance = 0;
        foreach (var record in drops)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;

            totalChance = totalChance + record.chancePercentage;
        }
    }

    public override bool Use(Monster monster, Inventory inv)
    {
        if(monster != null) return false;

        if(coinsRange.x != 0 || coinsRange.y != 0)
        {

            int coins = coinsRange.y == 0 ? coinsRange.x : Random.Range(coinsRange.x, coinsRange.y + 1);

            GameController.Instance.AddCoinsToPlayer(coins);

            return true;
        }

        if (gemRange.x != 0 || gemRange.y != 0)
        {

            int gems = gemRange.y == 0 ? gemRange.x : Random.Range(gemRange.x, gemRange.y + 1);

            GameController.Instance.AddGemsToPlayer(gems);

            return true;

        }

        if(drops.Count > 0 && totalChance == 100)
        {
            DropTableElement drop = GetRandomDrop();

            inv.AddItem(drop.drop.Item, drop.count);

            return true;
        }

        return true;
    }

    DropTableElement GetRandomDrop()
    {
        DropTableElement dropElement = new DropTableElement();

        int randVal = Random.Range(1, 101);

        Drop drop = drops.First(m => randVal >= m.chanceLower && randVal <= m.chanceUpper);

        int count = drop.amount.y == 0 ? drop.amount.x : Random.Range(drop.amount.x, drop.amount.y + 1);

        dropElement.drop = drop;
        dropElement.count = count;

        return dropElement;
    }

    public override bool CanUseInBattle => false;
}
