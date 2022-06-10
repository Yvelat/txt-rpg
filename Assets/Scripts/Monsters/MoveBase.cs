using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mossa", menuName = "Monster/Crea nuova Mossa")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonsterType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int staminaCost;
    [SerializeField] int priority;

    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaries;
    [SerializeField] MoveTarget target;

    [Header("VisualEffects")]
    [SerializeField] AnimationClip canvasEffect;
    [SerializeField] float canvasEffectDuration;
    [SerializeField] GameObject particleEffect;
    [SerializeField] float particleEffectDuration;


    //Proprietà --Inizio--
    public string Name
    {
        get { return moveName; }
    }

    public string Description
    {
        get { return description; }
    }

    public MonsterType Type
    {
        get { return type; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }
    public int StaminaCost
    {
        get { return staminaCost; }
    }
    public int Priority
    {
        get { return priority; }
    }
    public MoveCategory Category
    {
        get { return category; }
    }
    
    public MoveEffects Effects
    {
        get { return effects; }
    }
    public List<SecondaryEffects> Secondaries
    {
        get { return secondaries; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
    public GameObject ParticleEffect
    {
        get { return particleEffect; }
    }

    public float CanvasEffectDuration
    {
        get { return canvasEffectDuration; }
    }

    public float ParticleEffectDuration
    {
        get { return ParticleEffectDuration; }
    }

    public AnimationClip CanvasEffect
    {
        get { return canvasEffect; }
    }
    //Proprietà --Fine--
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status
    {
        get { return status; }
    }

    public ConditionID VolatileStatus
    {
        get { return volatileStatus; }
    }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance
    {
        get { return chance; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical, Status
}

public enum MoveTarget
{
    Foe, Self
}
