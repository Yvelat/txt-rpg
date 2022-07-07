using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bestiary : MonoBehaviour
{

    private List<BestiaryElement> bestiary;

    private void Start()
    {
        List<BestiaryElement> allMonsters = new List<BestiaryElement>();

        foreach (var element in Resources.LoadAll<MonsterBase>(""))
        {
            allMonsters.Add(new BestiaryElement(element, BeastState.Unknown));
        }

        bestiary = allMonsters.OrderBy(item => item.Monster.ID).ToList();
    }

    public void SetBeastState(MonsterBase monsterBase, BeastState state)
    {
        int index = bestiary.IndexOf(new BestiaryElement(monsterBase, BeastState.Unknown));

        if (index < 0) return;

        bestiary[index].bState = state; 

    }

    public int GetCountOfMonsterWithState(BeastState state)
    {
        return bestiary.Where(item => item.bState == state).Count();
    }

    public List<BestiaryElement> FullBestiary
    {
        get { return bestiary; }
    }

}

public class BestiaryElement
{
    private MonsterBase monster;
    public BeastState bState;

    public BestiaryElement(MonsterBase _base, BeastState beastState)
    {
        monster = _base;
        bState = beastState;
    }

    public MonsterBase Monster { get { return monster; } }
}

public enum BeastState
{
    Unknown,
    Seen,
    Captured
}
