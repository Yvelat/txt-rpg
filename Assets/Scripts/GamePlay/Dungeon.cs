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
    [SerializeField] int totalChanceCommon = 0;
    [HideInInspector]
    [SerializeField] int totalChanceRare = 0;

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
