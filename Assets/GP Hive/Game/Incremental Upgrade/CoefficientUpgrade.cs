using GPHive.Game.Upgrade;
using UnityEngine;

[CreateAssetMenu(menuName = "GP Hive Objects/Upgrades/Coefficient Upgrade")]
public class CoefficientUpgrade : Upgrade
{
    [SerializeField] private float startPrice;
    [SerializeField] private float lecelCoefficient;

    [Tooltip("-1 is infinity")] [SerializeField]
    private int maxLevel = -1;


    public override float GetPrice()
    {
        var _price = startPrice;

        for (var i = 0; i < level; i++) _price *= lecelCoefficient;

        return _price;
    }

    public override bool IsMaxLevel()
    {
        return Level == maxLevel;
    }
}