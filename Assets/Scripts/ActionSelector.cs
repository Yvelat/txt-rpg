using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector : MonoBehaviour
{
    BattleSystem battleSystem;

    private void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    public void Battle()
    {
        battleSystem.MoveSelection();
    }

    public void Run()
    {
        battleSystem.RunSelected();
    }

    public void Party()
    {
        battleSystem.OpenPartyScreen();
    }

    public void Bag()
    {
        battleSystem.OpenBag();
        gameObject.SetActive(false);
    }
}
