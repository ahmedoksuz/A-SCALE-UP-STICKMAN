using System;
using System.Collections.Generic;
using System.Numerics;
using GPHive.Game;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public string playerPrefName;
    public List<ItemData> itemDatas = new();
    public ItemData activeItemData;
    [SerializeField] private CharacterCostumizeManager characterCostumizeManager;
    private int _activeItemLevel;
    private bool costumes;

    private void Start()
    {
        PlayerPrefs.GetInt(playerPrefName, 0);
        UpdateLevelAndItem();
        UpdateItemDatas();
        UIManager.Instance.CreateCards(itemDatas);
        UIManager.Instance.UpdateCards();
    }


    public void TryBuy(ItemData itemData)
    {
        if (PlayerEconomy.Instance.GetMoney() > itemData.price || itemData.itemState == ItemData.ItemState.ovned)
        {
            if (itemData.itemState != ItemData.ItemState.ovned)
            {
                PlayerEconomy.Instance.SpendMoney(itemData.price);
                PlayerPrefs.SetInt(playerPrefName, PlayerPrefs.GetInt(playerPrefName) + 1);
                activeItemData = itemData;
                UpdateItemDatas();
                UIManager.Instance.UpdateCards();
                UpdateLevelAndItem();
            }
            else
            {
                activeItemData = itemData;
                UpdateItemDatas();
                UIManager.Instance.UpdateCards();
                UpdateLevelAndItem(itemData);
            }
        }
    }


    public void UpdateItemDatas()
    {
        for (var i = 0; i < itemDatas.Count; i++)
            if (PlayerPrefs.GetInt(playerPrefName) < i)
            {
                if (PlayerPrefs.GetInt(playerPrefName) == i - 1)
                {
                        itemDatas[i].itemState = ItemData.ItemState.available;
                }
                else
                {
                    itemDatas[i].itemState = ItemData.ItemState.inActive;
                }
            }
            else
            {
                if (activeItemData != null && activeItemData == itemDatas[i])
                    itemDatas[i].itemState = ItemData.ItemState.equiped;
                else
                    itemDatas[i].itemState = ItemData.ItemState.ovned;
            }

        if (activeItemData == null) itemDatas[0].itemState = ItemData.ItemState.equiped;
    }


    private void UpdateLevelAndItem()
    {
        if (itemDatas.Count > 0)
        {
            if (activeItemData != null)
                activeItemData.itemState = ItemData.ItemState.ovned;

            if (PlayerPrefs.GetInt(playerPrefName) < itemDatas.Count && PlayerPrefs.GetInt(playerPrefName) >= 0)
            {
                activeItemData = itemDatas[PlayerPrefs.GetInt(playerPrefName)];
            }
            else if (PlayerPrefs.GetInt(playerPrefName) >= itemDatas.Count)
            {
                PlayerPrefs.SetInt(playerPrefName, itemDatas.IndexOf(itemDatas[^1]));
                activeItemData = itemDatas[^1];
            }
            else
            {
                PlayerPrefs.SetInt(playerPrefName, 0);
                activeItemData = itemDatas[0];
            }


            characterCostumizeManager.UpdateItem(activeItemData);
        }
    }

    private void UpdateLevelAndItem(ItemData itemData)
    {
        characterCostumizeManager.UpdateItem(itemData);
    }
}