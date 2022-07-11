using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryUI : MonoBehaviour
{
    [SerializeField] Bestiary bestiary;

    [SerializeField] Image monsterImage;
    [SerializeField] TextMeshProUGUI description;

    [SerializeField] GameObject beastsList;
    [SerializeField] GameObject beastElementPrefab;

    private void Start()
    {

        Clear();

        for (int i = 0; i < bestiary.FullBestiary.Count; i++)
        {
            var obj = Instantiate(beastElementPrefab, beastsList.transform);

            obj.GetComponent<BestiaryUiSlot>().SetData(bestiary.FullBestiary[i], i);
        }

    }

    public void Clear()
    {
        foreach (Transform child in beastsList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetUpSegment(MonsterBase info)
    {
        description.text = info.Description;
        monsterImage.sprite = info.FrontSprite;
    }

    public void SetUpSegmentPartial(MonsterBase info)
    {
        description.text = "Informazioni insufficienti";
        monsterImage.sprite = info.FrontSprite;
    }

}
