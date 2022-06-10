using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image monsterImage;
    [SerializeField] GameObject EvoEffect;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;

    public static EvolutionManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(Monster monster, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        evolutionUI.SetActive(true);

        monsterImage.sprite = monster.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialodText($"{monster.Base.Name} si sta evolvendo!");

        yield return new WaitForSeconds(5f);

        var oldMonster = monster.Base;
        monster.Evolve(evolution);

        monsterImage.sprite = monster.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialodText($"{oldMonster.Name} si è evoluto in {monster.Base.Name}");

        evolutionUI.SetActive(false);
        OnCompleteEvolution?.Invoke();
    }
}
