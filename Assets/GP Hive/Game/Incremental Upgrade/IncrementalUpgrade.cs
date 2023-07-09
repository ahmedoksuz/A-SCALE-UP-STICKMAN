using System;
using System.Collections.Generic;
using System.Linq;
using GPHive.Core;
using UnityEngine;

namespace GPHive.Game.Upgrade
{
    public class IncrementalUpgrade : MonoBehaviour
    {
        public List<SerializableDictionary<Upgrade, UpgradeButton>> upgrades;


        private void OnEnable()
        {
            SetUpgrades();
        }

        public void SetUpgrades()
        {
            foreach (var _upgrade in upgrades)
            {
                _upgrade.Key.SetLevel();
                if (CheckMaxLevel(_upgrade)) continue;

                _upgrade.Value.LevelText.SetText($"LVL {_upgrade.Key.Level + 1}");
                _upgrade.Value.PriceText.SetText($"${PlayerEconomy.Instance.ConvertToKBM(_upgrade.Key.GetPrice())}");
            }

            CheckUpgradesBuyable();
        }

        public void CheckUpgradesBuyable()
        {
            foreach (var _upgrade in upgrades.Where(upgrade => !CheckMaxLevel(upgrade)))
                _upgrade.Value.Button.interactable = _upgrade.Key.IsBuyable();
        }

        private static bool CheckMaxLevel(SerializableDictionary<Upgrade, UpgradeButton> upgrade)
        {
            if (!upgrade.Key.IsMaxLevel()) return false;

            upgrade.Value.LevelText.SetText("MAX");
            upgrade.Value.PriceText.gameObject.SetActive(false);
            upgrade.Value.Button.interactable = false;
            return true;
        }

        public void Upgrade(Upgrade upgrade)
        {
            if (!PlayerEconomy.Instance.SpendMoney(upgrade.GetPrice())) return;
            upgrade.BuyUpgrade();
            SetUpgrades();
            UIManager.Instance.UpdateCards();
        }
    }
}