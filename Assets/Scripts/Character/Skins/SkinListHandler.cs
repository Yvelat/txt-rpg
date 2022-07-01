using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinListHandler : MonoBehaviour
{
    [SerializeField] GameObject skinListContainer;
    [SerializeField] GameObject skinListElementPrefab;

    [Header("Skins")]
    [SerializeField] List<Skin> skinList;

    private void Start()
    {
        foreach (Transform child in skinListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Skin skin in skinList)
        {
            var obj = Instantiate(skinListElementPrefab, skinListContainer.transform);

            obj.GetComponent<SkinListSlot>().SetData(skin);
        }
    }

}
