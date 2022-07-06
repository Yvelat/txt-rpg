using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;

    PartyMemberUI[] memberSlots;
    List<Monster> monsters;
    MonsterParty party;

    int selection = 0;

    Action onSelected;
    Action onBack;

    public Monster SelectedMember => monsters[selection];

    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = MonsterParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        monsters = party.Monsters;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < monsters.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(monsters[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        UpdateMemeberSelection(selection);

        messageText.text = "Scegli un MT";
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        this.onSelected = onSelected;
        this.onBack = onBack;
        /*var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selection -= 2;

        selection = Mathf.Clamp(selection, 0, monsters.Count - 1);

        if (selection != prevSelection)
            UpdateMemeberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }*/

    }

    public void SelectPartyMember(int index)
    {
        selection = index;
        selection = Mathf.Clamp(selection, 0, monsters.Count - 1);
        onSelected?.Invoke();
    }

    public void BackFromPartyScreen()
    {
        onBack?.Invoke();
    }

    public void UpdateMemeberSelection(int selectedMember)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void ShowIfGrimoireIsUsable(GrimoireItem grimoire)
    {

        for (int i = 0; i < monsters.Count; i++)
        {
            string message = grimoire.CanBeTaught(monsters[i]) ? "ABLE" : "NOT ABLE";

            memberSlots[i].SetMessage(message);

        }

    }

    public void ClearMemberSlotMessages()
    {

        for (int i = 0; i < monsters.Count; i++)
        {

            memberSlots[i].SetMessage("");

        }

    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }

    public int Selection => selection;
}
