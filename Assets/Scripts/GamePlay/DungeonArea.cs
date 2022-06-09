using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonArea : MonoBehaviour
{
    [SerializeField] MapArea area;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject transition;

    Dungeon dungeon;
    Animator transitionAnim;

    public bool searching = false;
    bool eventOccur = false;
    bool transitioning = false;

    private void OnEnable()
    {
        transition.SetActive(true);
    }

    private void OnDisable()
    {
        transition.SetActive(false);
    }

    private void Start()
    {
        transitionAnim = transition.GetComponent<Animator>();
        searching = false;
        eventOccur = false;
        infoText.text = "";
        buttons.SetActive(true);
    }

    public void SetDungeon(Dungeon dg)
    {
        dungeon = dg;
        InitializeAreaWildEncounters();
    }

    public void InitializeAreaWildEncounters()
    {
        area.SetEncounterList(dungeon.WildEncounters, dungeon.RareWildMonsters);
    }

    public void HandleUpdate()
    {

    }

    public IEnumerator Search()
    {

        if (searching)
        {
            buttons.SetActive(false);
            float randomWait = Random.Range(3, 6);
            Debug.Log($"sarted: {randomWait}");
            yield return new WaitForSeconds(randomWait);
            yield return CheckEvent();
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => eventOccur == false);
            buttons.SetActive(true);
            searching = false;
                
        }
        
    }

    public void StartSearch()
    {
        if(!searching)
            GameController.Instance.StartSearching();
    }

    public void BackToMenu()
    {
        if(!searching && !eventOccur)
        {
            GameController.Instance.OpenMenu();
            gameObject.SetActive(false);
        }
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
            transitioning = true;
            Debug.Log("Monster encounter");
            var monster = area.GetRandomWildMonster();
            Debug.Log(monster.ToString());
            yield return TransitionIN();
            yield return new WaitUntil(() => transitioning == false);
            yield return GameController.Instance.StartBattleCorutine(monster);
            transitionAnim.Play("idle");
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

    IEnumerator TransitionIN()
    {
        transitionAnim.Play("FadeIN");
        yield return new WaitForSeconds(1f);
        transitioning = false;
    }

    void CheckRareMonsterEncounter()
    {
        int random = Random.Range(1, 101);

        Debug.Log("RandomEncounter: " + random);

        if(random >= 0 && random <= 95)
        {
            Debug.Log("Monster encounter");
        }
        else
        {
            Debug.Log("Rare Monster encounter");
        }
    }

}
