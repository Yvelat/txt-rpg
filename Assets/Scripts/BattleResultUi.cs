using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleResultUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] GameObject DropList;

    [SerializeField] GameObject DropElementPrefab;

    [HideInInspector]
    public bool exitPressed = false;

    public void SetData(bool win, DropTable table)
    {
        exitPressed = false;
        winText.text = (win) ? "Hai Vinto!" : "Hai Perso";
        coinText.text = table.coins.ToString();
        gemText.text = table.gems.ToString();
        SetDropList(table.dropList);
    }

    public void SetDropList(List<DropTableElement> drops)
    {
        foreach (Transform child in DropList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (DropTableElement drop in drops)
        {
            var obj = Instantiate(DropElementPrefab, DropList.transform);

            obj.GetComponent<DropItemUiElement>().SetData(drop);
        }

    }

    public void ConfirmButton()
    {
        exitPressed = true;
    }

}
