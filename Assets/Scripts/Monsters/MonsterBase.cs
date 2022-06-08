using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Crea nuovo Monster")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] string monsterName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite battleIcon;

    [SerializeField] MonsterType type;

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

    public static int MaxNumOfMoves { get; set; } = 4;

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