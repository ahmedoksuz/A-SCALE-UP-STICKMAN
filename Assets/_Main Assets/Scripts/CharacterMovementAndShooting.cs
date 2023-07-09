using System;
using System.Collections;
using DG.Tweening;
using GPHive.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterMovementAndShooting : MonoBehaviour
{
    [SerializeField] private float chracterMovmentSec, boostTime;
    [SerializeField] private CharacterManage playerCharacterManage;
    [SerializeField] private float defoultDistanceZTargetEnemy;

    private CharacterManage TargetEnemy;

    [SerializeField] private CharacterCostumizeManager _characterCostumizeManager;
    [SerializeField] private CoefficientUpgrade atackSpeedUpgrade;
    [SerializeField] private float startAtacSpeedUpgradeMultiple, byLevelAtacSpeedReductionAmount, minAtacSpeedAmount;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem muzleFlash;

    [SerializeField] private GameEvent EnemyDie;

    private Vector3 startPos;
    private float timer;
    private float boostTimer;
    private bool canHit = false, bosstBool;
    private static readonly int ShootBlend = Animator.StringToHash("ShootBlend");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int TakeDamgeBlend = Animator.StringToHash("TakeDamgeBlend");
    private static readonly int TakeDamge = Animator.StringToHash("TakeDamge");
    private static readonly int Idle = Animator.StringToHash("Idle");


    private void OnEnable()
    {
        startPos = transform.position;
        EventManager.LevelStarted += GameStart;
    }

    private void OnDisable()
    {
        EventManager.LevelStarted -= GameStart;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            if (Input.touchCount > 0)
                foreach (var touch in Input.touches)
                    if (touch.phase == TouchPhase.Began)
                    {
                        bosstBool = true;
                        boostTimer = 0;
                    }

            if (bosstBool)
                boostTimer += Time.deltaTime;

            if (boostTimer >= boostTime) bosstBool = false;

            if (bosstBool)
                timer += Time.deltaTime * 3;
            else
                timer += Time.deltaTime;


            if (canHit)
                if (timer >= IncomeEarningCalculateValue())
                {
                    timer = 0;
                    Hit();
                }

            if (UIManager.Instance.equipedImageForFill)
                UIManager.Instance.equipedImageForFill.fillAmount = timer / IncomeEarningCalculateValue();
        }
    }


    private void GameStart()
    {
        EnemyDie.Raise();
    }

    public GameObject forceField;

    private void ForceFieldOpener()
    {
        forceField.SetActive(true);
    }

    public void ForceFieldMeth()
    {
        forceField.SetActive(false);
        Invoke(nameof(ForceFieldOpener), .75f);
    }

    public void Move()
    {
        playerCharacterManage.SetupRefresh(false);
        if (EnemySpawnManager.Instance.activeEnemies.Count > 0)
        {
            canHit = false;

            TargetEnemy = EnemySpawnManager.Instance.activeEnemies[0].GetComponent<CharacterManage>();
            TargetEnemy.SetHealth(playerCharacterManage.GetLevel(), _characterCostumizeManager.weapon.damageAmount);

            var scaleZMe = ScaleManager.Instance.ScaleAmountFinder(playerCharacterManage.GetLevel(), true).z;
            var scaleZEnemy = ScaleManager.Instance.ScaleAmountFinder(TargetEnemy.GetLevel(), false).z;
            var calculatedDefoultDistanceZTargetEnemy =
                defoultDistanceZTargetEnemy * scaleZMe * ScaleManager.Instance.startScaleSize.z;

            var distanceFromEnemy =
                Vector3.forward * ((scaleZMe + scaleZEnemy) / 2 + calculatedDefoultDistanceZTargetEnemy);
            var targetMovePos = TargetEnemy.transform.position - distanceFromEnemy;

            foreach (var enemy in EnemySpawnManager.Instance.activeEnemies)
                StartCoroutine(MoveWithinSeconds(enemy.transform, enemy.transform.position -
                                                                  (targetMovePos - transform.position),
                    chracterMovmentSec, enemy.GetComponent<CharacterManage>()));
        }
        else if (EnemySpawnManager.Instance.EnemyCount > 0)
        {
            UIManager.Instance.WinLevel();
        }
    }

    private IEnumerator MoveWithinSeconds(Transform obj, Vector3 to, float duration,
        CharacterManage enemyCharacterManage)
    {
        enemyCharacterManage.enemyWalk.Play();
        var timePassed = 0f;
        var startPosition = obj.position;
        while (timePassed < duration)
        {
            obj.position = Vector3.Lerp(startPosition, to, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }

        obj.position = to;
        canHit = true;
        enemyCharacterManage.enemyWalk.Stop();
    }


    private void Hit()
    {
        _animator.SetFloat(ShootBlend, _characterCostumizeManager.weapon.weaponShootBlend);
        _animator.SetTrigger(Shoot);
    }

    public void HitAfterAnimationTrigger()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            TargetEnemy._animator.SetTrigger(TakeDamge);
            TargetEnemy._animator.SetFloat(TakeDamgeBlend, Random.Range(0, 3));


            TargetEnemy.TakeDamage(_characterCostumizeManager.weapon.damageAmount,
                playerCharacterManage.GetLevel());
        }
    }

    public void Fire()
    {
        muzleFlash.transform.position = _characterCostumizeManager.ReturnShootingPoint().position;
        muzleFlash.Play();
    }

    public float IncomeEarningCalculateValue()
    {
        var returnValue = startAtacSpeedUpgradeMultiple - byLevelAtacSpeedReductionAmount * atackSpeedUpgrade.Level;
        if (returnValue > minAtacSpeedAmount)
            return returnValue;
        else
            return minAtacSpeedAmount;
    }

    public void Reset()
    {
        _animator.SetTrigger(Idle);
        StopAllCoroutines();
        timer = 0;
        boostTimer = 0;
        canHit = false;
        bosstBool = false;
    }
}