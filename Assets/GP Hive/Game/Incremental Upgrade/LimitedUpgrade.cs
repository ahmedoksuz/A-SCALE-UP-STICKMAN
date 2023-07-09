using GPHive.Game.Upgrade;
using UnityEngine;

[CreateAssetMenu(menuName = "GP Hive Objects/Upgrades/Limited Upgrade")]
public class LimitedUpgrade : Upgrade
{
    public float[] UpgradePrices;

    public override float GetPrice()
    {
        return UpgradePrices[level];
    }

    public override bool IsMaxLevel()
    {
        return level + 1 == UpgradePrices.Length;
    }
}