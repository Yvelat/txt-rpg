using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] string trainerName;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    bool battleLost = false;

    public IEnumerator Interact(Transform initiator)
    {
        

        if (!battleLost)
        {
            yield return DialogManager.Instance.ShowDialog(dialog);
            GameController.Instance.StartTrainerBattle(this);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;

        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        

        yield return DialogManager.Instance.ShowDialog(dialog);
        GameController.Instance.StartTrainerBattle(this);
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if(battleLost)
            fov.gameObject.SetActive(false);

    }

    public string Name
    {
        get => trainerName;
    }
    public Sprite Sprite
    {
        get => sprite;
    }
}
