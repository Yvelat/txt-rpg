using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestBase Base { get; private set; }
    public QuestStatus Status { get; private set; }

    public Quest(QuestBase _base)
    {
        Base = _base;
    }

    public Quest(QuestSaveData saveData)
    {
        Base = QuestDB.GetObjectByName(saveData.name);
        Status = saveData.status;
    }

    public QuestSaveData GetSaveData()
    {
        var saveData = new QuestSaveData()
        {
            name = Base.name,
            status = Status
        };

        return saveData;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;

        yield return DialogManager.Instance.ShowDialog(Base.StartDialogue);

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public IEnumerator CompleteQuest(Transform player)
    {
        Status = QuestStatus.Completed;

        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialogue);

        var inventory = Inventory.GetInventory();

        if(Base.RequireItem != null)
        {
            inventory.RemoveItem(Base.RequireItem);
        }

        if (Base.RewardItem != null)
        {
            inventory.AddItem(Base.RewardItem);

            string playerName = player.GetComponent<PlayerController>().Name;

            yield return DialogManager.Instance.ShowDialodText($"{playerName} ha ricevuto un {Base.RewardItem.Name}");
        }

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();

        if (Base.RequireItem != null)
        {
            if(!inventory.HasItem(Base.RequireItem))
                return false;
        }

        return true;
    }

}

[System.Serializable]
public class QuestSaveData
{
    public string name;
    public QuestStatus status;
}

public enum QuestStatus { None, Started, Completed }
