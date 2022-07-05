using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    public event Action OnUpdated;

    public List<Monster> Monsters
    {
        get
        {
            return monsters;
        }
        set
        {
            monsters = value;
            OnUpdated?.Invoke();
        }
    }

    private void Awake()
    {
        foreach (var monster in monsters)
        {
            monster.Init();
        }
    }

    private void Start()
    {

    }

    public Monster GetHealthyMonster()
    {
        return monsters.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddMonster(Monster newMonster)
    {
        if (monsters.Count < 6)
        {
            monsters.Add(newMonster);
            OnUpdated?.Invoke();
        }
        else
        {
            //TODO: Manda il Monster al PC
            gameObject.GetComponent<MonsterBox>().AddMonsterToBox(newMonster);
        }
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach (var monster in monsters)
        {
            var evolution = monster.CheckForEvolution();

            if (evolution != null)
            {
                yield return EvolutionManager.i.Evolve(monster, evolution);
            }

        }

    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }

    public static MonsterParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<MonsterParty>();
    }
}
