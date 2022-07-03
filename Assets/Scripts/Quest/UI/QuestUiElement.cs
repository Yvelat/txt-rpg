using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUiElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questDescription;
    [SerializeField] TextMeshProUGUI questProgress;

    public void SetData(Quest quest)
    {
        questName.text = quest.Base.Name;
        questDescription.text = quest.Base.Description;
        questProgress.text = $"{quest.Progress}/{quest.Base.MaxProgress}";
    }
}
