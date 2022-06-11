using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public bool IsPlayerUnit
    {
        get
        {
            return isPlayerUnit;
        }
    }

    public BattleHud Hud
    {
        get
        {
            return hud;
        }
    }

    public Monster Monster { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;
    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Monster monster)
    {

        Monster = monster;

        if (!isPlayerUnit)
        {
            image.sprite = Monster.Base.FrontSprite;
        }

        /*Adapt object to Sprite
        var img = this.gameObject.transform as RectTransform;
        var width = Monster.Base.BackSprite.texture.width;
        var height = Monster.Base.BackSprite.texture.height;
        img.sizeDelta = new Vector2(width, height);
        */

        hud.gameObject.SetActive(true);
        hud.SetData(monster);

        transform.localScale = new Vector3(1, 1, 1);
        image.color = originalColor;
        PlayEnterAnimation();

    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {

        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }

        image.transform.DOLocalMoveX(originalPos.x, 1f);

    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            //sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 50f, 0.25f));
        }

        sequence.Append(image.transform.DOLocalMoveY(originalPos.y, 0.25f));

    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.red, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));

    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}