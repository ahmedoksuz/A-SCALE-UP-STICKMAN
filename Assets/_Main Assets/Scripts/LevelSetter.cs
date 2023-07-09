using DG.Tweening;
using GPHive.Core;
using GPHive.Game.Upgrade;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSetter : MonoBehaviour
{
    [SerializeField] private CharacterManage playerCharacterManage;
    [SerializeField] private IncrementalUpgrade IncrementalUpgrade;


    private void Start()
    {
        StartSetLevel();
    }

    public void StartSetLevel()
    {
        for (var i = 0; i < EnemySpawnManager.Instance.startEnemyCount; i++)
            EnemySpawnManager.Instance.CreatEnemy();
    }


    public void SellButton()
    {
        if (playerCharacterManage.realLevel > 1)
        {
            UIManager.Instance.equipedImageForFill.fillAmount = 0;
            ScaleManager.Instance.ScaleManagerReset();
            playerCharacterManage.Reset();

            GameManager.Instance.WinLevel();
            EnemySpawnManager.Instance.ResetAll();
            UIManager.Instance.UpdateCards();
            IncrementalUpgrade.SetUpgrades();
            StartSetLevel();

            UIManager.Instance.TutorialSellButtonActivities();
        }
    }


    public void NextLevelButton()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
        PlayerPrefs.SetInt("Tutorial", 1);
    }
}