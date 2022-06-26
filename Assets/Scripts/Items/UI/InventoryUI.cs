using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy }

public class InventoryUI : MonoBehaviour
{

    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] TextMeshProUGUI categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;

    //[SerializeField] Image upArrow;
    //[SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    [SerializeField] GameObject RightButton;
    [SerializeField] GameObject LeftButton;

    [SerializeField] TreasureUi treasureUi;

    Action<ItemBase> OnItemUsed;
    Action onBack;

    int selectedItem = 0;
    int selectedCategory = 0;

    MoveBase moveToLearn;

    InventoryUIState state;

    const int itemsInViewport = 8;

    Inventory inventory;

    List<ItemSlotUI> slotUIList;

    RectTransform itemListRect;

    private void Awake()
    {
        inventory = Inventory.GetInventory();

        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;

        FirstUpdate();
    }

    void UpdateItemList()
    {
        //rimuovi tutti gli items esistenti
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        int index = 0;
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIOgj = Instantiate(itemSlotUI, itemList.transform);
            slotUIOgj.SetData(itemSlot, index);

            slotUIList.Add(slotUIOgj);
            index++;
        }

        UpdateItemSelection();
    }

    void FirstUpdate()
    {
        ResetSelection();
        categoryText.text = Inventory.ItemCategories[selectedCategory];
        UpdateItemList();
        ButtonDisableController();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUse=null)
    {

        this.OnItemUsed = onItemUse;
        this.onBack = onBack;

        if ( state == InventoryUIState.ItemSelection)
        {
            /*int prevSelection = selectedItem;
            int prevCatSelection = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedCategory;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedCategory;


            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;


            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            if (prevCatSelection != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(ItemSelected());
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }*/

        }
        else if (state == InventoryUIState.PartySelection)
        {

            Action onSelected = () => {

                StartCoroutine(UseItem());

            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if(state == InventoryUIState.MoveToForget)
        {

            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    public void SelectItem()
    {
        if (!inventory.IsCategoryEmpty(selectedCategory))
            if (state == InventoryUIState.ItemSelection)
                StartCoroutine(ItemSelected());
    }

    public void ButtonBack()
    {
        onBack?.Invoke();
    }

    public void NextPage()
    {
        ++selectedCategory;
        selectedItem = 0;

        if (selectedCategory > Inventory.ItemCategories.Count - 1)
            selectedCategory = 0;
        else if (selectedCategory < 0)
            selectedCategory = Inventory.ItemCategories.Count - 1;

        ResetSelection();
        categoryText.text = Inventory.ItemCategories[selectedCategory];
        UpdateItemList();
        ButtonDisableController();
    }

    public void PrevPage()
    {
        --selectedCategory;
        selectedItem = 0;

        if (selectedCategory > Inventory.ItemCategories.Count - 1)
            selectedCategory = 0;
        else if (selectedCategory < 0)
            selectedCategory = Inventory.ItemCategories.Count - 1;

        ResetSelection();
        categoryText.text = Inventory.ItemCategories[selectedCategory];
        UpdateItemList();
        ButtonDisableController();
    }

    void ButtonDisableController()
    {
        if(selectedCategory == 0)
        {
            LeftButton.SetActive(false);
            RightButton.SetActive(true);
        }
        else if (selectedCategory == Inventory.ItemCategories.Count - 1)
        {
            LeftButton.SetActive(true);
            RightButton.SetActive(false);
        }
        else
        {
            LeftButton.SetActive(true);
            RightButton.SetActive(true);
        }
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Battle)
        {
            if (!item.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialodText($"Non puoi usare questo oggetto in combattimento");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialodText($"Puoi usare questo oggetto solo in combattimento");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }

        if(selectedCategory == (int)ItemCategory.CaptureDevice || item is TreasureItem)
        {
            yield return UseItem();
        }
        else
        {
            OpenPartyScreen();

            if (item is GrimoireItem)
                partyScreen.ShowIfGrimoireIsUsable(item as GrimoireItem);

        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleGrimoireItems();

        var item = inventory.GetItem(selectedItem, selectedCategory);
        Monster monster = null;

        if (!(item is TreasureItem))
            monster = partyScreen.SelectedMember;

        //Handle Evolution by Item
        if (item is EvolutionItem)
        {
            var evolution = monster.CheckForEvolution(item);
            if(evolution != null)
            {
                yield return EvolutionManager.i.Evolve(monster, evolution);
            }
            else
            {
                yield return DialogManager.Instance.ShowDialodText($"Non avrebbe nessun effetto");
                ClosePartyScreen();
                yield break;
            }
        }

        var usedItem = inventory.UseItem(selectedItem, monster, selectedCategory);
        if(usedItem != null)
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialodText($"Hai usato {usedItem.Name}");

            if (usedItem is TreasureItem)
            {
                yield return new WaitUntil(() => treasureUi.confirm == true);
                treasureUi.gameObject.SetActive(false);
                state = InventoryUIState.ItemSelection;
                yield break;
            }

            OnItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Item)
                yield return DialogManager.Instance.ShowDialodText($"Non avrebbe nessun effetto");
        }

        if(!(item is TreasureItem))
            ClosePartyScreen();
    }

    IEnumerator HandleGrimoireItems()
    {
        var grimoireItem = inventory.GetItem(selectedItem, selectedCategory) as GrimoireItem;

        if(grimoireItem == null)
            yield break;

        var monster = partyScreen.SelectedMember;

        if (monster.HasMove(grimoireItem.Move))
        {
            yield return DialogManager.Instance.ShowDialodText($"{monster.Base.Name} conosce gia {grimoireItem.Move.Name}");
            yield break;
        }

        if (!grimoireItem.CanBeTaught(monster))
        {
            yield return DialogManager.Instance.ShowDialodText($"{monster.Base.Name} non puo imparare {grimoireItem.Move.Name}");
            yield break;
        }

        if(monster.Moves.Count < MonsterBase.MaxNumOfMoves)
        {
            monster.LearnMove(grimoireItem.Move);

            yield return DialogManager.Instance.ShowDialodText($"{monster.Base.Name} ha imparato {grimoireItem.Move.Name}");

        }
        else
        {
            yield return DialogManager.Instance.ShowDialodText($"{monster.Base.Name} sta cercando di imparare {grimoireItem.Move.Name}");
            yield return DialogManager.Instance.ShowDialodText($"Ma non può conoscere più di {MonsterBase.MaxNumOfMoves} mosse");
            yield return ChooseMoveToForget(monster, grimoireItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }

    IEnumerator ChooseMoveToForget(Monster monster, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialodText("Scegli una mossa da dimenticare", true, false);

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(monster.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            /*if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;*/
        }

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        //HandleScrolling();
    }

    public void UpdateDetails(ItemBase item, int index)
    {
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;
        selectedItem = index;
    }

    void HandleScrolling()
    {

        if (slotUIList.Count <= itemsInViewport) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport/2, 0, selectedItem) * slotUIList[0].Height;

        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        //upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
        //downArrow.gameObject.SetActive(showDownArrow);
        
    }

    void ResetSelection()
    {
        selectedItem = 0;
        //upArrow.gameObject.SetActive(false);
        //downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var monster = partyScreen.SelectedMember;

        DialogManager.Instance.CloseDialog();

        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == MonsterBase.MaxNumOfMoves)
        {
            yield return DialogManager.Instance.ShowDialodText($"{monster.Base.Name} non ha imparato {moveToLearn.Name}");
        }
        else
        {
            var selectedMove = monster.Moves[moveIndex].Base;

            yield return DialogManager.Instance.ShowDialodText($"{monster.Base.Name} ha dimenticato {selectedMove.Name} ed al suo posto ha imparato {moveToLearn.Name}");

            monster.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;

        state = InventoryUIState.ItemSelection;
    }
}
