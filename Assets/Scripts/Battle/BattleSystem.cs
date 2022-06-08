using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, Bag, PartyScreen, AboutToUse, MoveToForget, BattleOver }
public enum BattleAction { Move, SwitchMonster, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject captureDevice;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    [Header("CaptureDevice")]
    [SerializeField] Transform captureStartPosition;
    [SerializeField] Transform captureEndPosition;
    [SerializeField] Transform captureDeviceHost;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    bool aboutToUseChoice = true;

    MonsterParty playerParty;
    MonsterParty trainerParty;
    Monster wildMonster;

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttempts;
    MoveBase moveToLearn;

    public void StartBattle(MonsterParty playerParty, Monster wildMonster)
    {
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;
        player = playerParty.GetComponent<PlayerController>();
        isTrainerBattle = false;

        StartCoroutine(SetupBattle());
    }

    public IEnumerator StartBattleCorutine(MonsterParty playerParty, Monster wildMonster)
    {
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;
        player = playerParty.GetComponent<PlayerController>();
        isTrainerBattle = false;

        yield return SetupBattle();
    }

    public void StartTrainerBattle(MonsterParty playerParty, MonsterParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            //setup battaglia con Monster selvatico
            playerUnit.Setup(playerParty.GetHealthyMonster());
            enemyUnit.Setup(wildMonster);

            dialogBox.SetMoveNames(playerUnit.Monster.Moves);
            yield return dialogBox.TypeDialog($"È apparso un {enemyUnit.Monster.Base.Name} selvatico!");
        }
        else
        {
            //setup battaglia con allenatore
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            //playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} vuole combattere!");

            //Invia il primo Monster dell'allenatore
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyMonster = trainerParty.GetHealthyMonster();
            enemyUnit.Setup(enemyMonster);
            yield return dialogBox.TypeDialog($"{trainer.Name} manda in campo {enemyMonster.Base.Name}!");

            //Invia il primo Monster del giocatore
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerMonster = playerParty.GetHealthyMonster();
            playerUnit.Setup(playerMonster);
            yield return dialogBox.TypeDialog($"Vai {playerMonster.Base.Name}!");
            dialogBox.SetMoveNames(playerUnit.Monster.Moves);
        }

