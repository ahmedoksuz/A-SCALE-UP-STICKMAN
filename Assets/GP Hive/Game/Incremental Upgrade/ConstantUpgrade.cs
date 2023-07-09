using GPHive.Game.Upgrade;
using UnityEngine;

[CreateAssetMenu(menuName = "GP Hive Objects/Upgrades/Constant Upgrade")]
public class ConstantUpgrade : Upgrade
{
    [SerializeField] private float startPrice;
    [SerializeField] private float pricePerLevel;

    [SerializeField] private int maxLevel;

    public override float GetPrice()
    {
        return startPrice + pricePerLevel * Level;
    }

    public override bool IsMaxLevel()
    {
        return Level == maxLevel;
    }
}