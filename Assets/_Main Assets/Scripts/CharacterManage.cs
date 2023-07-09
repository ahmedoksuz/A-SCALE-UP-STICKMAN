using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using GPHive.Game;
using MoreMountains.NiceVibrations;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using NaughtyAttributes;
using UnityEngine.UI;

public class CharacterManage : MonoBehaviour
{
    [HideInInspector] public int realLevel;
    private int health, maxHealth;
    [SerializeField] private TextMeshProUGUI characterLevelTxt;
    private CharacterMovementAndShooting _characterMovementAndShooting;
    private Collider _collider;
    [HideIf("enemy")] [SerializeField] private CoefficientUpgrade incomeUpgrade;
    [HideIf("enemy")] [SerializeField] private float startIncomeUpgradeMultiple, levelByAmountOfIncrease;
    public float characterLevelChangingTime;
    [HideIf("enemy")] [SerializeField] private ParticleSystem upgrade;
    [SerializeField] private bool enemy;

    [ShowIf("enemy")] [SerializeField] private ParticleSystem /*downgrade,*/ chracterHit, charDeadParticle;

    [ShowIf("enemy")] [SerializeField] private GameObject charDead;
    [ShowIf("enemy")] public ParticleSystem enemyWalk;
    [HideInInspector] public int myCount;
    [ShowIf("enemy")] [SerializeField] private GameEvent enemyDie;
    public Animator _animator;
    private bool _isColliderNotNull;

    [ShowIf("enemy")] [SerializeField] private List<GameObject> Skins = new();
    [ShowIf("enemy")] [SerializeField] private List<Color> DeathParticleColor = new();
    [ShowIf("enemy")] [SerializeField] private TextMeshPro referanceTakeDamageTextMeshPro;
    [ShowIf("enemy")] [SerializeField] private GameObject takeDamageTxtGameObj;
    [ShowIf("enemy")] [SerializeField] private SlicedFilledImage healthBarFiiler, arcadeBarFiiler;
    [ShowIf("enemy")] [SerializeField] private Texture atlas1, atlas2;
    private static readonly int Play = Animator.StringToHash("Play");

    //   [ShowIf("enemy")] [SerializeField] private List<Rigidbody> ragdollRigiddbodies = new();

