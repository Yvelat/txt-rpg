using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "Dungeons/Crea nuovo Dungeon")]
public class Dungeon : ScriptableObject
{
    [SerializeField] string dungeonName;

    [Header("Monster Encounters")]
    [SerializeField] List<MonsterEncounterRecord> wildMonsters;
    [Header("Rare Monster Encounters")]
    [SerializeField] List<MonsterEncounterRecord> rareWildMonsters;

    [HideInInspector]
    [SerializeField] int totalChance = 0;

    private void OnValidate()
    {
        totalChance = 0;
        foreach (var record in wildMonsters)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePercentage;

            totalChance = totalChance + record.chancePercentage;
        }
    }

    public List<MonsterEncounterRecord> WildEncounters
    {
        get { return wildMonsters; }
    }

    public List<MonsterEncounterRecord> RareWildMonsters
    {
        get { return rareWildMonsters; }
    }

    public string Name
    {
        get { return dungeonName; }
    }
}
