using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<GameObject> moveTexts;
    [SerializeField] Color highlightedColor;

    int currentSelection = 0;

    Action<int> onSelected;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for(int i = 0; i < currentMoves.Count; ++i)
        {
            moveTexts[i].GetComponent<TextMeshProUGUI>().text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].GetComponent<TextMeshProUGUI>().text = newMove.Name;
    }

    public void HandleMoveSelection(Action<int> onSelected)
    {

        this.onSelected = onSelected;

        /*if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, MonsterBase.MaxNumOfMoves);

        UpdateMoveSelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Z))
            onSelected?.Invoke(currentSelection);*/
    }

    public void Move1()
    {
        onSelected?.Invoke(0);
    }

    public void Move2()
    {
        onSelected?.Invoke(1);
    }

    public void Move3()
    {
        onSelected?.Invoke(2);
    }

    public void Move4()
    {
        onSelected?.Invoke(3);
    }

    public void MovetoLearn()
    {
        onSelected?.Invoke(4);
    }

    public void UpdateMoveSelection(int selection)
    {
        /*for(int i = 0; i < MonsterBase.MaxNumOfMoves+1; ++i)
        {
            if (i == selection)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }*/
    }
}
