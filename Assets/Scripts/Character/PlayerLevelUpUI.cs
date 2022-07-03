using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUpUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statsUp;

    [Header("PrevInfo")]
    [SerializeField] Image prevXpBar;
    [SerializeField] TextMeshProUGUI prevLevel;

    [Header("ActualInfo")]
    [SerializeField] Image xpBar;
    [SerializeField] TextMeshProUGUI level;

    bool isClosed = false;

    public void SetData(float prevXp, float xp, int prevLvl, int lvl, int prevEnergy, int energy)
    {
        isClosed = false;
        statsUp.text = $"Stamina Massima: {prevEnergy} -> {energy}";
        prevXpBar.fillAmount = prevXp;
        xpBar.fillAmount = xp;
        prevLevel.text = $"Lv. {prevLvl}";
        level.text = $"Lv. {lvl}";

    }

    public void CloseUI()
    {
        isClosed = true;
        gameObject.SetActive(false);
    }

    public bool IsClosed => isClosed;

}
