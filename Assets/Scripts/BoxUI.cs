using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxUI : MonoBehaviour
{
    [SerializeField] GameObject monsterList;
    [SerializeField] BoxSlotUI boxSlotUI;

    [SerializeField] TextMeshProUGUI PageText;
    [SerializeField] Image MonsterIcon;
    [SerializeField] TextMeshProUGUI monsterDescription;

    [SerializeField] PartyScreen partyScreen;

    [SerializeField] GameObject RightButton;
    [SerializeField] GameObject LeftButton;

    int selectedMonster = 0;
    int selectedPage = 0;

    Box box;

    List<BoxSlotUI> boxUIList;

    RectTransform boxListRect;

    private void Awake()
    {
        box = Box.GetBox();

        boxListRect = monsterList.GetComponent<RectTransform>();
    }
}
