using System.Collections.Generic;
using System.Numerics;
using GPHive.Game;
using UnityEngine;

public class EnemySpawnManager : Singleton<EnemySpawnManager>
{
    [SerializeField] private GameObject PoolEnemyPrefeb;

    [SerializeField] private int howManyCharactersToSpawn, levelUpAmountDivider;
    [SerializeField] private int maxEnemyCount;
    public int startEnemyCount;

    [HideInInspector] public List<GameObject> activeEnemies = new();
    [SerializeField] private Transform firstEnemyStartPos;
    [SerializeField] private float distanceZBetweenEnemies;
    private int enemyCount = 0;

    [HideInInspector] public int EnemyCount => enemyCount;
    private bool firstTime = true;
    public CharacterManage playerCharacterManage;


    private void Start()
    {
        enemyCount = GetEnemyCountForEndPlayerPref();
    }

    public int GetEnemyCountForEndPlayerPref()
    {
        return PlayerPrefs.GetInt("EnemyCount", 0);
    }

    public void SetEnemyCountForEndPlayerPref(int value)
    {
        PlayerPrefs.SetInt("EnemyCount", value);
    }

    public void SetFirstEnemyLevelPlayerPref(int value)
    {
        PlayerPrefs.SetInt("FirstEnemyLevel", value);
    }

    public int GetFirstEnemyLevelPlayerPref()
    {
        return PlayerPrefs.GetInt("FirstEnemyLevel", 1);
    }

    public void CreatEnemy()
    {
        if (maxEnemyCount > enemyCount)
        {
            var go = ObjectPooling.Instance.GetFromPool(PoolEnemyPrefeb);
            var goCharManage = go.GetComponent<CharacterManage>();
            var enemyLevel = 0;

            if (firstTime)
            {
                if (GetFirstEnemyLevelPlayerPref() > 0 && GetEnemyCountForEndPlayerPref() > 0)
                    enemyLevel = GetFirstEnemyLevelPlayerPref();
                else
                    enemyLevel = LevelCalculater(enemyCount);

                firstTime = false;
            }
            else
            {
                enemyLevel = LevelCalculater(enemyCount);
            }

            enemyCount++;
            goCharManage.SetupRefresh(enemyLevel, firstEnemyStartPos.position, distanceZBetweenEnemies, enemyCount);

            go.SetActive(true);
            activeEnemies.Add(go);
        }
    }


    public void RemoveEnemy(GameObject go, int count)
    {
        SetEnemyCountForEndPlayerPref(count);
        RemoveEnemy(go);
    }


    private int LevelCalculater(int numberOfRows)
    {
        int firstNumber = 1, nextNumber = 0;
        var characterLevel = 1;

        if (numberOfRows == 0)
            return firstNumber;

        for (var j = firstNumber; j <= numberOfRows; j++)
        {
            characterLevel += firstNumber;
            nextNumber = characterLevel;

            if (nextNumber > 20)
                nextNumber += nextNumber / levelUpAmountDivider;
            else
                nextNumber += nextNumber / 2;

            firstNumber = nextNumber;
        }

        return nextNumber;
    }

    public void RemoveEnemy(GameObject Enemy)
    {
        activeEnemies.Remove(Enemy);
        CheckEnemyCountAndCreatEnemy();
    }

    private void CheckEnemyCountAndCreatEnemy()
    {
        if (activeEnemies.Count < howManyCharactersToSpawn)
        {
            var tempCount = startEnemyCount - activeEnemies.Count;
            for (var i = 0; i < tempCount; i++) CreatEnemy();
        }
    }


    public void ResetAll()
    {
        foreach (var _activeEnemies in activeEnemies) _activeEnemies.GetComponent<CharacterManage>().CloseMyGo(true);
        enemyCount = 0;

        SetEnemyCountForEndPlayerPref(0);
        SetFirstEnemyLevelPlayerPref(0);
        activeEnemies.Clear();
    }
}