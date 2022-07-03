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
    [SerializeField] GameObject menuChoices;
    [SerializeField] TreasureUi treasureUI;
    [SerializeField] QuestController questController;
    [SerializeField] PlayerLevelUpUI playerLevelUpUI;

    TrainerController trainer;

    GameState state;
    GameState prevState;
    GameState stateBeforeEvolution;

    bool isBossBattle = false;
    QuestBase bossQuest;

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

        /*menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };*/

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
        if (playerController.HasEnergy())
        {
            dungeonArea.searching = true;
            playerController.UseEnergy();
            ProgressAllQuestOfType(QuestType.Step, 1);
            StartCoroutine(dungeonArea.Search());
        }
        
    }

    public void AddStepToPlayer()
    {
        playerController.AddStep();
    }

    public int GetPlayerStepCounter()
    {
        return playerController.stepCounter;
    }

    public void OpenMenu()
    {
        menuChoices.SetActive(true);
    }

    public IEnumerator StartBattleCorutine(Monster wildMonster)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        isBossBattle = false;

        var playerParty = playerController.GetComponent<MonsterParty>();

        var wildMonsterCopy = new Monster(wildMonster.Base, wildMonster.Level);

        yield return battleSystem.StartBattleCorutine(playerParty, wildMonsterCopy);
    }

    public IEnumerator StartBossBattleCorutine(Monster boss, DropTable table, QuestBase dgQuest)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        isBossBattle = true;

        bossQuest = dgQuest;

        var playerParty = playerController.GetComponent<MonsterParty>();

        var wildMonsterCopy = new Monster(boss.Base, boss.Level);

        yield return battleSystem.StartBossBattleCorutine(playerParty, wildMonsterCopy, table);
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

    public IEnumerator StartTrainerBattle(Trainer trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        isBossBattle = false;

        //this.trainer = trainer;
        var playerParty = playerController.GetComponent<MonsterParty>();
        //var trainerParty = trainer.GetComponent<MonsterParty>();

        battleSystem.StartTrainerBattle(playerParty, trainer);
        yield return null;
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

        if (isBossBattle)
        {
            if (won)
            {
                AddStepToPlayer();

                if(bossQuest != null)
                    ProgressSpecificQuest(bossQuest);

                bossQuest = null;
                isBossBattle = false;
            }
            else
            {
                isBossBattle = false;
            }
        }

        partyScreen.SetPartyData();

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        dungeonArea.BattleOver();

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
                menuChoices.gameObject.SetActive(true);
                state = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                menuChoices.gameObject.SetActive(true);
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

    public void SetStateToParty()
    {
        state = GameState.PartyScreen;
    }

    public void SetStateToBag()
    {
        state = GameState.Bag;
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

    public void AddCoinsToPlayer(int amount)
    {
        int coins = playerController.Coins;

        coins += amount;

        playerController.SetCoins(coins);
    }

    public void AddGemsToPlayer(int amount)
    {
        int gems = playerController.Gems;

        gems += amount;

        playerController.SetGems(gems);
    }

    public void AddXpToPlayer(int xp)
    {
        playerController.AddXp(xp);
    }

    public bool CheckIfPlayerCanLevelUp()
    {
        return playerController.CanLevelUp();
    }

    public void ExecuteLevelUp()
    {
        playerLevelUpUI.gameObject.SetActive(true);
        playerController.LevelUp();
    }

    public void OpenTreasureUI(int value, TreasureType type)
    {
        treasureUI.gameObject.SetActive(true);
        treasureUI.SetData(value, type);
    }

    public void OpenTreasureUI(DropTableElement drop)
    {
        treasureUI.gameObject.SetActive(true);
        treasureUI.SetData(drop);
    }

    public void ProgressAllQuestOfType(QuestType type, int amount)
    {
        StartCoroutine(questController.ProgressAllQuestOfType(type, amount));
    }

    public void ProgressSpecificQuest(QuestBase quest)
    {
        questController.ProgressSpecificQuest(quest, 1);
    }

    public QuestList GetQuestListComponent()
    {
        return playerController.GetComponent<QuestList>();
    }

    public Skin GetPlayerSkin()
    {
        return playerController.skin;
    }

    public void ShowLevelUpUI(float prevXp, float xp, int prevLvl, int lvl, int prevEnergy, int energy)
    {
        playerLevelUpUI.SetData(prevXp, xp, prevLvl, lvl, prevEnergy, energy);
    }

    public GameState State => state;
}
