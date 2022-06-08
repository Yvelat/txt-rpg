using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveSelection : MonoBehaviour
{
    [SerializeField] GameObject BackButton;

    [SerializeField] List<GameObject> movesButtons;

    BattleSystem battleSystem;

    private void OnEnable()
    {
        BackButton.SetActive(true);
    }

    private void OnDisable()
    {
        BackButton.SetActive(false);
    }

    private void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>(true);
    }

    public void SetMoveNames(List<Move> moves)
    {

        for (int i = 0; i < movesButtons.Count; ++i)
        {
            if (i < moves.Count)
            {
                movesButtons[i].GetComponentInChildren<TextMeshProUGUI>(true).text = moves[i].Base.Name;
                movesButtons[i].GetComponent<LeanButton>().interactable = true;
            }
            else
            {
                movesButtons[i].GetComponentInChildren<TextMeshProUGUI>(true).text = "---";
                movesButtons[i].GetComponent<LeanButton>().interactable = false;
            }
        }

    }

    public void Move1()
    {
        battleSystem.SetCurrentMove(0);
    }

    public void Move2()
    {
        battleSystem.SetCurrentMove(1);
    }

    public void Move3()
    {
        battleSystem.SetCurrentMove(2);
    }

    public void Move4()
    {
        battleSystem.SetCurrentMove(3);
    }

    public void Exit()
    {
        battleSystem.ExitMoveSelection();
    }

}
