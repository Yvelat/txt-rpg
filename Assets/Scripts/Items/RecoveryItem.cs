using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Crea nuovo item di recupero")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("Stamina")]
    [SerializeField] int stAmount;
    [SerializeField] bool restoreMaxST;

    [Header("Condizioni di Stato")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Rianima")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Monster monster)
    {
        if(revive || maxRevive)
        {
            if (monster.HP > 0) return false;

            if (revive)
                monster.IncreaseHP(monster.MaxHp / 2);
            else if (maxRevive)
                monster.IncreaseHP(monster.MaxHp);

            monster.CureStatus();

            return true;
        }

        if (monster.HP <= 0) return false;

        if (restoreMaxHP || hpAmount > 0)
        {
            if (monster.HP == monster.MaxHp) return false;

            if (restoreMaxHP)
                monster.IncreaseHP(monster.MaxHp);
            else
                monster.IncreaseHP(hpAmount);
        }

        if(restoreMaxST || stAmount > 0)
        {
            if (monster.Stamina == monster.MaxStamina) return false;

            if (restoreMaxST)
                monster.IncreaseStamina(monster.MaxStamina);
            else
                monster.IncreaseStamina(stAmount);

        }

        if(recoverAllStatus || status != ConditionID.none)
        {
            if (monster.Status == null && monster.VolatileStatus == null) return false;

            if (recoverAllStatus)
            {
                monster.CureStatus();
                monster.CureVolatileStatus();
            }
            else
            {
                if (monster.Status.Id == status)
                    monster.CureStatus();
                else if (monster.VolatileStatus.Id == status)
                    monster.CureVolatileStatus();
                else
                    return false;


            }
        }

        return true;
    }
}
