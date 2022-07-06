using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterBoxUIState { ItemSelection, PartySelection, Busy }
public enum SwitchMode { Switch, Remove }

public class MonsterBoxUI : MonoBehaviour
{
    [SerializeField] MonsterBox monsterBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MonsterParty monsterParty;
    [SerializeField] GameObject confirmBox;

    [Header("UpPageUI")]
    [SerializeField] Image itemIcon;

    [Header("DownPageUI")]
    [SerializeField] GameObject monsterBoxList;
    [SerializeField] GameObject monsterBoxUiSlotPrefab;

    [SerializeField] GameObject RightButton;
    [SerializeField] GameObject LeftButton;

    [SerializeField] TextMeshProUGUI pageText;

    int selectedItem = 0;
    int selectedPage = 0;

    bool messageBoxConfirmed;

    Action onBack;

    MonsterBoxUIState state;

    SwitchMode mode;

    Vector2Int selectedSlot;

    private void Start()
    {
        confirmBox.SetActive(false);
        messageBoxConfirmed = false;
        state = MonsterBoxUIState.ItemSelection;
        InitializePage(0);
    }

    void InitializePage(int page)
    {
        Clear();

        LoadPage(page);
    }

    void LoadPage(int page)
    {
        for (int i = 0; i < monsterBox.Monsters[page].Length; i++)
        {
            if (monsterBox.Monsters[page][i] == null) continue;

            var obj = Instantiate(monsterBoxUiSlotPrefab, monsterBoxList.transform);

            obj.GetComponent<MonsterBoxUiSlot>().SetData(monsterBox.Monsters[page][i], new Vector2Int(page, i));
        }
    }

    void Clear()
    {
        foreach (Transform child in monsterBoxList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void ResetSelection()
    {
        selectedItem = 0;
    }

    public void HandleUpdate(Action onBack)
    {
        this.onBack = onBack;

        if (state == MonsterBoxUIState.PartySelection)
        {

            Action onSelected = () => {

                StartCoroutine(EditParty());

            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
    }

    public IEnumerator EditParty()
    {
        state = MonsterBoxUIState.Busy;

        messageBoxConfirmed = false;

        if (selectedSlot == null) yield break;

        if (mode == SwitchMode.Switch)
        {

            Monster monster = monsterBox.GetInitializedMonsterFromSlot(selectedSlot);

            Monster partyMonster = monsterParty.GetMonsterByIndex(partyScreen.Selection);

            if(monsterParty.SetMonster(monster, partyScreen.Selection))
                monsterBox.SetMonsterToSlot(partyMonster, selectedSlot);

        }
        else if (mode == SwitchMode.Remove)
        {

            Monster partyMonster = monsterParty.GetMonsterByIndex(partyScreen.Selection);

            if(monsterParty.RemoveMonster(partyScreen.Selection))
                monsterBox.AddMonsterToBox(partyMonster);

        }

        ClosePartyScreen();

        confirmBox.SetActive(true);

        yield return new WaitUntil(() => messageBoxConfirmed == true);

        state = MonsterBoxUIState.ItemSelection;

    }

    public IEnumerator AddMonsterToParty()
    {
        state = MonsterBoxUIState.Busy;
        messageBoxConfirmed = false;

        Monster monster = monsterBox.GetInitializedMonsterFromSlot(selectedSlot);
        monsterParty.AddMonster(monster);

        confirmBox.SetActive(true);

        yield return new WaitUntil(() => messageBoxConfirmed == true);

        state = MonsterBoxUIState.ItemSelection;
    }

    public void SelectedMonsterToSwitch()
    {
        mode = SwitchMode.Switch;
        if (state == MonsterBoxUIState.ItemSelection)
            if (!monsterParty.HasEmptySpace())
                OpenPartyScreen();
            else
                StartCoroutine(AddMonsterToParty());
    }

    public void SelectMonsterToRemoveFromParty()
    {
        mode = SwitchMode.Remove;
        if (state == MonsterBoxUIState.ItemSelection)
            OpenPartyScreen();
    }

    public void ButtonBack()
    {
        onBack?.Invoke();
    }

    public void MessageBoxConfirmed()
    {
        messageBoxConfirmed = true;
    }

    void OpenPartyScreen()
    {
        state = MonsterBoxUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = MonsterBoxUIState.ItemSelection;
        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    public void NextPage()
    {
        ++selectedPage;
        selectedItem = 0;

        if (selectedPage > monsterBox.Monsters.Count - 1)
            selectedPage = 0;
        else if (selectedPage < 0)
            selectedPage = monsterBox.Monsters.Count - 1;

        ResetSelection();
        pageText.text = $"Pagina {selectedPage+1}";
        InitializePage(selectedPage);
        ButtonDisableController();
    }

    public void PrevPage()
    {
        --selectedPage;
        selectedItem = 0;

        if (selectedPage > monsterBox.Monsters.Count - 1)
            selectedPage = 0;
        else if (selectedPage < 0)
            selectedPage = monsterBox.Monsters.Count - 1;

        ResetSelection();
        pageText.text = $"Pagina {selectedPage + 1}";
        InitializePage(selectedPage);
        ButtonDisableController();
    }

    void ButtonDisableController()
    {
        if (selectedPage == 0)
        {
            LeftButton.SetActive(false);
            RightButton.SetActive(true);
        }
        else if (selectedPage == monsterBox.Monsters.Count - 1)
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

    public void UpdateDetails(MonsterBoxItem item, Vector2Int index)
    {
        itemIcon.sprite = item.monster.FrontSprite;
        selectedItem = index.y;
        selectedSlot = index;
    }
}
