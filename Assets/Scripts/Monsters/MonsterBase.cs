using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Crea nuovo Monster")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] int id;
    [SerializeField] string monsterName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite battleIcon;

    [SerializeField] MonsterType type;
    [SerializeField] Rarity rarity;

    //Statistiche Base
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;
    [SerializeField] int stamina;

    [SerializeField] int xpYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int catchRate = 255;

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> lernableByItems;

    [SerializeField] List<Evolution> evolutions;

    [Header("DropTable")]
    [SerializeField] Vector2Int coinsRange;
    [SerializeField] Vector2Int gemsRange;
    [SerializeField] Vector2Int dropNumber;
    [SerializeField] List<Drop> drops;

    [HideInInspector]
    [SerializeField] int totalChance = 0;

    public static int MaxNumOfMoves { get; set; } = 4;

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

    public DropTable GetDrops(int level)
    {
        int droprange = dropNumber.y == 0 ? dropNumber.x : Random.Range(dropNumber.x, dropNumber.y + 1);

        DropTable dropsToGive = new DropTable();

        dropsToGive.coins = CalculateCoinsToGive(level);
        dropsToGive.gems = CalculateGemsToGive();

        List<DropTableElement> dropGetted = new List<DropTableElement>();

        for (int i = 0; i < droprange; i++)
        {
            DropTableElement randomDrop = GetRandomDrop();

            DropTableElement dropEqual = null;

            if (dropGetted.Count > 0)
                dropEqual = dropGetted.First(dr => dr.drop.Item.Name == randomDrop.drop.Item.Name);

            if(dropEqual != null)
            {
                dropGetted.Remove(dropEqual);

                dropEqual.count += randomDrop.count;

                dropGetted.Add(dropEqual);
            }
            else
            {
                dropGetted.Add(randomDrop);
            }
        }

        dropsToGive.dropList = dropGetted;

        return dropsToGive;

    }

    int CalculateCoinsToGive(int level)
    {
        int coins = coinsRange.y == 0 ? coinsRange.x : Random.Range(coinsRange.x, coinsRange.y + 1);

        //TODO: better coin calculation
        return ((coins * 10) * level) / 20;
    }

    int CalculateGemsToGive()
    {
        return gemsRange.y == 0 ? gemsRange.x : Random.Range(gemsRange.x, gemsRange.y + 1);
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

    public int GetXPForLevel(int level)
    {
        if (growthRate == GrowthRate.Erratic)
        {
            return GetErraticXP(level);
        }
        else if (growthRate == GrowthRate.Fast)
        {
            return (4 * (level * level * level)) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return 6 * (level * level * level) / 5 - 15 * (level * level) + 100 * level - 140;
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return 5 * (level * level * level) / 4;
        }
        else if (growthRate == GrowthRate.Fluctuating)
        {
            return GetFluctuatingXP(level);
        }

        return -1;
    }

    public int GetErraticXP(int level)
    {
        if (level <= 50)
        {
            return Mathf.FloorToInt((Mathf.Pow(level, 3) * (100 - level)) / 50);
        }
        else if (level >= 50 && level <= 68)
        {
            return Mathf.FloorToInt((Mathf.Pow(level, 3) * (150 - level)) / 100);
        }
        else if (level >= 68 && level <= 98)
        {
            return Mathf.FloorToInt((Mathf.Pow(level, 3) * ((1911 - (10 * level)) / 3)) / 500);
        }
        else
        {
            return Mathf.FloorToInt((Mathf.Pow(level, 3) * (160 - level)) / 100);
        }
    }

    public int GetFluctuatingXP(int level)
    {
        if (level <= 15)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor((level + 1) / 3) + 24) / 50));
        }
        else if (level >= 15 && level <= 36)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 14) / 50));
        }
        else
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor(level / 2) + 32) / 50));
        }
    }

    //Proprietà --Inizio--
    public string Name
    {
        get { return monsterName; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public MonsterType Type
    {
        get { return type; }
    }

    public Rarity Rarity
    {
        get { return rarity; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public int Stamina
    {
        get { return stamina; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public Sprite BattleIcon
    {
        get { return battleIcon; }
    }

    public int ID
    {
        get { return id; }
    }

    public List<MoveBase> LernableByItems => lernableByItems;
    public List<Evolution> Evolutions => evolutions;

    public int ChatchRate => catchRate;
    public int XpYield => xpYield;
    public GrowthRate GrowthRate => growthRate;
    //Proprietà --Fine--
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

[System.Serializable]
public class Evolution
{
    [SerializeField] MonsterBase evolvesInto;
    [SerializeField] int requiredLevel;
    [SerializeField] EvolutionItem requiredItem;

    public MonsterBase EvolvesInto => evolvesInto;
    public int RequiredLevel => requiredLevel;
    public EvolutionItem RequiredItem => requiredItem;
}

public enum MonsterType
{
    None,
    Normale,
    Fuoco,
    Acqua,
    Erba,
    Elettro,
    Psicho,
    Spettro,
    Drago,
    Roccia,
    Aria

}
public enum GrowthRate
{
    Erratic, Fast, MediumFast, MediumSlow, Slow, Fluctuating
}

public enum Stat
{
    Attack,
    Defense,
    Speed,

    //Statistiche di  battaglia (nascoste)
    Accuracy,
    Evasion
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    UltraRare,
    Epic,
    Legendary
}

[System.Serializable]
public class Drop
{
    [SerializeField] ItemBase item;
    public Vector2Int amount;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }

    public ItemBase Item => item;
}

[System.Serializable]
public class DropTable
{
    public int coins;
    public int gems;
    public List<DropTableElement> dropList;
}

public class DropTableElement
{
    public Drop drop;
    public int count;
}

public class TypeChart
{
    static float[][] chart =
    {
            //                  NOR  FIR  ACQ  ERB  ELT  PSY  SPT  DRG  ROC  AIR
            /*NOR*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*FIR*/new float[]{ 1f , 1f , 1f , 2f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*ACQ*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*ERB*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*ELT*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*PSY*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*SPT*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*DRG*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*ROC*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },
            /*AIR*/new float[]{ 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f },

        };

    public static float GetEffectiveness(MonsterType attackType, MonsterType defenseType)
    {

        if (attackType == MonsterType.None || defenseType == MonsterType.None) return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];

    }

}