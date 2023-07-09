using GPHive.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [SerializeField] private Image bacgroundImage, objImage;
    public Image equipedImage;
    [SerializeField] private Button cardButton;
    [SerializeField] private TextMeshProUGUI moneyText, nameText;
    public ItemData cardItemData;

    public void UpdateCard()
    {
        if (cardItemData != null)
            switch (cardItemData.itemState)
            {
                case ItemData.ItemState.inActive:
                    bacgroundImage.sprite = cardItemData.cardInactiveImage;
                    moneyText.enabled = false;
                    nameText.enabled = false;
                    objImage.enabled = false;
                    cardButton.enabled = false;
                    equipedImage.enabled = false;

                    break;

                case ItemData.ItemState.available:


                    objImage.sprite = cardItemData.objSprite;
                    moneyText.enabled = true;
                    moneyText.text = "$ " + PlayerEconomy.Instance.ConvertToKBM(cardItemData.price);
                    nameText.enabled = true;
                    nameText.text = cardItemData.name;
                    objImage.enabled = true;
                    cardButton.enabled = true;
                    equipedImage.enabled = false;
                    if (PlayerEconomy.Instance.GetMoney() >= cardItemData.price)
                        bacgroundImage.sprite = cardItemData.cardImage;
                    else
                        bacgroundImage.sprite = cardItemData.cardImagePasif;
                    break;

                /*case ItemData.ItemState.expencive:
                    bacgroundImage.sprite = cardItemData.cardImage;
                    objImage.sprite = cardItemData.objSprite;
                    moneyText.enabled = true;
                    moneyText.text = "$ " + PlayerEconomy.Instance.ConvertToKBM(cardItemData.price);
                    nameText.enabled = true;
                    nameText.text = cardItemData.name;
                    objImage.enabled = true;
                    cardButton.enabled = false;
                    equipedImage.enabled = false;
                    break;*/

                case ItemData.ItemState.ovned:
                    bacgroundImage.sprite = cardItemData.cardImage;
                    objImage.sprite = cardItemData.objSprite;
                    moneyText.enabled = true;
                    moneyText.text = "OWNED";
                    nameText.enabled = true;
                    nameText.text = cardItemData.name;
                    objImage.enabled = true;
                    cardButton.enabled = true;
                    equipedImage.enabled = false;
                    break;
                case ItemData.ItemState.equiped:
                    bacgroundImage.sprite = cardItemData.cardImage;
                    objImage.sprite = cardItemData.objSprite;
                    moneyText.enabled = true;
                    moneyText.text = "EQUIPED";
                    nameText.enabled = true;
                    nameText.text = cardItemData.name;
                    objImage.enabled = true;
                    cardButton.enabled = false;
                    equipedImage.enabled = true;
                    break;
            }
    }

    public void ButtonClicted()
    {
        UIManager.Instance.TryBuy(cardItemData);
    }
}