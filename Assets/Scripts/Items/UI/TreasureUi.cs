using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureUi : MonoBehaviour
{
    [SerializeField] GameObject visualizer;

    [Header("Prefabs")]
    [SerializeField] GameObject valuePrefab;
    [SerializeField] GameObject DropPrefab;

    [Header("ValueIcon")]
    [SerializeField] Sprite coin;
    [SerializeField] Sprite gem;

    [HideInInspector]
    public bool confirm = false;

    public void SetData(int value, TreasureType type)
    {
        Clear();

        if (type == TreasureType.Coin)
        {
            var obj = Instantiate(valuePrefab, visualizer.transform);

            obj.GetComponent<TrasureUiValues>().SetData(coin, value, "coins");
        }
        else if (type == TreasureType.Gem)
        {
            var obj = Instantiate(valuePrefab, visualizer.transform);

            obj.GetComponent<TrasureUiValues>().SetData(gem, value, "gems");
        }
    }

    public void SetData(DropTableElement drop)
    {
        Clear();

        if (drop != null)
        {
            var obj = Instantiate(DropPrefab, visualizer.transform);

            obj.GetComponent<DropItemUiElement>().SetData(drop);
        }
    }

    void Clear()
    {
        confirm = false;

        foreach (Transform child in visualizer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Confirm()
    {
        confirm = true;
    }

}

public enum TreasureType
{
    Coin,
    Gem,
    Drop
}
