using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonList : MonoBehaviour
{
    [SerializeField] GameObject dungeonList;
    [SerializeField] GameObject dungeonButtonPrefab;
    [SerializeField] GameObject adventuring;
    [SerializeField] List<Dungeon> list;

    private void Start()
    {
        InitializeDungeonList();
    }

    public void InitializeDungeonList()
    {
        foreach (Transform child in dungeonList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var dungeon in list)
        {
            var newButton = Instantiate(dungeonButtonPrefab, dungeonList.transform);
            newButton.GetComponent<DungeonButton>().SetDungeon(dungeon, gameObject, adventuring);
        }
    }
}
