using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField] QuestList questList;

    public void ProgressAllQuestOfType(QuestType type, int amount)
    {
        foreach (var quest in questList.Quests)
        {
            if(quest.Base.Type == type)
            {
                quest.AddProgressQuest(amount);
            }
        }
    }

    public void ProgressSpecificQuest(string name, int amount)
    {
        foreach (var quest in questList.Quests)
        {
            if (quest.Base.Name == name)
            {
                quest.AddProgressQuest(amount);
                break;
            }
        }
    }

    public void ProgressSpecificQuest(QuestBase questBase, int amount)
    {
        foreach (var quest in questList.Quests)
        {
            if (quest.Base.Name == questBase.Name)
            {
                quest.AddProgressQuest(amount);
                break;
            }
        }
    }

}
