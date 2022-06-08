using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Monster
{
    [SerializeField] MonsterBase _base;
    [SerializeField] int level;

    public Monster(MonsterBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;

        Init();
    }

    public MonsterBase Base
    {
        get
        {
            return _base;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }

    }

    public int XP { get; set; }
    public int HP { get; set; }
    public int Stamina { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }

    public Queue<string> StatusChanges { get; private set; }

    public event System.Action OnStatusChange;
    public event System.Action OnHPChange;
    public event System.Action OnStaminaChange;

    public void Init()
    {
        //Generazione mosse in base al livello
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {

            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            if (Moves.Count >= MonsterBase.MaxNumOfMoves) break;

        }

        XP = Base.GetXPForLevel(level);

        CalculateStats();
        HP = MaxHp;
        Stamina = MaxStamina;

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    public Monster(MonsterSaveData saveData)
    {
        _base = MonsterDB.GetObjectByName(saveData.name);
        HP = saveData.hp;
        Stamina = saveData.stamina;
        level = saveData.level;
        XP = saveData.xp;

        if (saveData.statusId != null)
        {
            Status = ConditionsDB.Conditions[saveData.statusId.Value];
        }
        else
        {
            Status = null;
        }

        Moves = saveData.moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;

    }

    public MonsterSaveData GetSaveData()
    {
        var saveData = new MonsterSaveData()
        {
            name = Base.name,
            hp = HP,
            stamina = Stamina,
            level = Level,
            xp = XP,
            statusId = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();

        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        int oldMaxHP = MaxHp;

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;
        MaxStamina = Mathf.FloorToInt(Base.Stamina + (Level * 2f)) + 10;

        Stamina = MaxStamina;

        if (oldMaxHP != 0)
            HP += MaxHp - oldMaxHP;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0}
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //Potenziamento mosse
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoost(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"L'{stat} di {Base.Name} aumenta!");
            else
                StatusChanges.Enqueue($"L'{stat} di {Base.Name} diminuisce!");

            Debug.Log($"{stat} è stato boostato a: {StatBoosts[stat]}");
        }
    }

    public bool CheckForLevelUp()
    {
        if (XP >= Base.GetXPForLevel(level + 1))
        {
            ++level;
            CalculateStats();
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count >= MonsterBase.MaxNumOfMoves)
            return;

        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredLevel <= level && e.RequiredLevel != 0);
    }

    public Evolution CheckForEvolution(ItemBase item)
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredItem == item);
    }

    public void Evolve(Evolution evolution)
    {
        _base = evolution.EvolvesInto;
        CalculateStats();
    }

    public void Heal()
    {
        HP = MaxHp;
        Stamina = MaxStamina;

        OnHPChange?.Invoke();
        OnStaminaChange?.Invoke();

        CureStatus();
    }

    //Statistiche Effettive --Inizio--
    public int MaxHp { get; private set; }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxStamina { get; private set; }
    //Statistiche Effettive --Fine--

    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f) critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type);

        var damageDetails = new DamageDetails()
        {
            Fainted = false,
            TypeEffectiveness = type,
            Critical = critical
        };

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        DecreaseHP(damage);

        return damageDetails;
    }

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        OnHPChange?.Invoke();
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHPChange?.Invoke();
    }

    public void IncreaseStamina(int amount)
    {
        Stamina = Mathf.Clamp(Stamina + amount, 0, MaxStamina);
        OnStaminaChange?.Invoke();
    }

    public void DecreaseStamina(int amount)
    {
        Stamina = Mathf.Clamp(Stamina - amount, 0, MaxStamina);
        OnStaminaChange?.Invoke();
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChange?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChange?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(1, Moves.Count);
        return Moves[r-1];
    }

    public bool OnBeforeMove()
    {
        var canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

[System.Serializable]
public class MonsterSaveData
{
    public string name;
    public int hp;
    public int stamina;
    public int level;
    public int xp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}