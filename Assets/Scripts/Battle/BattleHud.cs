using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI monsterNameText;
    [SerializeField] TextMeshProUGUI monsterLevelText;
    [SerializeField] Image statusBack;
    [SerializeField] Image actualStatus;
    [SerializeField] Image BattleIcon;
    [SerializeField] HPBar hpBar;
    [SerializeField] STBar stBar;
    [SerializeField] Image xpBar;

    [SerializeField] TextMeshProUGUI rarityText;

    [SerializeField] Sprite psn;
    [SerializeField] Sprite brn;
    [SerializeField] Sprite slp;
    [SerializeField] Sprite par;
    [SerializeField] Sprite frz;

    Monster _monster;
    Dictionary<ConditionID, Sprite> statusSrites;

    public void SetData(Monster monster)
    {

        ClearData();

        _monster = monster;

        monsterNameText.text = monster.Base.Name;
        SetLevel();
        hpBar.SetHp((float)monster.HP / monster.MaxHp);
        stBar.SetStamina((float)monster.Stamina / monster.MaxStamina);
        SetXP();
        SetRarity(monster.Base.Rarity);

        BattleIcon.sprite = monster.Base.BattleIcon;

        statusSrites = new Dictionary<ConditionID, Sprite>()
        {
            {ConditionID.psn, psn },
            {ConditionID.brn, brn },
            {ConditionID.slp, slp },
            {ConditionID.par, par },
            {ConditionID.frz, frz }
        };

        SetStatusText();
        _monster.OnStatusChange += SetStatusText;
        _monster.OnHPChange += UpdateHP;
        _monster.OnStaminaChange += UpdateStamina;
    }

    
    void SetStatusText()
    {
        if (_monster.Status == null)
        {
            statusBack.gameObject.SetActive(false);
        }
        else
        {
            //statusBack.text = _monster.Status.Id.ToString().ToUpper();
            statusBack.gameObject.SetActive(true);
            actualStatus.sprite = statusSrites[_monster.Status.Id];
        }
    }

    public void SetLevel()
    {
        monsterLevelText.text = $"Lvl: {_monster.Level}";
    }

    void SetRarity(Rarity rarity)
    {
        if (rarityText == null) return;

        rarityText.color = Color.white;

        switch (rarity)
        {
            case Rarity.Common:
                rarityText.text = "Comune";
                rarityText.colorGradientPreset = GlobalSettings.i.CommonColor;
                break;
            case Rarity.Uncommon:
                rarityText.text = "Non Comune";
                rarityText.colorGradientPreset = GlobalSettings.i.UncommonColor;
                break;
            case Rarity.Rare:
                rarityText.text = "Raro";
                rarityText.colorGradientPreset = GlobalSettings.i.RareColor;
                break;
            case Rarity.UltraRare:
                rarityText.text = "Ultra Raro";
                rarityText.colorGradientPreset = GlobalSettings.i.UltraRareColor;
                break;
            case Rarity.Epic:
                rarityText.text = "Epico";
                rarityText.colorGradientPreset = GlobalSettings.i.EpicColor;
                break;
            case Rarity.Legendary:
                rarityText.text = "Leggendario";
                rarityText.colorGradientPreset = GlobalSettings.i.LegendaryColor;
                break;
        }

    }

    public void SetXP()
    {
        if (xpBar == null) return;

        float normalizedXP = GetNormalizedXP();
        //xpBar.transform.localScale = new Vector3(normalizedXP, 1, 1);
        xpBar.fillAmount = normalizedXP;
    }

    
    public IEnumerator SetXPSmooth(bool reset = false)
    {
        if (xpBar == null) yield break;

        if (reset)
            xpBar.fillAmount = 0;
        //xpBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedXP = GetNormalizedXP();
        //yield return xpBar.transform.DOScaleX(normalizedXP, 1.5f).WaitForCompletion();
        yield return xpBar.DOFillAmount(normalizedXP, 1.5f).WaitForCompletion();
    }

    float GetNormalizedXP()
    {
        int currLevelXP = _monster.Base.GetXPForLevel(_monster.Level);
        int nextLevelXP = _monster.Base.GetXPForLevel(_monster.Level + 1);

        float normalizedXP = (float)(_monster.XP - currLevelXP) / (nextLevelXP - currLevelXP);
        return Mathf.Clamp01(normalizedXP);
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)_monster.HP / _monster.MaxHp);
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.isUpdating == false);
    }

    public void UpdateStamina()
    {
        StartCoroutine(UpdateStaminaAsync());
    }

    public IEnumerator UpdateStaminaAsync()
    {
        yield return stBar.SetStaminaSmooth((float)_monster.Stamina / _monster.MaxStamina);
    }

    public IEnumerator WaitForStaminaUpdate()
    {
        yield return new WaitUntil(() => stBar.isUpdating == false);
    }

    public void ClearData()
    {
        if (_monster != null)
        {
            _monster.OnStaminaChange -= UpdateStamina;
            _monster.OnHPChange -= UpdateHP;
            _monster.OnStatusChange -= SetStatusText;
        }
    }
}

