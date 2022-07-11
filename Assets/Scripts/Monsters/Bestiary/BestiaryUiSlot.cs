using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryUiSlot : MonoBehaviour
{
    [SerializeField] Image icon;

    public int index;

    MonsterBase monster;

    BeastState state;

    public void SetData(BestiaryElement element, int index)
    {
        this.index = index;
        monster = element.Monster;

        if(element.bState == BeastState.Unknown)
        {

        }
        else if(element.bState == BeastState.Seen)
        {

        }
        else
        {

        }

        state = element.bState;
    }

    public void onClick()
    {
        if (state == BeastState.Captured)
            FindObjectOfType<BestiaryUI>().SetUpSegment(monster);
        else if (state == BeastState.Seen)
            FindObjectOfType<BestiaryUI>().SetUpSegmentPartial(monster);
    }

}
