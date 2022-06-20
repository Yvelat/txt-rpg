using System.Collections;
using System.Collections.Generic;
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
        totalChance = 0;
        foreach (var record in drops)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;

            totalChance = totalChance + record.chancePercentage;
        }
    }

    public override bool Use(Monster monster)
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

        //TODO: drops

        if(drops.Count > 0 && totalChance == 100)
        {

        }

        return true;
    }

    public override bool CanUseInBattle => false;
}
