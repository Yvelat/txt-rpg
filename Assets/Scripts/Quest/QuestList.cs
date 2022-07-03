using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestList : MonoBehaviour
{
    [SerializeField] List<Quest> quests;

    public bool IsUncomplete(string questName)
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QuestStatus.Uncomplete;
    }

    public bool IsCompleted(string questName)
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QuestStatus.Completed;
    }

    public List<Quest> Quests => quests;
}
