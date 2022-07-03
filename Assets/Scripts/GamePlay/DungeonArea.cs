using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonArea : MonoBehaviour
{
    [SerializeField] MapArea area;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject bossButton;
    [SerializeField] PlayerLevelUpUI playerLevelUpUI;

    [Header("Transitions")]
    [SerializeField] GameObject transition;
    [SerializeField] GameObject preTransitionCommon;
    [SerializeField] GameObject preTransitionRare;

    [Header("Audio")]
    [SerializeField] SingleAudioManager menuAudio;
    [SerializeField] SingleAudioManager adventureAudio;
    [SerializeField] SingleAudioManager commonEncounterAudio;
    [SerializeField] SingleAudioManager rareEncounterAudio;

    [Header("PlayerSprite")]
    [SerializeField] SpriteAnimator animator;

    Dungeon dungeon;
    Animator transitionAnim;
    Inventory inventory;

    public bool searching = false;
    bool eventOccur = false;
    bool transitioning = false;

    private void OnEnable()
    {
        transition.SetActive(true);
        menuAudio.Stop();
        menuAudio.gameObject.SetActive(false);
        adventureAudio.gameObject.SetActive(true);
        adventureAudio.ResetAndPlay();
    }

    private void OnDisable()
    {
        transition.SetActive(false);
        adventureAudio.Stop();
        adventureAudio.gameObject.SetActive(false);
        menuAudio.gameObject.SetActive(true);
        menuAudio.ResetAndPlay();
    }

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    private void Start()
    {
        transitionAnim = transition.GetComponent<Animator>();
        searching = false;
        eventOccur = false;
        infoText.text = "";
        animator.SetData(GameController.Instance.GetPlayerSkin().Sprites);
        buttons.SetActive(true);
        bossButton.SetActive(false);
    }

    public void SetDungeon(Dungeon dg)
    {
        dungeon = dg;
        InitializeMapArea();
    }

    public void InitializeMapArea()
    {
        area.SetData(dungeon);
    }

    public void HandleUpdate()
    {

    }

    public IEnumerator Search()
    {

        if (searching)
        {
            buttons.SetActive(false);
            infoText.text = "";
            float randomWait = Random.Range(3, 6);
            Debug.Log($"sarted: {randomWait}");
            yield return new WaitForSeconds(randomWait);
            yield return CheckEvent();
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => eventOccur == false);
            buttons.SetActive(true);
            if(CheckIfCanBossBattle())
                bossButton.SetActive(true);
            else
                bossButton.SetActive(false);
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

    public void StartBossBattle()
    {
        buttons.SetActive(false);
        infoText.text = "";
        StartCoroutine(BossBattle());
    }

    public bool CheckIfCanBossBattle()
    {
        if(GameController.Instance.GetPlayerStepCounter() == dungeon.StepToComplete)
            return true;
        else
            return false;
    }

    public bool CheckIfIsComplete()
    {
        if (GameController.Instance.GetPlayerStepCounter() > dungeon.StepToComplete)
            return true;
        else
            return false;
    }

    IEnumerator BossBattle()
    {
        transitioning = true;
        Debug.Log("Boss encounter");
        var monster = new Monster(dungeon.Boss, dungeon.BossLevel);
        preTransitionRare.SetActive(true);
        adventureAudio.Pause();
        //adventureAudio.gameObject.SetActive(false);
        rareEncounterAudio.gameObject.SetActive(true);
        rareEncounterAudio.ResetAndPlay();
        yield return new WaitForSeconds(6f);
        yield return TransitionIN();
        yield return new WaitUntil(() => transitioning == false);
        preTransitionRare.SetActive(false);
        yield return GameController.Instance.StartBossBattleCorutine(monster, dungeon.BossDrop, dungeon.BossQuest);
        transitionAnim.Play("idle");
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => GameController.Instance.State != GameState.Battle && GameController.Instance.State != GameState.Evolution);
        eventOccur = false;
        buttons.SetActive(true);
        bossButton.SetActive(false);
    }

    IEnumerator CheckEvent()
    {
        eventOccur = true;

        if(!CheckIfCanBossBattle() && !CheckIfIsComplete())
            GameController.Instance.AddStepToPlayer();

        int random = Random.Range(1, 101);

        /* Solo per scopo di Test */
        random = 99;
        /* Solo per scopo di Test */

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
            //oggetto o ?soldi? || ?Soldi tramite oggetto?
            DropTableElement drop = area.GetRandomDrop();
            inventory.AddItem(drop.drop.Item, drop.count);
            Debug.Log($"Object: {drop.drop.Item.Name} x{drop.count}");
            infoText.text = $"Hai trovato {drop.count} {drop.drop.Item.Name}!";
            eventOccur = false;
        }
        else if (random > 45 && random <= 60)
        {
            //trainer
            // TODO: Add Trainer Encounter
            yield return TrainerBattle();
        }
        else if (random > 60 && random <= 100)
        {
            //mostro normale
            yield return CheckRareMonsterEncounter();
        }

        yield return null;
    }

    IEnumerator TransitionIN()
    {
        transitionAnim.Play("FadeIN");
        yield return new WaitForSeconds(1f);
        transitioning = false;
    }

    public void BattleOver()
    {
        commonEncounterAudio.gameObject.SetActive(false);
        rareEncounterAudio.gameObject.SetActive(false);
        adventureAudio.gameObject.SetActive(true);
        adventureAudio.Resume();

        if (GameController.Instance.CheckIfPlayerCanLevelUp())
        {
            //execute and show UI
            StartCoroutine(WaitForLevelUp());
        }
    }

    IEnumerator WaitForLevelUp()
    {
        buttons.gameObject.SetActive(false);
        GameController.Instance.ExecuteLevelUp();
        yield return new WaitUntil(() => playerLevelUpUI.IsClosed == true);
        buttons.gameObject.SetActive(true);
    }

    IEnumerator TrainerBattle()
    {
        transitioning = true;
        Debug.Log("Trainer encounter");
        var trainer = area.GetRandomTrainer();
        preTransitionCommon.SetActive(true);
        adventureAudio.Pause();
        commonEncounterAudio.gameObject.SetActive(true);
        commonEncounterAudio.ResetAndPlay();
        yield return new WaitForSeconds(3f);
        yield return TransitionIN();
        yield return new WaitUntil(() => transitioning == false);
        preTransitionCommon.SetActive(false);
        yield return GameController.Instance.StartTrainerBattle(trainer);
        transitionAnim.Play("idle");
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => GameController.Instance.State != GameState.Battle && GameController.Instance.State != GameState.Evolution);
        eventOccur = false;
    }

    IEnumerator CheckRareMonsterEncounter()
    {
        int random = Random.Range(1, 101);

        /* Solo per scopo di Test */
        random = 99;
        /* Solo per scopo di Test */

        Debug.Log("RandomEncounter: " + random);

        if(random >= 0 && random <= 95)
        {
            // Normal Encounter
            transitioning = true;
            Debug.Log("Monster encounter");
            var monster = area.GetRandomWildMonster();
            preTransitionCommon.SetActive(true);
            adventureAudio.Pause();
            //adventureAudio.gameObject.SetActive(false);
            commonEncounterAudio.gameObject.SetActive(true);
            commonEncounterAudio.ResetAndPlay();
            yield return new WaitForSeconds(3f);
            yield return TransitionIN();
            yield return new WaitUntil(() => transitioning == false);
            preTransitionCommon.SetActive(false);
            yield return GameController.Instance.StartBattleCorutine(monster);
            transitionAnim.Play("idle");
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => GameController.Instance.State != GameState.Battle && GameController.Instance.State != GameState.Evolution);
            eventOccur = false;
        }
        else
        {
            // Rare Encounter
            transitioning = true;
            Debug.Log("Rare Monster encounter");
            var monster = area.GetRandomRareWildMonster();
            preTransitionRare.SetActive(true);
            adventureAudio.Pause();
            //adventureAudio.gameObject.SetActive(false);
            rareEncounterAudio.gameObject.SetActive(true);
            rareEncounterAudio.ResetAndPlay();
            yield return new WaitForSeconds(6f);
            yield return TransitionIN();
            yield return new WaitUntil(() => transitioning == false);
            preTransitionRare.SetActive(false);
            yield return GameController.Instance.StartBattleCorutine(monster);
            transitionAnim.Play("idle");
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => GameController.Instance.State != GameState.Battle && GameController.Instance.State != GameState.Evolution);
            eventOccur = false;
        }
    }

}