        escapeAttempts = 0;
        partyScreen.Init();
        ActionSelection();

    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Monsters.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Seleziona un'azione");
        dialogBox.EnableActionSelector(true);
    }

    public void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
    }

    public void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    public void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    public void RunSelected()
    {
        StartCoroutine(RunTurns(BattleAction.Run));
    }

    IEnumerator AboutToUse(Monster newMonster)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} manderà in campo un {newMonster.Base.Name}. Vuoi cambiare Monster?");

        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    IEnumerator ChooseMoveToForget(Monster monster, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog("Scegli una mossa da dimenticare");

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(monster.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Monster.CurrentMove = playerUnit.Monster.Moves[currentMove];
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

            int playerMovePriority = playerUnit.Monster.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Monster.CurrentMove.Base.Priority;

            //Controllo su quale entità fa la prima mossa
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if(enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondMonster = secondUnit.Monster;

            //Prima Azione
            yield return RunMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondMonster.HP > 0)
            {
                //Seconda azione
                yield return RunMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchMonster)
            {
                dialogBox.EnableActionSelector(false);
                var selectedMonster = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchMonster(selectedMonster);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                dialogBox.EnableActionSelector(false);
                yield return TryToEscape();
            }

            //Turno Dell'avversario
            var enemyMove = enemyUnit.Monster.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Monster.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Monster);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Monster);

        if(move.StaminaCost > sourceUnit.Monster.Stamina)
        {
            
            Debug.Log("Pre Stamina: " + sourceUnit.Monster.Stamina);

            sourceUnit.Monster.IncreaseStamina(10);

            yield return sourceUnit.Hud.WaitForStaminaUpdate();

            Debug.Log("After Stamina: " + (sourceUnit.Monster.Stamina));

            yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} è stanco");
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} usa {move.Base.Name}");

            if (CheckIfMoveHits(move, sourceUnit.Monster, targetUnit.Monster))
            {

                Debug.Log("Pre Stamina: " + sourceUnit.Monster.Stamina);
                sourceUnit.Monster.DecreaseStamina(move.StaminaCost);
                Debug.Log("After Stamina: " + sourceUnit.Monster.Stamina);

                yield return sourceUnit.Hud.WaitForStaminaUpdate();

                if (!sourceUnit.IsPlayerUnit)
                {
                    sourceUnit.PlayAttackAnimation();
                    yield return new WaitForSeconds(1f);
                }

                targetUnit.PlayHitAnimation();

                if (move.Base.Category == MoveCategory.Status)
                {
                    yield return RunMoveEffects(move.Base.Effects, sourceUnit.Monster, targetUnit.Monster, move.Base.Target);
                }
                else
                {
                    var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
                    yield return targetUnit.Hud.WaitForHPUpdate();
                    yield return ShowDamageDetails(damageDetails);
                }

                if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Monster.HP > 0)
                {
                    foreach (var secondary in move.Base.Secondaries)
                    {
                        var rand = UnityEngine.Random.Range(1, 101);
                        if (rand <= secondary.Chance)
                            yield return RunMoveEffects(secondary, sourceUnit.Monster, targetUnit.Monster, secondary.Target);
                    }
                }

                if (targetUnit.Monster.HP <= 0)
                {
                    yield return HandleMonsterFainted(targetUnit);
                }

            }
            else
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Monster.Base.Name} ha evitato l'attacco");
            }
        }

    }

    IEnumerator RunMoveEffects(MoveEffects effects, Monster source, Monster target, MoveTarget moveTarget)
    {

        //Applica potenziamento o depotenziamento delle statistiche
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoost(effects.Boosts);
            else
                target.ApplyBoost(effects.Boosts);
        }

        //Applica un effetto di stato
        if(effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        //Applica un effetto di stato volatile
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //Sezione per gli stati: Avvelenamento e Bruciatura
        sourceUnit.Monster.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Monster);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        if (sourceUnit.Monster.HP <= 0)
        {
            yield return HandleMonsterFainted(sourceUnit);

            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    bool CheckIfMoveHits(Move move, Monster source, Monster target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Monster monster)
    {
        while(monster.StatusChanges.Count > 0)
        {
            var message = monster.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandleMonsterFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Monster.Base.Name} è stato sconfitto");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        //Logica Degli XP
        if (!faintedUnit.IsPlayerUnit)
        {
            int xpYield = faintedUnit.Monster.Base.XpYield;
            int enemyLevel = faintedUnit.Monster.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int XpGain = Mathf.FloorToInt((xpYield * enemyLevel * trainerBonus) / 7);

            playerUnit.Monster.XP += XpGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} ha guadagnato {XpGain} punti esperienza");
            yield return playerUnit.Hud.SetXPSmooth();

            //Controlla livello
            while (playerUnit.Monster.CheckForLevelUp())
            {

                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} ha raggiunto il livello {playerUnit.Monster.Level}");

                //Controlla se è possibile imparare una nuova mossa
                var newMove = playerUnit.Monster.GetLearnableMoveAtCurrLevel();
                if (newMove != null)
                {
                    if (playerUnit.Monster.Moves.Count < MonsterBase.MaxNumOfMoves)
                    {
                        playerUnit.Monster.LearnMove(newMove.Base);
                        yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} ha imparato {newMove.Base.Name}");
                        dialogBox.SetMoveNames(playerUnit.Monster.Moves);
                    }
                    else
                    {
                        yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} sta cercando di imparare {newMove.Base.Name}");
                        yield return dialogBox.TypeDialog($"Ma non può conoscere più di {MonsterBase.MaxNumOfMoves} mosse alla volta");
                        yield return ChooseMoveToForget(playerUnit.Monster, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);

                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.Hud.SetXPSmooth(true);

            }

            yield return new WaitForSeconds(1f);
        }
        //Fine logica degli XP

        CheckForBattleOver(faintedUnit);
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextMonster = playerParty.GetHealthyMonster();

            if (nextMonster != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextMonster = trainerParty.GetHealthyMonster();

                if (nextMonster != null)
                    StartCoroutine(AboutToUse(nextMonster));
                else
                    BattleOver(true);
            }
        }
            

    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f) yield return dialogBox.TypeDialog("Colpo Critico!");

        if(damageDetails.TypeEffectiveness > 1f) 
            yield return dialogBox.TypeDialog("È Superefficace!");
        else if(damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("Non è molto efficace");
    }

    public void HandleUpdate()
    {
        if(state == BattleState.ActionSelection)
        {
            //HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            //HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection(); //done
        }
        else if (state == BattleState.Bag)
        {

            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                StartCoroutine(OnItemUsed(usedItem));
            };

            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.AboutToUse)
        {
            //HandleAboutToUse();
        }
        else if(state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if(moveIndex == MonsterBase.MaxNumOfMoves)
                {
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} non ha imparato {moveToLearn.Name}"));
                }
                else
                {
                    var selectedMove = playerUnit.Monster.Moves[moveIndex].Base;

                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} ha dimenticato {selectedMove.Name} ed al suo posto ha imparato {moveToLearn.Name}"));

                    playerUnit.Monster.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;

                state = BattleState.RunningTurn;

            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction == 0)
            {
                //Lotta
                MoveSelection();
            }
            else if(currentAction == 1)
            {
                //Zaino
                //StartCoroutine(RunTurns(BattleAction.UseItem));
                OpenBag();
            }
            else if (currentAction == 2)
            {
                //Monster
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //Scappa
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    void HandleMoveSelection()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Monster.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Monster.Moves[currentMove], playerUnit.Monster);

        if (Input.GetKeyDown(KeyCode.Z))
        {

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);

            StartCoroutine(RunTurns(BattleAction.Move));

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);

            ActionSelection();
        }
    }

    public void SetCurrentMove(int move)
    {
        currentMove = move;

        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);

        StartCoroutine(RunTurns(BattleAction.Move));
    }

    public void ExitMoveSelection()
    {
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);

        ActionSelection();
    }

    void HandlePartySelection()
    {

        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("Non puoi scegliere un MT esausto");
                return;
            }

            if (selectedMember == playerUnit.Monster)
            {

                partyScreen.SetMessageText("Questo MT è già schierato");
                return;

            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                StartCoroutine(RunTurns(BattleAction.SwitchMonster));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUse;
                StartCoroutine(SwitchMonster(selectedMember, isTrainerAboutToUse));
            }

            partyScreen.CalledFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.Monster.HP <= 0)
            {
                partyScreen.SetMessageText("Devi scegliere un Monster per continuare");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.AboutToUse)
            {
                StartCoroutine(SendNextTrainerMonster());
            }
            else
                ActionSelection();

            partyScreen.CalledFrom = null;
        };

        partyScreen.HandleUpdate(onSelected, onBack);

    }

    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if(aboutToUseChoice == true)
            {
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTrainerMonster());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerMonster());
        }
    }

    public void AboutToUseConfirm()
    {
        dialogBox.EnableChoiceBox(false);
        OpenPartyScreen();
    }

    public void AboutToUseDeny()
    {
        dialogBox.EnableChoiceBox(false);
        StartCoroutine(SendNextTrainerMonster());
    }

    IEnumerator SwitchMonster(Monster newMonster, bool isTrainerAboutToUse=false)
    {
        if (playerUnit.Monster.HP > 0)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} ritorna!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(0.2f);
        }

        playerUnit.Setup(newMonster);
        dialogBox.SetMoveNames(newMonster.Moves);
        yield return dialogBox.TypeDialog($"Vai {newMonster.Base.Name}!");

        if(isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerMonster());
        else
            state = BattleState.RunningTurn;
        
    }

    IEnumerator SendNextTrainerMonster()
    {
        state = BattleState.Busy;

        var nextMonster = trainerParty.GetHealthyMonster();
        enemyUnit.Setup(nextMonster);
        yield return dialogBox.TypeDialog($"{trainer.Name} manda in campo {nextMonster.Base.Name}!");

        state = BattleState.RunningTurn;
    }

    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleState.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is CaptureDeviceItem)
        {
            yield return ThrowCaptureDevice((CaptureDeviceItem)usedItem);
        }

        StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    IEnumerator ThrowCaptureDevice(CaptureDeviceItem captureDeviceItem)
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"Non puoi rubare i Monster altrui");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} usa un {captureDeviceItem.Name}!");

        var cdObj = Instantiate(captureDevice, captureStartPosition.position /*+ new Vector3(2, 0)*/, Quaternion.identity);
        cdObj.transform.localScale = Vector3.one;
        cdObj.GetComponent<Image>().sprite = captureDeviceItem.Icon;
        cdObj.transform.SetParent(captureDeviceHost, false);
        cdObj.transform.position = captureStartPosition.position;
        //var captureDev = cdObj.GetComponent<Image>();
        var captureDev = cdObj.GetComponentInChildren<CaptureDevAnimations>();
        //captureDev.sprite = captureDeviceItem.Icon;

        //Animazioni
        yield return cdObj.transform.DOJump(captureEndPosition.position /*+ new Vector3(0, 2)*/, 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        //yield return captureDev.transform.DOMoveY(enemyUnit.transform.position.y - 2f, 0.5f).WaitForCompletion();
        yield return captureDev.Initialize();
        yield return new WaitUntil(() => captureDev.animating == false);

        int shakeCount = TryToCatchMonster(enemyUnit.Monster, captureDeviceItem);

        Debug.Log("Shakes: "+shakeCount);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            //yield return captureDev.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
            yield return captureDev.Shake();
            yield return new WaitUntil(() => captureDev.animating == false);
        }

        yield return new WaitForSeconds(1f);

        if (shakeCount == 4)
        {
            yield return captureDev.Captured();
            yield return new WaitUntil(() => captureDev.animating == false);
            //Catturato
            yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} è stato catturato!");
            //yield return captureDev.DOFade(0, 1.5f).WaitForCompletion();
            

            playerParty.AddMonster(enemyUnit.Monster);
            yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} è stato aggiunto alla tua squadra");

            //Destroy(captureDev);
            Destroy(cdObj);
            BattleOver(true);
        }
        else
        {
            //Non catturato
            yield return new WaitForSeconds(1f);
            yield return captureDev.NotCaptured();
            yield return new WaitUntil(() => captureDev.animating == false);
            //yield return enemyUnit.PlayBreakOutAnimation();

            //Destroy(captureDev);
            Destroy(cdObj);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} si è liberato!");
            else
                yield return dialogBox.TypeDialog($"C'eri quasi!");
            
            state = BattleState.RunningTurn;
        }
    }

    int TryToCatchMonster(Monster monster, CaptureDeviceItem captureDeviceItem)
    {
        float a = (3 * monster.MaxHp - 2 * monster.HP) * monster.Base.ChatchRate * captureDeviceItem.CatchRateModifier * ConditionsDB.GetStatusBonus(monster.Status) / (3 * monster.MaxHp);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++shakeCount;
        }

        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("Non puoi fuggire dalla battaglia contro un allenatore");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Monster.Speed;
        int enemySpeed = enemyUnit.Monster.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog("Sei riuscito a fuggire!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog("Sei riuscito a fuggire!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog("Fuga fallita!");
                state = BattleState.RunningTurn;
            }
        }
    }
}