    public void SetHealth(int level, int damageAmount)
    {
        health = damageAmount * 2 + (realLevel - level) / 2;
        maxHealth = health;
        healthBarFiiler.fillAmount = 1;
        arcadeBarFiiler.fillAmount = 1;
    }

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _isColliderNotNull = _collider != null;
    }

    private int skinNumber;
    private static readonly int Idle = Animator.StringToHash("Idle");

    private void EnemySkinChange()
    {
        if (enemy)
        {
            foreach (var skin in Skins)
                skin.SetActive(false);
            skinNumber = Random.Range(0, Skins.Count);
            Skins[skinNumber].SetActive(true);

            MaterialPropertyBlock propertyBlock = new();
            var myRenderer = Skins[skinNumber].GetComponent<SkinnedMeshRenderer>();


            if (Skins[skinNumber].name == "devil")
                myRenderer.GetPropertyBlock(propertyBlock, 0);
            else
                myRenderer.GetPropertyBlock(propertyBlock, 1);


            if (Random.Range(0, 2) == 0)
                propertyBlock.SetTexture("_BaseMap", atlas1);
            else
                propertyBlock.SetTexture("_BaseMap", atlas2);

            if (Skins[skinNumber].name == "devil")
                myRenderer.SetPropertyBlock(propertyBlock, 0);
            else
                myRenderer.SetPropertyBlock(propertyBlock, 1);

            arcadeBarFiiler.transform.parent.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (!enemy)
        {
            SetLevel(GetLevel());
            _characterMovementAndShooting = GetComponent<CharacterMovementAndShooting>();
        }
        else
        {
            EnemySkinChange();
            characterLevelTxt.enabled = true;
            _animator.enabled = true;
        }

        SetupRefresh(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (enemy && other.CompareTag("Bullet")) other.gameObject.SetActive(false);
    }

    public void SetupRefresh(bool start)
    {
        if (_isColliderNotNull)
            _collider.enabled = true;

        ScaleManager.Instance.ScaleCalculationAndSet(start, GetLevel(), gameObject, enemy);
        UIManager.Instance.SetLevelText(characterLevelTxt, GetLevel());
    }


//For Enemy
    public void SetupRefresh(int level, Vector3 firstEnemyStartPos, float distanceZBetweenEnemies,
        int enemyCount)
    {
        if (_isColliderNotNull)
            _collider.enabled = true;

        SetLevel(level);
        UIManager.Instance.SetLevelText(characterLevelTxt, GetLevel());
        myCount = enemyCount;
        ScaleManager.Instance.ScaleCalculationAndSet(false, GetLevel(), gameObject, enemy);
        EnemyPosFindAndSet(firstEnemyStartPos, distanceZBetweenEnemies);
    }

    private void EnemyPosFindAndSet(Vector3 firstEnemyStartPos, float distanceZBetweenEnemies)
    {
        var myTransform = transform;

        var _firstEnemyStartPos = firstEnemyStartPos;
        myTransform.position = _firstEnemyStartPos;

        if (EnemySpawnManager.Instance.activeEnemies.Count > 0)
        {
            var TargetEnemy = EnemySpawnManager.Instance.activeEnemies[^1].GetComponent<CharacterManage>();

            var scaleZMe = ScaleManager.Instance.ScaleAmountFinder(GetLevel(), false).z;
            var scaleZEnemy = ScaleManager.Instance.ScaleAmountFinder(TargetEnemy.GetLevel(), true).z;

            var calculatedDefoultDistanceZTargetEnemy =
                distanceZBetweenEnemies * scaleZMe * ScaleManager.Instance.startScaleSize.z;

            var distanceFromEnemy =
                Vector3.forward * ((scaleZMe + scaleZEnemy) / 2 + calculatedDefoultDistanceZTargetEnemy);
            var targetMovePos = TargetEnemy.transform.position + distanceFromEnemy;

            myTransform.position = targetMovePos;
        }
    }


    public void TakeDamage(int damageAmount, int attackingLevel)
    {
        var returnLevel = GetLevel();
        var enemySpawnManager = EnemySpawnManager.Instance;
        enemySpawnManager.SetFirstEnemyLevelPlayerPref(GetLevel());

        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        CinemachineShake.Instance.ShakeCamera(.7f, .4f);
        var obj = ObjectPooling.Instance.GetFromPool(takeDamageTxtGameObj);

        obj.GetComponent<EnemyDamageText>().Setup(damageAmount, referanceTakeDamageTextMeshPro.transform);

        health -= damageAmount;
        healthBarFiiler.fillAmount = (float)health / (float)maxHealth;

        if (FillWithinSecondsTempCo != null) StopCoroutine(FillWithinSecondsTempCo);
        FillWithinSecondsTempCo =
            StartCoroutine(FillWithinSeconds(arcadeBarFiiler, (float)health / (float)maxHealth, .2f));

        if (attackingLevel > GetLevel() || (returnLevel == 1 && attackingLevel == 1) || returnLevel - damageAmount <= 0)
        {
            enemySpawnManager.playerCharacterManage.AddLevel(returnLevel);
            Die();
        }
        else
        {
            chracterHit.Play();
            DecreaseLevel(damageAmount);
            enemySpawnManager.playerCharacterManage.AddLevel(damageAmount);
        }
    }

    private Coroutine FillWithinSecondsTempCo;

    private IEnumerator FillWithinSeconds(SlicedFilledImage arcadeBarFiiler, float to, float duration)
    {
        var timePassed = 0f;
        var startAmount = arcadeBarFiiler.fillAmount;
        while (timePassed < duration)
        {
            arcadeBarFiiler.fillAmount = Mathf.Lerp(startAmount, to, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }


        arcadeBarFiiler.fillAmount = (float)health / (float)maxHealth;
        if (arcadeBarFiiler.fillAmount == 0) arcadeBarFiiler.transform.parent.gameObject.SetActive(false);
    }


    private void Die()
    {
        if (enemy)
        {
            EnemySpawnManager.Instance.RemoveEnemy(gameObject, myCount);
            _collider.enabled = false;
            enemyDie.Raise();
            characterLevelTxt.enabled = false;
            Invoke(nameof(CloseMyGo), .3f);
            //  Invoke(nameof(CloseMyGo), 1.5f);
            PlayerPrefs.SetInt("EnemyDieCount", PlayerPrefs.GetInt("EnemyDieCount") + 1);
        }
        else
        {
        }
    }

    public void CloseMyGo(bool EnemySpawnManager)
    {
        gameObject.SetActive(false);
    }

    public void CloseMyGo()
    {
        if (FillWithinSecondsTempCo != null) StopCoroutine(FillWithinSecondsTempCo);
        var go = ObjectPooling.Instance.GetFromPool(charDead);

        go.transform.position = charDeadParticle.transform.position;
        go.transform.localScale = charDeadParticle.transform.lossyScale;
        go.SetActive(true);
        var goParticleSystem = go.GetComponent<ParticleSystem>();
        goParticleSystem.startColor = DeathParticleColor[skinNumber];
        goParticleSystem.startSize = realLevel * .000000001f + .25f;
        goParticleSystem.Play();
        go.transform.parent = EnemySpawnManager.Instance.playerCharacterManage.transform;

        gameObject.SetActive(false);
    }


    public void Reset()
    {
        PlayerPrefs.SetInt("EnemyDieCount", 0);
        _characterMovementAndShooting.Reset();
        PlayerEconomy.Instance.AddMoney(IncomeUpgradeCalculateValue());
        SetLevel(1);
        _animator.SetTrigger(Idle);
        _animator.ResetAll();
        SetupRefresh(false);
    }

    public float IncomeUpgradeCalculateValue()
    {
        return GetLevel() * (startIncomeUpgradeMultiple + levelByAmountOfIncrease * incomeUpgrade.Level);
    }


    public void SetLevel(int level)
    {
        realLevel = level;
        if (!enemy)
            PlayerPrefs.SetInt("realLevel", realLevel);
    }

    public void AddLevel(int level)
    {
        realLevel += level;
        if (!enemy)
        {
            upgrade.Play();
            PlayerPrefs.SetInt("realLevel", realLevel);
            SetupRefresh(false);
        }
    }

    public void DecreaseLevel(int level)
    {
        realLevel -= level;

        if (!enemy)
            PlayerPrefs.SetInt("realLevel", realLevel);
        else
            UIManager.Instance.SetLevelText(characterLevelTxt, GetLevel());
        //   downgrade.Play();
    }

    public int GetLevel()
    {
        if (!enemy)
            realLevel = PlayerPrefs.GetInt("realLevel", 1);
        return realLevel;
    }
}


[Serializable]
public class CharacterLevel
{
    public int minLevel;
    public int maxLevel;
    public GameObject modelGo;
}