using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestList : MonoBehaviour, ISavable
{
    List<Quest> quests = new List<Quest>();

    public event Action onUpdated;

    public void AddQuest(Quest quest)
    {
        if (!quests.Contains(quest))
            quests.Add(quest);

        onUpdated?.Invoke();
    }

    public bool IsStarted(string questName)
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QuestStatus.Started || questStatus == QuestStatus.Completed;
    }

    public bool IsCompleted(string questName)
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QuestStatus.Completed;
    }

    public static QuestList GetQuestList()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestList>();
    }

    public object CaptureState()
    {
        return quests.Select(quest => quest.GetSaveData()).ToList();
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<QuestSaveData>;

        if (saveData != null)
        {
            quests = saveData.Select(quest => new Quest(quest)).ToList();
            onUpdated?.Invoke();
        }
    }
}
