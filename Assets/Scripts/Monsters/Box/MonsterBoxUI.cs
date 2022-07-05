using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterBoxUI : MonoBehaviour
{
    [SerializeField] MonsterBox monsterBox;

    [Header("UI")]
    [SerializeField] GameObject monsterBoxList;
    [SerializeField] GameObject monsterBoxUiSlotPrefab;

    [SerializeField] GameObject RightButton;
    [SerializeField] GameObject LeftButton;

    [SerializeField] TextMeshProUGUI pageText;

    int selectedItem = 0;
    int selectedPage = 0;

    private void Start()
    {
        InitializePage(0);
    }

    void InitializePage(int page)
    {
        Clear();

        LoadPage(page);
    }

    void LoadPage(int page)
    {
        for (int i = 0; i < monsterBox.Monsters[page].Length; i++)
        {
            if (monsterBox.Monsters[page][i] == null) continue;

            var obj = Instantiate(monsterBoxUiSlotPrefab, monsterBoxList.transform);

            obj.GetComponent<MonsterBoxUiSlot>().SetData(monsterBox.Monsters[page][i], new Vector2Int(page, i));
        }
    }

    void Clear()
    {
        foreach (Transform child in monsterBoxList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void ResetSelection()
    {
        selectedItem = 0;
    }

    public void NextPage()
    {
        ++selectedPage;
        selectedItem = 0;

        if (selectedPage > monsterBox.Monsters.Count - 1)
            selectedPage = 0;
        else if (selectedPage < 0)
            selectedPage = monsterBox.Monsters.Count - 1;

        ResetSelection();
        pageText.text = $"Pagina {selectedPage+1}";
        InitializePage(selectedPage);
        ButtonDisableController();
    }

    public void PrevPage()
    {
        --selectedPage;
        selectedItem = 0;

        if (selectedPage > monsterBox.Monsters.Count - 1)
            selectedPage = 0;
        else if (selectedPage < 0)
            selectedPage = monsterBox.Monsters.Count - 1;

        ResetSelection();
        pageText.text = $"Pagina {selectedPage + 1}";
        InitializePage(selectedPage);
        ButtonDisableController();
    }

    void ButtonDisableController()
    {
        if (selectedPage == 0)
        {
            LeftButton.SetActive(false);
            RightButton.SetActive(true);
        }
        else if (selectedPage == monsterBox.Monsters.Count - 1)
        {
            LeftButton.SetActive(true);
            RightButton.SetActive(false);
        }
        else
        {
            LeftButton.SetActive(true);
            RightButton.SetActive(true);
        }
    }
}
