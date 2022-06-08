using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSeconds;

    public event Action OnShowDialog;
    public event Action OnDialogFinished;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public bool isShowing { get; private set; }

    public IEnumerator ShowDialodText(string text, bool waitFroInput=true, bool autoClose=true, List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        OnShowDialog?.Invoke();
        isShowing = true;
        dialogBox.SetActive(true);

        yield return TypeDialog(text);

        if (waitFroInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }
        OnDialogFinished?.Invoke();
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        isShowing = false;
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        isShowing = true;

        dialogBox.SetActive(true);

        foreach(var line in dialog.Lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        isShowing = false;

        OnDialogFinished?.Invoke();

    }

    public void HandleUpdate()
    {
        
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSeconds);
        }
    }
}
