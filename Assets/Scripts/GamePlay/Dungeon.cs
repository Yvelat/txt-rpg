using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "Dungeons/Crea nuovo Dungeon")]
public class Dungeon : ScriptableObject
{
    [SerializeField] string dungeonName;
    [SerializeField] int stepToComplete;
    [SerializeField] int stepToUnlock;

    [Header("Boss")]
    [SerializeField] MonsterBase monsterBoss;
    [SerializeField] int bossLevel;
    [SerializeField] DropTable fixedDrop;
    [SerializeField] QuestBase bossQuest;

    [Header("Monster Encounters")]
    [SerializeField] List<MonsterEncounterRecord> wildMonsters;
    [Header("Rare Monster Encounters")]
    [SerializeField] List<MonsterEncounterRecord> rareWildMonsters;
    [Header("Trainers")]
    [SerializeField] List<Trainer> trainers;
    [Header("DropsWalking")]
    [SerializeField] List<Drop> drops;

    [HideInInspector]
    [SerializeField] int totalChanceCommon = 0;
    [HideInInspector]
    [SerializeField] int totalChanceRare = 0;
    [HideInInspector]
    [SerializeField] int totalChanceDrops = 0;

    private void OnValidate()
    {
        totalChanceCommon = 0;
        foreach (var record in wildMonsters)
        {
            record.chanceLower = totalChanceCommon;
            record.chanceUpper = totalChanceCommon + record.chancePercentage;

            totalChanceCommon = totalChanceCommon + record.chancePercentage;
        }

        totalChanceRare = 0;
        foreach (var record in rareWildMonsters)
        {
            record.chanceLower = totalChanceRare;
            record.chanceUpper = totalChanceRare + record.chancePercentage;

            totalChanceRare = totalChanceRare + record.chancePercentage;
        }

        totalChanceDrops = 0;
        foreach (var record in drops)
        {
            record.chanceLower = totalChanceDrops;
            record.chanceUpper = totalChanceDrops + record.chancePercentage;

            totalChanceDrops = totalChanceDrops + record.chancePercentage;
        }
    }

    public MonsterBase Boss
    {
        get { return monsterBoss; }
    }

    public int BossLevel
    {
        get { return bossLevel; }
    }

    public DropTable BossDrop
    {
        get { return fixedDrop; }
    }

    public List<MonsterEncounterRecord> WildEncounters
    {
        get { return wildMonsters; }
    }

    public List<MonsterEncounterRecord> RareWildMonsters
    {
        get { return rareWildMonsters; }
    }

    public List<Trainer> Trainers
    {
        get { return trainers; }
    }

    public List<Drop> Drops
    {
        get { return drops; }
    }

    public string Name
    {
        get { return dungeonName; }
    }

    public int StepToComplete
    {
        get { return stepToComplete; }
    }

    public int StepToUnlock
    {
        get { return stepToUnlock; }
    }

    public QuestBase BossQuest
    {
        get { return bossQuest; }
    }
}
