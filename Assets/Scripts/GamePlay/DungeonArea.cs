using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonArea : MonoBehaviour
{
    [SerializeField] MapArea area;
    [SerializeField] TextMeshProUGUI infoText;

    Dungeon dungeon;

    public bool searching = false;
    bool eventOccur = false;

    public void SetDungeon(Dungeon dg)
    {
        dungeon = dg;
        InitializeAreaWildEncounters();
    }

    public void InitializeAreaWildEncounters()
    {
        area.SetEncounterList(dungeon.WildEncounters);
    }

    public void HandleUpdate()
    {

    }

    public IEnumerator Search()
    {
        while (searching)
        {
            float randomWait = Random.Range(2, 5);
            Debug.Log($"sarted: {randomWait}");
            yield return CheckEvent();
            yield return new WaitForSeconds(randomWait);
            if(eventOccur)
                yield return new WaitUntil(() => eventOccur == false);
        }
        yield return null;
    }

    IEnumerator CheckEvent()
    {
        eventOccur = true;
        int random = Random.Range(1, 101);

        Debug.Log("Random: "+random);

        if(random > 0 && random <= 25)
        {
            //nulla
            Debug.Log("Nulla");
            infoText.text = "Non hai trovato nulla";
            eventOccur = false;
        }
        else if(random > 25 && random <= 45)
        {
            //oggetto
            Debug.Log("Object");
            infoText.text = "Hai trovato un oggetto!";
            eventOccur = false;
        }
        else if (random > 45 && random <= 60)
        {
            //trainer
            Debug.Log("TrainerEncounter");
            eventOccur = false;
        }
        else if (random > 60 && random <= 90)
        {
            //mostro normale
            Debug.Log("Monster encounter");
            var monster = area.GetRandomWildMonster();
            Debug.Log(monster.ToString());
            yield return GameController.Instance.StartBattleCorutine(monster);
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => GameController.Instance.State != GameState.Battle && GameController.Instance.State != GameState.Evolution);
            eventOccur = false;
        }
        else if (random > 90 && random <= 100)
        {
            //mostro raro
            Debug.Log("Rare Monster encounter");
            eventOccur = false;
        }

        yield return null;
    }

}
