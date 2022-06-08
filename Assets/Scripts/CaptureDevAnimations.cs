using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureDevAnimations : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Image mainBody;
    [SerializeField] Image net;
    [SerializeField] Image core;

    [SerializeField] Color success;
    [SerializeField] Color failure;

    public bool animating = false;

    public IEnumerator Initialize()
    {
        animating = true;
        yield return mainBody.DOFillAmount(1f, 1f).WaitForCompletion();
        yield return net.DOFade(1f, 1f).WaitForCompletion();
        animating = false;
    }

    public IEnumerator Shake()
    {
        animating = true;
        anim.Play("schake");
        yield return new WaitForSeconds(1.5f);
        anim.Play("idle");
        animating = false;
    }

    public IEnumerator Captured()
    {
        animating = true;
        core.color = success;
        yield return core.DOFade(1f, 1f).WaitForCompletion();
        animating = false;
    }

    public IEnumerator NotCaptured()
    {
        animating = true;
        core.color = failure;
        yield return core.DOFade(1f, 1f).WaitForCompletion();
        animating = false;
    }
}
