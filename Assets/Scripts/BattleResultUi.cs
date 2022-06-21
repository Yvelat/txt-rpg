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

    public void SetData(bool win, DropTable table)
    {
        winText.text = (win) ? "Hai Vinto!" : "Hai Perso";
        coinText.text = table.coins.ToString();
        gemText.text = table.gems.ToString();
        SetDropList(table.dropList);
    }

    public void SetDropList(List<DropTableElement> drops)
    {

        foreach (DropTableElement drop in drops)
        {
            var obj = Instantiate(DropElementPrefab, DropList.transform);

            obj.GetComponent<DropItemUiElement>().SetData(drop);
        }

    }

}
