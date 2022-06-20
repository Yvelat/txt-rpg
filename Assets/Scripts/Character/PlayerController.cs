using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string playerName;
    //[SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI gemsText;
    [SerializeField] TextMeshProUGUI energyText;

    int coins;
    int gems;
    int energy = -1;
    int levl;
    int xp;

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

        coinsText.text = $"{coins}";
        gemsText.text = $"{gems}";

        if (energy < maxEnergy && !recovering)
        {
            StartCoroutine(RecoverEnergy());
        }
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
        energy = 25 + (3 * levl);
        maxEnergy = energy;
    }

    int GetNextLevelXp()
    {
        int level = levl + 1;

        return level * level * level;
    }

    void SetName(string Name)
    {
        playerName = Name;
    }

    public void SetCoins(int coins)
    {
        this.coins = coins;
    }

    public void SetGems(int gems)
    {
        this.gems = gems;
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
        get => levl;
    }

    public int Xp
    {
        get => xp;
    }
}
