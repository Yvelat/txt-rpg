using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject dgChose;
    [SerializeField] GameObject adventuring;
    [SerializeField] GameObject partyScreen;
    [SerializeField] GameObject inventoryScreen;
    [SerializeField] SingleAudioManager mainMenuMusic;

    public event Action<int> onMenuSelected;
    public event Action onBack;

    private void Start()
    {
        mainMenuMusic.gameObject.SetActive(true);
        mainMenuMusic.ResetAndPlay();
    }

    public void OpenMenu()
    {
        mainMenu.SetActive(true);
    }

    public void CloseMenu()
    {
        mainMenu.SetActive(false);
    }

    public void BtStart()
    {
        Debug.Log("Start Pressed");
        mainMenu.SetActive(false);
        dgChose.SetActive(true);
    }

    public void BtExit()
    {
        Debug.Log("Exit Pressed");
        //TODO: exit Game
    }

    public void BtInventory()
    {
        Debug.Log("Inventory Pressed");
        mainMenu.SetActive(false);
        inventoryScreen.SetActive(true);
        GameController.Instance.SetStateToBag();
    }

    public void BtParty()
    {
        Debug.Log("Party Pressed");
        mainMenu.SetActive(false);
        partyScreen.SetActive(true);
        GameController.Instance.SetStateToParty();
    }

    public void HandleUpdate()
    {
        /*int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        if (prevSelection != selectedItem)
            //UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            CloseMenu();
        }*/
    }
}
