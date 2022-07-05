using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBox : MonoBehaviour
{
    [SerializeField] int boxPageLength;

    private List<MonsterBoxItem[]> monsters;

    private void Awake()
    {
        monsters = new List<MonsterBoxItem[]>()
        {
            new MonsterBoxItem[boxPageLength],
            new MonsterBoxItem[boxPageLength],
            new MonsterBoxItem[boxPageLength],
            new MonsterBoxItem[boxPageLength],
            new MonsterBoxItem[boxPageLength],
            new MonsterBoxItem[boxPageLength]
        };
    }

    public Vector2Int GetFirstEmptySlot()
    {
        Vector2Int output = new Vector2Int(-1,-1);

        int i = 0;

        foreach (MonsterBoxItem[] monsterBoxSlot in monsters)
        {
            for(int j = 0; j < monsterBoxSlot.Length; j++)
            {
                if(monsterBoxSlot[j] == null)
                {
                    output = new Vector2Int(i,j);
                    break;
                }
            }
            i++;
        }

        return output;
    }

    public void AddMonsterToBox(Monster monster)
    {
        Vector2Int slot = GetFirstEmptySlot();

        if (slot.x < 0 || slot.y < 0) return;

        monsters[slot.x][slot.y] = new MonsterBoxItem(monster.Base, monster.Level, monster.XP, monster.Stamina, monster.HP);

    }

    public void SetMonsterToSlot(Monster monster, Vector2Int slot)
    {
        monsters[slot.x][slot.y] = new MonsterBoxItem(monster.Base, monster.Level, monster.XP, monster.Stamina, monster.HP);
    }

    public MonsterBoxItem[] GetAllMonsterOfPage(int page)
    {
        return monsters[page];
    }

    public Monster GetInitializedMonsterFromSlot(Vector2Int slot)
    {
        MonsterBoxItem monsterBox = monsters[slot.x][slot.y];

        if(monsterBox == null) return null;

        Monster monster = new Monster(monsterBox.monster, monsterBox.level);

        monster.Init();

        monster.HP = monsterBox.hp;
        monster.Stamina = monsterBox.stamina;

        return monster;
    }

    public List<MonsterBoxItem[]> Monsters => monsters;

}

[System.Serializable]
public class MonsterBoxItem
{
    public MonsterBase monster;
    public int level;
    public int xp;
    public int stamina;
    public int hp;

    public MonsterBoxItem(MonsterBase _base, int lvl, int xp, int stamina, int hp)
    {
        monster = _base;
        level = lvl;
        this.xp = xp;
        this.stamina = stamina;
        this.hp = hp;
    }
}
