using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    List<MonsterEncounterRecord> wildMonsters;
    List<MonsterEncounterRecord> rareWildMonsters;
    List<Drop> drops;

    public void SetData(Dungeon dungeon)
    {
        wildMonsters = dungeon.WildEncounters;
        rareWildMonsters = dungeon.RareWildMonsters;
        drops = dungeon.Drops;
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
        // From UltraRare To Legendary?

        int randVal = Random.Range(1, 101);

        var monsterRecord = rareWildMonsters.First(m => randVal >= m.chanceLower && randVal <= m.chanceUpper);

        var levelRange = monsterRecord.levelRange;

        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildMonster = new Monster(monsterRecord.monster, level);
        wildMonster.Init();
        return wildMonster;
    }

    public DropTableElement GetRandomDrop()
    {
        //From Common To Rare

        int randVal = Random.Range(1, 101);

        DropTableElement dropToGive = new DropTableElement();

        var drop = drops.First(m => randVal >= m.chanceLower && randVal <= m.chanceUpper);

        var numRange = drop.amount;

        int count = numRange.y == 0 ? numRange.x : Random.Range(numRange.x, numRange.y + 1);

        dropToGive.drop = drop;
        dropToGive.count = count;

        return dropToGive;
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
