using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSeconds;

    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject exitButton;
    //[SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    //[SerializeField] Text staminaText;
    //[SerializeField] Text typeText;

    [SerializeField] Text yesText;
    [SerializeField] Text noText;

    Color highlightedColor;

    private void Start()
    {
        highlightedColor = GlobalSettings.i.HighlightedColor;
    }

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSeconds);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        exitButton.SetActive(enabled);
        //moveDetails.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move, Monster monster)
    {
        /*
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }

        }

        
        staminaText.text = $"Stamina {move.StaminaCost}";
        typeText.text = move.Base.Type.ToString();

        if (move.StaminaCost > monster.Stamina)
            staminaText.color = Color.red;
        else
            staminaText.color = Color.black;
        */
    }

    public void SetMoveNames(List<Move> moves)
    {

        moveSelector.GetComponent<MoveSelection>().SetMoveNames(moves);

    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = highlightedColor;
        }
    }
}
