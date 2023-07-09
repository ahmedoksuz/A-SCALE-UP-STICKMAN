using System;
using System.Numerics;
using UnityEngine;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;


namespace GPHive.Game
{
    public class PlayerEconomy : Singleton<PlayerEconomy>
    {
        [SerializeField] private TextMeshProUGUI currencyText;

        public float GetMoney()
        {
            return PlayerPrefs.GetFloat("Player Currency", 0);
        }

        private void SetMoney(float amount)
        {
            PlayerPrefs.SetFloat("Player Currency", (float)Math.Round(amount, 2));
        }

        [SerializeField] private bool moneyTextAnimationEnabled;

        public GameEvent OnMoneyChange;

        [Button]
        private void Add10Coin()
        {
            AddMoney(10);
        }

        [Button]
        private void Add100Coin()
        {
            AddMoney(100);
        }

        [Button]
        private void Add1000Coin()
        {
            AddMoney(100000);
        }

        private void Start()
        {
            SetMoneyText();
        }


        /// <summary>
        /// Returns true if player have enough currency.
        /// </summary>
        /// <param name="spendAmount">Currency amount to spend</param>
        /// <returns></returns>
        public bool SpendMoney(float spendAmount)
        {
            if (GetMoney() < spendAmount) return false;

            var _oldMoney = GetMoney();
            SetMoney(GetMoney() - spendAmount);
            SetMoneyText();


            OnMoneyChange.Raise();
            return true;
        }


        public void AddMoney(float amount)
        {
            var _oldMoney = GetMoney();
            SetMoney(GetMoney() + amount);
            SetMoneyText();

            OnMoneyChange.Raise();
        }

        public bool CheckEnoughMoney(float amount)
        {
            return GetMoney() >= amount;
        }


        private void SetMoneyText()
        {
            currencyText.text = ConvertToKBM(GetMoney());
        }

        private static string[] _suffix = { "", "K", "M", "B", "T", "Q", "QU", "S" };

        public string ConvertToKBM(float value)
        {
            if (value >= 1000)
            {
                var _count = 0;
                while (value >= 1000f)
                {
                    _count++;
                    value /= 1000f;
                }

                return value < .01f && value != 0 ? $"{value:0.000}{_suffix[_count]}" : $"{value:0.0}{_suffix[_count]}";
            }
            else
            {
                if (value - (int)value > 0)
                    return $"{value:0.0}";
                else
                    return $"{value:0}";
            }
        }
    }
}