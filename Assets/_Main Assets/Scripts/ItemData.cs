using System;
using System.Numerics;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemState
    {
        inActive,
        available,

        /*expencive,*/
        equiped,
        ovned
    }

    public string name;
    public Sprite cardImage, cardInactiveImage, cardImagePasif, objSprite;
    public int price = 100;
    public ItemState itemState;
    public GameObject modelPrefeb;

    public bool weapon;

    // [ShowIf("weapon")] public int buyingTime;
    [ShowIf("weapon")] public int damageAmount;
    [ShowIf("weapon")] public int weaponShootBlend;
    [ShowIf("weapon")] public bool longRangeWeapon;
    [HideIf("weapon")] public bool hat;
}