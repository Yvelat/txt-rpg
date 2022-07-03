using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    public Dungeon dungeon;

    GameObject dungeonSelector;
    GameObject adventuring;

    public void SetDungeon(Dungeon dg, GameObject selector, GameObject newMenu)
    {
        dungeon = dg;
        dungeonSelector = selector;
        adventuring = newMenu;
        text.text = dungeon.Name;

        if(GameController.Instance.GetPlayerStepCounter() < dungeon.StepToUnlock)
        {
            GetComponent<LeanButton>().interactable = false;
            text.text = "Bloccato";
        }
        else
        {
            GetComponent<LeanButton>().interactable = true;
        }

    }

    public void EnterDungeon()
    {
        adventuring.SetActive(true);
        adventuring.GetComponent<DungeonArea>().SetDungeon(dungeon);
        //GameController.Instance.StartSearching();
        dungeonSelector.SetActive(false);
    }

}
