using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image healthLeft;
    [SerializeField] Image healthRight;

    public bool isUpdating { get; private set; }

    public void SetHp(float hpNormalized)
    {
        healthLeft.fillAmount = hpNormalized;
        healthRight.fillAmount = hpNormalized;
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        isUpdating = true;

        float curHP = healthLeft.fillAmount;
        float changeAmt = curHP - newHP;

        while (curHP - newHP > Mathf.Epsilon)
        {
            curHP -= changeAmt * Time.deltaTime;
            healthLeft.fillAmount = curHP;
            healthRight.fillAmount = curHP;
            yield return null;
        }

        healthLeft.fillAmount = newHP;
        healthRight.fillAmount = newHP;

        isUpdating = false;
    }
}
