using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectManager : MonoBehaviour
{
    [SerializeField] GameObject particleContainerEffect;
    [SerializeField] GameObject canvasContainerEffect;

    public bool isPlaing = false;

    public IEnumerator PlayParticleEffect(GameObject particleprefab, float duration)
    {
        isPlaing = true;
        var obj = Instantiate(particleprefab, particleContainerEffect.transform.position, Quaternion.identity, particleContainerEffect.transform);
        obj.transform.position = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(duration);
        Destroy(obj.gameObject);
        isPlaing = false;
    }

    public IEnumerator PlayCanvasEffect(AnimationClip effectAnimation, float duration)
    {
        isPlaing = true;
        Debug.Log(effectAnimation.name);
        canvasContainerEffect.GetComponent<Animator>().Play(effectAnimation.name);
        yield return new WaitForSeconds(duration);
        canvasContainerEffect.GetComponent<Animator>().Play("idle");
        isPlaing = false;
    }

    public bool HasCanvasEffect(Move move)
    {
        return move.HasCanvasEffect();
    }

    public bool HasParticleEffect(Move move)
    {
        return move.HasParticleEffect();
    }
}
