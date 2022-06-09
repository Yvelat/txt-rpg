using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI monsterNameText;
    [SerializeField] TextMeshProUGUI monsterLevelText;
    [SerializeField] Image monsterIcon;
    [SerializeField] HPBar hpBar;
    [SerializeField] STBar stBar;
    [SerializeField] TextMeshProUGUI messageText;

    Monster _monster;

    public void Init(Monster monster)
    {
        _monster = monster;
        UpdateData();
        SetMessage("");

        _monster.OnHPChange += UpdateData;

    }

    void UpdateData()
    {
        monsterIcon.sprite = _monster.Base.BattleIcon;
        monsterNameText.text = _monster.Base.Name;
        monsterLevelText.text = "Lvl. " + _monster.Level;
        hpBar.SetHp((float)_monster.HP / _monster.MaxHp);
        stBar.SetStamina((float)_monster.Stamina / _monster.MaxStamina);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            monsterNameText.color = GlobalSettings.i.HighlightedColor;
        else
            monsterNameText.color = Color.black;
    }

    public void SetMessage(string message)
    {
        //messageText.text = message;
    }
}
