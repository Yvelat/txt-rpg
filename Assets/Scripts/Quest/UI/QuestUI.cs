using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] GameObject questList;
    [SerializeField] GameObject questUiElementPrefab;

    public void SetData()
    {
        List<Quest> quests = GameController.Instance.GetQuestListComponent().Quests;

        foreach (Transform child in questList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest quest in quests)
        {
            var obj = Instantiate(questUiElementPrefab, questList.transform);

            obj.GetComponent<QuestUiElement>().SetData(quest);
        }
    }
}
