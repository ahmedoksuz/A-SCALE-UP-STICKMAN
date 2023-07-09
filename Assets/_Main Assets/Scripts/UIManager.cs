using System;
using System.Collections.Generic;
using System.Numerics;
using GPHive.Core;
using GPHive.Game;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject tapToStart,
        upgradeMain,
        backPacMain,
        upgradeButton,
        bacPacButton,
        sellButton,
        tapToAtackFaster,
        hand,
        hand2;

    [SerializeField] private GameObject conteinerWeapons, conteinerAccessories;
    [SerializeField] private Transform scrollParentWeapons, scrollParentAccessories, weaponBuyingAreaConteiner;
    private List<ItemCard> weaponCards = new();
    private List<ItemCard> accessoriesCards = new();
    private List<ItemCard> weaponBuyingAreaCards = new();
    [SerializeField] private GameObject cardPoolPrefeb;
    [SerializeField] private ItemManager _weaponItemManagerManager;
    [SerializeField] private ItemManager _accessoriesItemManagerManger;
    [SerializeField] private ItemData nullWeaponData, nullAccessorieData;
    [SerializeField] private GameObject winLevelUI, customUI;
    [HideInInspector] public Image equipedImageForFill;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) < 1) PlayerPrefs.DeleteAll();
    }

    public void SetLevelText(TextMeshProUGUI characterLevelTxt, int level)
    {
        characterLevelTxt.text = "Lvl " + PlayerEconomy.Instance.ConvertToKBM(level);
    }


    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) < 1 && justPlayOnTimeTutorial)
            EventManager.LevelSuccessed += LevelFinished;
        EventManager.LevelStarted += LevelStart;
    }

    private bool justPlayOnTimeTutorial = true;

    private void LevelStart()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) < 1)
        {
            justPlayOnTimeTutorial = false;
            EventManager.LevelStarted -= LevelStart;
            Invoke(nameof(TapToAtackFasterOpener), 5);
            Invoke(nameof(TapToAtackFasterCloser), 10);
        }
    }

    private void OnDisable()
    {
        EventManager.LevelSuccessed -= LevelFinished;
    }

    private void LevelFinished()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) >= 1)
        {
            tapToStart.SetActive(true);
            upgradeButton.SetActive(true);
            bacPacButton.SetActive(true);
        }
    }

    public void WinLevel()
    {
        GameManager.Instance.WinLevel();

        customUI.SetActive(false);
        winLevelUI.SetActive(true);
    }


    private void Start()
    {
        scrollParentAccessories.GetComponent<RectTransform>().position -= new Vector3(0, 1000, 0);
        scrollParentWeapons.GetComponent<RectTransform>().position -= new Vector3(0, 1000, 0);

        if (PlayerPrefs.GetInt("Tutorial", 0) >= 1)
            CloseButtonClick();
        else
            TutorialStart();
    }

    public void CreateCards(List<ItemData> itemDataList)
    {
        for (var i = 0; i < itemDataList.Count; i++)
        {
            var go = ObjectPooling.Instance.GetFromPool(cardPoolPrefeb);
            var itemCard = go.GetComponent<ItemCard>();
            itemCard.cardItemData = itemDataList[i];

            if (itemDataList[i].weapon)
            {
                weaponCards.Add(itemCard);
                go.transform.SetParent(scrollParentWeapons);
            }
            else
            {
                accessoriesCards.Add(itemCard);
                go.transform.SetParent(scrollParentAccessories);
            }

            go.transform.localScale = Vector3.one;
            go.SetActive(true);
        }

        if (weaponBuyingAreaCards.Count == 0)
            for (var i = 0; i < 2; i++)
            {
                var go = ObjectPooling.Instance.GetFromPool(cardPoolPrefeb);
                var itemCard = go.GetComponent<ItemCard>();
                itemCard.cardItemData = itemDataList[i];

                weaponBuyingAreaCards.Add(itemCard);
                go.transform.SetParent(weaponBuyingAreaConteiner);

                if (i == 0) go.transform.localScale = Vector3.one * 1.2f;
                else
                    go.transform.localScale = Vector3.one;
                go.SetActive(true);
            }

        equipedImageForFill = weaponBuyingAreaCards[0].equipedImage;
    }

    public void UpdateCards()
    {
        _weaponItemManagerManager.UpdateItemDatas();
        _accessoriesItemManagerManger.UpdateItemDatas();

        BuyingAreaCardsUpdate();

        foreach (var card in weaponCards) card.UpdateCard();
        foreach (var card in accessoriesCards) card.UpdateCard();
        foreach (var card in weaponBuyingAreaCards) card.UpdateCard();
    }

    private void BuyingAreaCardsUpdate()
    {
        for (var i = 0; i < _weaponItemManagerManager.itemDatas.Count; i++)
        {
            var tempItemData = _weaponItemManagerManager.itemDatas[i];

            if (tempItemData.itemState == ItemData.ItemState.equiped)
                weaponBuyingAreaCards[0].cardItemData = tempItemData;

            if (i > 0)
                if (_weaponItemManagerManager.itemDatas[i - 1].itemState == ItemData.ItemState.equiped &&
                    tempItemData.itemState == ItemData.ItemState.available)
                    weaponBuyingAreaCards[1].cardItemData = tempItemData;
        }

        if (weaponBuyingAreaCards[0].cardItemData == weaponBuyingAreaCards[1].cardItemData)
            weaponBuyingAreaCards[1].gameObject.SetActive(false);
        else
            weaponBuyingAreaCards[1].gameObject.SetActive(true);
    }


    public void TryBuy(ItemData itemData)
    {
        if (itemData.weapon)
            _weaponItemManagerManager.TryBuy(itemData);
        else
            _accessoriesItemManagerManger.TryBuy(itemData);
    }


    public void UpgradeButtonClick()
    {
        upgradeMain.SetActive(true);
        backPacMain.SetActive(false);

        bacPacButton.SetActive(false);
        upgradeButton.SetActive(false);
        sellButton.SetActive(false);

        if (PlayerPrefs.GetInt("Tutorial", 0) < 1)
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            hand2.SetActive(false);
        }
    }

    public void BackPacButtonClick()
    {
        upgradeMain.SetActive(false);
        backPacMain.SetActive(true);

        bacPacButton.SetActive(false);
        upgradeButton.SetActive(false);
        sellButton.SetActive(false);

        if (PlayerPrefs.GetInt("Tutorial", 0) < 1)
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            hand2.SetActive(false);
        }
    }

    public void CloseButtonClick()
    {
        upgradeMain.SetActive(false);
        backPacMain.SetActive(false);
        conteinerWeapons.SetActive(true);
        conteinerAccessories.SetActive(false);
        weaponBuyingAreaConteiner.gameObject.SetActive(true);
        bacPacButton.SetActive(true);
        upgradeButton.SetActive(true);
        sellButton.SetActive(true);
        tapToAtackFaster.SetActive(false);
        hand.SetActive(false);
        hand2.SetActive(false);
    }

    public void TutorialStart()
    {
        upgradeMain.SetActive(false);
        backPacMain.SetActive(false);
        conteinerWeapons.SetActive(false);
        conteinerAccessories.SetActive(false);
        weaponBuyingAreaConteiner.gameObject.SetActive(false);
        bacPacButton.SetActive(false);
        upgradeButton.SetActive(false);
        sellButton.SetActive(false);
        tapToAtackFaster.SetActive(false);
    }

    private bool justOneTime2 = true;

    public void CheckPlayerLevel()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) < 1)
        {
            if (EnemySpawnManager.Instance.playerCharacterManage.realLevel > 25 && justOneTime2)
            {
                justOneTime2 = false;
                hand.SetActive(true);
                sellButton.SetActive(true);
            }

            if (PlayerEconomy.Instance.GetMoney() > 25 && !justOneTime2)
            {
                CloseButtonClick();
                hand2.SetActive(true);
            }
        }
    }

    public void TutorialSellButtonActivities()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) < 1)
        {
            hand.SetActive(false);
            weaponBuyingAreaConteiner.gameObject.SetActive(true);
            tapToStart.SetActive(true);
        }

        tapToStart.SetActive(true);
    }

    private void TapToAtackFasterOpener()
    {
        tapToAtackFaster.SetActive(true);
    }

    private void TapToAtackFasterCloser()
    {
        tapToAtackFaster.SetActive(false);
    }

    public void WaponButtonClick()
    {
        conteinerWeapons.SetActive(true);
        conteinerAccessories.SetActive(false);
    }

    public void AccessoriesButtonClick()
    {
        conteinerWeapons.SetActive(false);
        conteinerAccessories.SetActive(true);
    }

    private void OnApplicationQuit()
    {
        if (winLevelUI.activeSelf)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("Tutorial", 1);
        }
    }
}