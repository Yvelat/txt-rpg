using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    [SerializeField] QuestBase baseQuest;
    public QuestStatus Status { get; private set; }

    public int Progress { get; private set; }

    private bool redeemed = false;

    public void Init()
    {
        if (!redeemed)
        {
            if (Progress < baseQuest.MaxProgress)
            {
                Status = QuestStatus.Uncomplete;
            }
            else if (Progress >= baseQuest.MaxProgress)
            {
                Status = QuestStatus.Completed;
            }
        }
        else
        {
            Status = QuestStatus.Completed;
        }
    }

    public IEnumerator CompleteQuest()
    {
        Status = QuestStatus.Completed;

        yield return null;
    }

    public void AddProgressQuest(int amount)
    {
        Progress += amount;
    }

    public QuestBase Base => baseQuest;

    public bool Redeemed => redeemed;
}

public enum QuestStatus { None, Uncomplete, Completed }
