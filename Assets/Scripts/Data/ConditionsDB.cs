using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Avvelenato",
                StartMessage = "è stato avvelenato",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.DecreaseHP(monster.MaxHp / 8);
                    monster.StatusChanges.Enqueue($"Il veleno ha effetto su {monster.Base.Name}");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Scottatura",
                StartMessage = "è stato scottato",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.DecreaseHP(monster.MaxHp / 16);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} ha subito danni dalla scottatura");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralisi",
                StartMessage = "è stato paralizzato",
                OnBeforeMove = (Monster monster) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} è paralizzato e non può agire");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Congelato",
                StartMessage = "è stato congelato",
                OnBeforeMove = (Monster monster) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} non è più congelato");
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Addormentato",
                StartMessage = "è stato addormentato",
                OnStart = (Monster monster) =>
                {
                    monster.StatusTime = Random.Range(1, 4);
                },
                OnBeforeMove = (Monster monster) =>
                {

                    if(monster.StatusTime <= 0)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} si è svegliato");
                        return true;
                    }

                    monster.StatusTime--;
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} sta dormendo");
                    return false;
                }
            }
        },

        //Stati volatili
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confuso",
                StartMessage = "è stato confuso",
                OnStart = (Monster monster) =>
                {
                    monster.VolatileStatusTime = Random.Range(1, 4);
                },
                OnBeforeMove = (Monster monster) =>
                {

                    if(monster.VolatileStatusTime <= 0)
                    {
                        monster.CureVolatileStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} si è svegliato");
                        return true;
                    }
                    monster.VolatileStatusTime--;

                    monster.StatusChanges.Enqueue($"{monster.Base.Name} è confuso");

                    if(Random.Range(1, 3) == 1)
                        return true;

                    monster.DecreaseHP(monster.HP / 8);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} è cosi confuso che si colpisce da solo!");
                    return false;
                }
            }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn)
            return 1.5f;

        return 1f;
    }

}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion
}
