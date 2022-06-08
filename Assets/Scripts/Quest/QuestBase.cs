using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Crea nuova quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string questName;
    [SerializeField] string description;

    [SerializeField] Dialog startDialogue;
    [SerializeField] Dialog inProgressDialogue; //OPZIONALE
    [SerializeField] Dialog completedDialogue;

    [SerializeField] ItemBase requireItem;
    [SerializeField] ItemBase rewardItem;

    public string Name => questName;
    public string Description => description;
    public Dialog StartDialogue => startDialogue;
    public Dialog InProgressDialogue => inProgressDialogue?.Lines?.Count > 0 ? inProgressDialogue : startDialogue;
    public Dialog CompletedDialogue => completedDialogue;
    public ItemBase RequireItem => requireItem;
    public ItemBase RewardItem => rewardItem;
}
