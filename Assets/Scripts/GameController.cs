using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused, Evolution }

public class GameController : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] DungeonArea dungeonArea;

    TrainerController trainer;

    GameState state;
    GameState prevState;
    GameState stateBeforeEvolution;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    MenuController menuController;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        // MOUSE DISABLED
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        MonsterDB.Init();
        MoveDB.Init();
        ConditionsDB.Init();
        ItemDB.Init();
        QuestDB.Init();
    }

    private void Start()
    {

        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnDialogFinished += () =>
        {
            if (state == GameState.Dialog)
                state = prevState;
        };

        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += OnMenuSelected;

        EvolutionManager.i.OnStartEvolution += () => 
        {
            stateBeforeEvolution = state;
            state = GameState.Evolution; 
        };

        EvolutionManager.i.OnCompleteEvolution += () =>
        {
            partyScreen.SetPartyData();
            state = stateBeforeEvolution;
        };
    }

    public void PauseGame(bool pause)
    {

        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
        }

    }

    public void StartSearching()
    {
        dungeonArea.searching = true;
        StartCoroutine(dungeonArea.Search());
    }

    public IEnumerator StartBattleCorutine(Monster wildMonster)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonsterParty>();

        var wildMonsterCopy = new Monster(wildMonster.Base, wildMonster.Level);

        yield return battleSystem.StartBattleCorutine(playerParty, wildMonsterCopy);
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildMonster = CurrentScene.GetComponent<MapArea>().GetRandomWildMonster();

        var wildMonsterCopy = new Monster(wildMonster.Base, wildMonster.Level);

        battleSystem.StartBattle(playerParty, wildMonsterCopy);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerController.GetComponent<MonsterParty>();
        var trainerParty = trainer.GetComponent<MonsterParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void OnEnterTrainerView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        partyScreen.SetPartyData();

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        var playerParty = playerController.GetComponent<MonsterParty>();
        StartCoroutine(playerParty.CheckForEvolutions());
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {

            /*playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }*/

        }
        else if (state == GameState.Battle)
        {

            battleSystem.HandleUpdate();

        }
        else if (state == GameState.Dialog)
        {

            DialogManager.Instance.HandleUpdate();

        }
        else if (state == GameState.Menu)
        {
            //menuController.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                //TODO: Schermata Informazioni
            };

            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            inventoryUI.HandleUpdate(onBack);
        }
        
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            //Party
            partyScreen.gameObject.SetActive(true);
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            //Zaino
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            //Salva
            SavingSystem.i.Save("saveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            //Carica
            SavingSystem.i.Load("saveSlot1");
            state = GameState.FreeRoam;
        }
    }

    public GameState State => state;
}
