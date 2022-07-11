using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Crea nuova Quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string questName;
    [SerializeField] string description;
    [SerializeField] string bigDescription; //Opzionale

    [SerializeField] QuestType type;
    [SerializeField] int maxProgress;

    [Header("FixedDrop")]
    [SerializeField] DropTable dropTable;

    public string Name => questName;
    public string Description => description;
    public string BigDescription => bigDescription;
    public int MaxProgress => maxProgress;
    public DropTable DropTable => dropTable;
    public QuestType Type => type;

}

public enum QuestType
{
    Dungeon,
    DefeatedMonster,
    Catch,
    Step,
    DefeatedTrainers,
    Bestiary
}
