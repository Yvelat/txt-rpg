using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class STBar : MonoBehaviour
{
    [SerializeField] Image staminaLeft;
    [SerializeField] Image staminaRight;

    public bool isUpdating { get; private set; }

    public void SetStamina(float staminaNormalized)
    {
        staminaLeft.fillAmount = staminaNormalized;
        staminaRight.fillAmount = staminaNormalized;
    }

    public IEnumerator SetStaminaSmooth(float newStamina)
    {
        isUpdating = true;

        float curStamina = staminaLeft.fillAmount;
        float changeAmt = curStamina - newStamina;

        while (curStamina - newStamina > Mathf.Epsilon)
        {
            curStamina -= changeAmt * 10f * Time.deltaTime;
            staminaLeft.fillAmount = curStamina;
            staminaRight.fillAmount = curStamina;
            yield return null;
        }

        staminaLeft.fillAmount = newStamina;
        staminaRight.fillAmount = newStamina;

        isUpdating = false;
    }
}
