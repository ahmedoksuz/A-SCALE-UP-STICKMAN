using System.Collections.Generic;
using UnityEngine;

public class CharacterCostumizeManager : MonoBehaviour
{
    [SerializeField] private Transform weaponSpawnTransform, assoccieSpawnTransform;
    public ItemData weapon, accessorie;


    private GameObject spawnedWeaponModel, spawnedAccessorieModel;

    public List<Skin> Skins = new();
    public List<ShootingPoint> ShootingPoints = new();

    public void UpdateItem(ItemData itemmData)
    {
        if (itemmData.weapon)
        {
            if (weapon != null && spawnedWeaponModel != null) Destroy(spawnedWeaponModel);
            weapon = itemmData;
            spawnedWeaponModel = Instantiate(weapon.modelPrefeb, weaponSpawnTransform);
        }
        else
        {
            if (accessorie != null)
            {
                accessorie = itemmData;
                var hasSkin = false;
                if (spawnedAccessorieModel != null) Destroy(spawnedAccessorieModel);
                if (accessorie.hat)
                    spawnedAccessorieModel = Instantiate(accessorie.modelPrefeb, assoccieSpawnTransform);
                else
                    foreach (var skin in Skins)
                        if (skin.skinName == accessorie.name)
                        {
                            skin.skinGameObje.SetActive(true);
                            hasSkin = true;
                        }
                        else
                        {
                            skin.skinGameObje.SetActive(false);
                        }


                if (!hasSkin) Skins[0].skinGameObje.SetActive(true);
            }
        }
    }

    public Transform ReturnShootingPoint()
    {
        foreach (var point in ShootingPoints)
            if (weapon.name == point.skinName)
                return point.shootingPoint;

        return null;
    }
}

[System.Serializable]
public class Skin
{
    public string skinName;
    public GameObject skinGameObje;
}

[System.Serializable]
public class ShootingPoint
{
    public string skinName;
    public Transform shootingPoint;
}