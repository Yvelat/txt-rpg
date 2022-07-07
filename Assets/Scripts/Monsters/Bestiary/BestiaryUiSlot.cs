using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryUiSlot : MonoBehaviour
{
    [SerializeField] Image icon;

    public int index;

    public void SetData(BestiaryElement element, int index)
    {
        this.index = index;

        if(element.bState == BeastState.Unknown)
        {

        }
        else if(element.bState == BeastState.Seen)
        {

        }
        else
        {

        }
    }

}
