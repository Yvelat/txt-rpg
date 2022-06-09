using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    List<MonsterEncounterRecord> wildMonsters;
    List<MonsterEncounterRecord> rareWildMonsters;

    public void SetEncounterList(List<MonsterEncounterRecord> wild, List<MonsterEncounterRecord> rareWild)
    {
        wildMonsters = wild;
        rareWildMonsters = rareWild;
    }

    public Monster GetRandomWildMonster()
    {

        int randVal = Random.Range(1, 101);

        var monsterRecord = wildMonsters.First(m => randVal >= m.chanceLower && randVal <= m.chanceUpper);

        var levelRange = monsterRecord.levelRange;

        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildMonster = new Monster(monsterRecord.monster, level);
        wildMonster.Init();
        return wildMonster;
    }

    public Monster GetRandomRareWildMonster()
    {

        int randVal = Random.Range(1, 101);

        var monsterRecord = rareWildMonsters.First(m => randVal >= m.chanceLower && randVal <= m.chanceUpper);

        var levelRange = monsterRecord.levelRange;

        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildMonster = new Monster(monsterRecord.monster, level);
        wildMonster.Init();
        return wildMonster;
    }

}

[System.Serializable]
public class MonsterEncounterRecord
{
    public MonsterBase monster;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
