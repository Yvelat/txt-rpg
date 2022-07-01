using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBootHandler : MonoBehaviour
{
    [SerializeField] GameObject skinSelector;
    [SerializeField] GameObject nameSelector;
    [SerializeField] FirstBootReview review;

    public string playerName { get; private set; }
    public Skin playerSkin { get; private set; }

    private void Start()
    {
        Restart();
    }

    public void SetSkin(Skin skin)
    {
        playerSkin = skin;
        skinSelector.SetActive(false);
        nameSelector.SetActive(true);
    }

    public void SetName(string name)
    {
        playerName = name;
        nameSelector.SetActive(false);
        review.gameObject.SetActive(true);
        review.SetReview(playerName, playerSkin);
    }

    public void Restart()
    {
        nameSelector.SetActive(false);
        skinSelector.SetActive(true);
        review.gameObject.SetActive(false);
    }

}
