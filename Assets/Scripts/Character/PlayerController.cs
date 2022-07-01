using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //TODO: make playerName private
    [SerializeField] string playerName;
    //[SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI gemsText;
    [SerializeField] TextMeshProUGUI energyText;

    [Header("PlayerUI")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image playerImage;

    public Skin skin { get; private set; }

    int coins = 0;
    int gems = 0;
    int energy = -1;
    int level = 1;
    int xp = 0;

    int maxEnergy = 25;

    int actualTime;

    bool recovering = false;

    private void Awake()
    {
        if (energy < 0)
        {
            energy = maxEnergy;
        }
    }

    private void Update()
    {
        energyText.text = $"{energy}/{maxEnergy}";

        if (energy < maxEnergy && !recovering)
        {
            StartCoroutine(RecoverEnergy());
        }
    }

    public void Init(string name, Skin skin)
    {
        playerName = name;
        this.skin = skin;
    }

    public void UpdateUIValues()
    {
        coinsText.text = $"{coins}";
        gemsText.text = $"{gems}";
    }

    public void UpdateUiData()
    {
        levelText.text = $"Lv. {level}";
    }

    public void UseEnergy(int amount)
    {
        energy -= amount;
    }

    public bool HasEnergy()
    {
        if(energy > 0) return true;
        else return false;
    }

    IEnumerator RecoverEnergy()
    {
        Debug.Log("StartRecovering");
        recovering = true;
        yield return new WaitForSeconds(60f * 1f);
        energy++;
        recovering = false;
        Debug.Log("1 Recovered");
            
    }

    IEnumerator WaitForSecondsWithPrint(float seconds)
    {

        while (seconds != 0)
        {
            yield return new WaitForSeconds(1f);
            seconds -= 1f;

            float minutes = Mathf.Floor(seconds / 60);
            float second = Mathf.RoundToInt(seconds % 60);

            if (second < 10)
            {
                //timeText.text = $"{minutes}:0{Mathf.RoundToInt(seconds)}";
            }
            else
            {
                //timeText.text = $"{minutes}:{Mathf.RoundToInt(seconds)}";
            }
        }

    }

    public void HandleUpdate()
    {

    }

    //TODO: rivedere calcolo energia
    void SetEnergyBasedOnLevel()
    {
        energy = 25 + (1 * level);
        maxEnergy = energy;
    }

    int GetNextLevelXp()
    {
        int level = this.level + 1;

        return level * level * level;
    }

    public void AddXp(int amount)
    {
        xp += amount;
    }

    public bool CheckLevelUp()
    {
        int xpToNextLevel = GetNextLevelXp();

        if( xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            level++;
            UpdateUiData();
            SetEnergyBasedOnLevel();
            return true;
        }

        return false;
    }

    public void SetCoins(int coins)
    {
        this.coins = coins;
        UpdateUIValues();
    }

    public void SetGems(int gems)
    {
        this.gems = gems;
        UpdateUIValues();
    }

    public string Name
    {
        get => name;
    }

    public int Coins
    {
        get => coins;
    }

    public int Gems
    {
        get => gems;
    }

    public int Energy
    {
        get => energy;
    }

    public int Levl
    {
        get => level;
    }

    public int Xp
    {
        get => xp;
    }
}
